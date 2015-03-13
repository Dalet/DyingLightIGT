﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DyingLightIGT
{
    class DeepPointer
    {
        private List<int> _offsets;
        private int _base;
        private string _module;

        public DeepPointer(string module, int base_, params int[] offsets)
        {
            _module = module;
            _base = base_;
            _offsets = new List<int>();
            _offsets.Add(0); // deref base first
            _offsets.AddRange(offsets);
        }

        public DeepPointer(int base_, params int[] offsets)
        {
            _base = base_;
            _offsets = new List<int>();
            _offsets.Add(0); // deref base first
            _offsets.AddRange(offsets);
        }

        public bool Deref<T>(Process process, out T value, bool x64 = false) where T : struct
        {
            int offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            if (!this.DerefOffsets(process, out ptr, x64)
                || !ReadProcessValue(process, ptr + offset, out value, x64))
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public bool Deref(Process process, out Vector3f value, bool x64 = false)
        {
            int offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            float x, y, z;
            if (!this.DerefOffsets(process, out ptr, x64)
                || !ReadProcessValue(process, ptr + offset + 0, out x, x64)
                || !ReadProcessValue(process, ptr + offset + 4, out y, x64)
                || !ReadProcessValue(process, ptr + offset + 8, out z, x64))
            {
                value = new Vector3f();
                return false;
            }

            value = new Vector3f(x, y, z);
            return true;
        }

        public bool Deref(Process process, out string str, int max, bool x64 = false)
        {
            var sb = new StringBuilder(max);

            int offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;
            if (!this.DerefOffsets(process, out ptr, x64)
                || !ReadProcessString(process, ptr + offset, sb, x64))
            {
                str = String.Empty;
                return false;
            }

            str = sb.ToString();
            return true;
        }

        bool DerefOffsets(Process process, out IntPtr ptr, bool x64 = false)
        {
            if (!String.IsNullOrEmpty(_module))
            {
                ProcessModule module = process.Modules.Cast<ProcessModule>()
                    .FirstOrDefault(m => Path.GetFileName(m.FileName).ToLower() == _module.ToLower());
                if (module == null)
                {
                    ptr = IntPtr.Zero;
                    return false;
                }

                ptr = module.BaseAddress + _base;
            }
            else
            {
                ptr = process.MainModule.BaseAddress + _base;
            }


            for (int i = 0; i < _offsets.Count - 1; i++)
            {
                if (!x64)
                {
                    if (!ReadProcessPtr32(process, ptr + _offsets[i], out ptr))
                        return false;
                }
                else
                {
                    if (!ReadProcessPtr64(process, ptr + _offsets[i], out ptr))
                        return false;
                }

                if (ptr == IntPtr.Zero)
                    return false;
            }

            return true;
        }

        static bool ReadProcessValue<T>(Process process, IntPtr addr, out T val, bool x64 = false) where T : struct
        {
            Type type = typeof(T);

            var bytes = new byte[Marshal.SizeOf(type)];
            val = default(T);

            if (!x64)
            {
                int read;
                if (!SafeNativeMethods.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read) || read != bytes.Length)
                    return false;
            }
            else
            {
                long read;
                if (!SafeNativeMethods64.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read) || read != bytes.Length)
                    return false;
            }

            if (type == typeof(int))
            {
                val = (T)(object)BitConverter.ToInt32(bytes, 0);
            }
            else if (type == typeof(uint))
            {
                val = (T)(object)BitConverter.ToUInt32(bytes, 0);
            }
            else if (type == typeof(float))
            {
                val = (T)(object)BitConverter.ToSingle(bytes, 0);
            }
            else if (type == typeof(byte))
            {
                val = (T)(object)bytes[0];
            }
            else if (type == typeof(bool))
            {
                val = (T)(object)BitConverter.ToBoolean(bytes, 0);
            }
            else
            {
                throw new Exception("Type not supported.");
            }

            return true;
        }

        static bool ReadProcessPtr32(Process process, IntPtr addr, out IntPtr val)
        {
            byte[] bytes = new byte[4];
            int read;
            val = IntPtr.Zero;
            if (!SafeNativeMethods.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read) || read != bytes.Length)
                return false;
            val = (IntPtr)BitConverter.ToInt32(bytes, 0);
            return true;
        }

        static bool ReadProcessPtr64(Process process, IntPtr addr, out IntPtr val)
        {
            byte[] bytes = new byte[4];
            long read;
            val = IntPtr.Zero;
            if (!SafeNativeMethods64.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read) || read != bytes.Length)
                return false;
            val = (IntPtr)BitConverter.ToInt32(bytes, 0);
            return true;
        }

        static bool ReadProcessString(Process process, IntPtr addr, StringBuilder sb, bool x64 = false)
        {
            byte[] bytes = new byte[sb.Capacity];
            int read32;
            long read;

            if (!x64)
            {
                if (!SafeNativeMethods.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read32) || read32 != bytes.Length)
                    return false;
                read = read32;
            }
            else
            {
                if (!SafeNativeMethods64.ReadProcessMemory(process.Handle, addr, bytes, bytes.Length, out read) || read != bytes.Length)
                    return false;
            }

            if (read >= 2 && bytes[1] == '\x0') // hack to detect utf-16
                sb.Append(Encoding.Unicode.GetString(bytes));
            else
                sb.Append(Encoding.ASCII.GetString(bytes));


            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '\0')
                {
                    sb.Remove(i, sb.Length - i);
                    break;
                }
            }

            return true;
        }

        public IntPtr GetAddress(Process process, bool x64 = false)
        {
            int offset = _offsets[_offsets.Count - 1];
            IntPtr ptr;

            this.DerefOffsets(process, out ptr, x64);
            if (ptr != IntPtr.Zero)
                ptr += offset;
            return ptr;
        }
    }

    public class Vector3f
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int IX { get { return (int)this.X; } }
        public int IY { get { return (int)this.Y; } }
        public int IZ { get { return (int)this.Z; } }

        public Vector3f() { }

        public Vector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float Distance(Vector3f other)
        {
            float result = (this.X - other.X) * (this.X - other.X) +
                (this.Y - other.Y) * (this.Y - other.Y) +
                (this.Z - other.Z) * (this.Z - other.Z);
            return (float)Math.Sqrt(result);
        }

        public float DistanceXY(Vector3f other)
        {
            float result = (this.X - other.X) * (this.X - other.X) +
                (this.Y - other.Y) * (this.Y - other.Y);
            return (float)Math.Sqrt(result);
        }

        public override string ToString()
        {
            return this.X + " " + this.Y + " " + this.Z;
        }
    }
}

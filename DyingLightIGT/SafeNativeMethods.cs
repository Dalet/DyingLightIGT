
using System;
using System.Runtime.InteropServices;

namespace DyingLightIGT
{
    static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize, // should be IntPtr if we ever need to read a size bigger than 32 bit address space
            out int lpNumberOfBytesRead);
    }

    static class SafeNativeMethods64
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize, // should be IntPtr if we ever need to read a size bigger than 32 bit address space
            out long lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64BitProcess(IntPtr pHandle)
        {
            bool retVal;

            IsWow64Process(pHandle, out retVal);
            return !retVal;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DyingLightIGT
{
    public partial class Autosplits : Form
    {
        Settings _settingsWindow;
        BindingList<int> _storyAutosplits;

        public Autosplits(Settings parent)
        {
            InitializeComponent();
            this.MaximumSize = new Size(this.Width, Screen.AllScreens.Max(s => s.Bounds.Height));
            this.MinimumSize = new Size(this.Width, this.Height);

            _settingsWindow = parent;

            _storyAutosplits = new BindingList<int>(_settingsWindow.AutoSplits.OrderBy(i => i).ToList());
            this.lstAutoSplits.DataSource = _storyAutosplits;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.lstAutoSplits.SelectedItem != null)
                _storyAutosplits.Remove((int)this.lstAutoSplits.SelectedItem);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!_storyAutosplits.Contains((int)numPercent.Value))
                _storyAutosplits.Add((int)numPercent.Value);
            _storyAutosplits = new BindingList<int>(_storyAutosplits.OrderBy(i => i).ToList());
            lstAutoSplits.DataSource = _storyAutosplits;
        }

        private void numPercent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnAdd.PerformClick();
        }

        private void lstAutoSplits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnRemove.PerformClick();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _settingsWindow.AutoSplits = _storyAutosplits;
            this.Close();
        }
    }
}

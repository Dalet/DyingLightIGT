namespace LiveSplit.DyingLightIGT
{
    partial class DyingLightIGTSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tlpGeneral = new System.Windows.Forms.TableLayoutPanel();
            this.gbDLIGT = new System.Windows.Forms.GroupBox();
            this.tlpDLIGT = new System.Windows.Forms.TableLayoutPanel();
            this.chkNoGUI = new System.Windows.Forms.CheckBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkAutoReset = new System.Windows.Forms.CheckBox();
            this.gbMisc = new System.Windows.Forms.GroupBox();
            this.tlpMisc = new System.Windows.Forms.TableLayoutPanel();
            this.chkServerAutoStart = new System.Windows.Forms.CheckBox();
            this.tabs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tlpGeneral.SuspendLayout();
            this.gbDLIGT.SuspendLayout();
            this.tlpDLIGT.SuspendLayout();
            this.gbMisc.SuspendLayout();
            this.tlpMisc.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabGeneral);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(7, 7);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(462, 473);
            this.tabs.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tabGeneral.Controls.Add(this.tlpGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(7);
            this.tabGeneral.Size = new System.Drawing.Size(454, 447);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            // 
            // tlpGeneral
            // 
            this.tlpGeneral.AutoSize = true;
            this.tlpGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpGeneral.ColumnCount = 1;
            this.tlpGeneral.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGeneral.Controls.Add(this.gbDLIGT, 0, 0);
            this.tlpGeneral.Controls.Add(this.gbMisc, 0, 1);
            this.tlpGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpGeneral.Location = new System.Drawing.Point(7, 7);
            this.tlpGeneral.Name = "tlpGeneral";
            this.tlpGeneral.RowCount = 2;
            this.tlpGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneral.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGeneral.Size = new System.Drawing.Size(440, 142);
            this.tlpGeneral.TabIndex = 1;
            // 
            // gbDLIGT
            // 
            this.gbDLIGT.AutoSize = true;
            this.gbDLIGT.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbDLIGT.Controls.Add(this.tlpDLIGT);
            this.gbDLIGT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDLIGT.Location = new System.Drawing.Point(3, 3);
            this.gbDLIGT.Name = "gbDLIGT";
            this.gbDLIGT.Size = new System.Drawing.Size(434, 88);
            this.gbDLIGT.TabIndex = 0;
            this.gbDLIGT.TabStop = false;
            this.gbDLIGT.Text = "Dying Light IGT";
            // 
            // tlpDLIGT
            // 
            this.tlpDLIGT.AutoSize = true;
            this.tlpDLIGT.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpDLIGT.ColumnCount = 1;
            this.tlpDLIGT.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDLIGT.Controls.Add(this.chkAutoStart, 0, 1);
            this.tlpDLIGT.Controls.Add(this.chkAutoReset, 0, 2);
            this.tlpDLIGT.Controls.Add(this.chkNoGUI, 0, 0);
            this.tlpDLIGT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDLIGT.Location = new System.Drawing.Point(3, 16);
            this.tlpDLIGT.Name = "tlpDLIGT";
            this.tlpDLIGT.RowCount = 3;
            this.tlpDLIGT.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDLIGT.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDLIGT.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDLIGT.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDLIGT.Size = new System.Drawing.Size(428, 69);
            this.tlpDLIGT.TabIndex = 0;
            // 
            // chkNoGUI
            // 
            this.chkNoGUI.AutoSize = true;
            this.chkNoGUI.Checked = true;
            this.chkNoGUI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNoGUI.Location = new System.Drawing.Point(3, 3);
            this.chkNoGUI.Name = "chkNoGUI";
            this.chkNoGUI.Size = new System.Drawing.Size(235, 17);
            this.chkNoGUI.TabIndex = 1;
            this.chkNoGUI.Text = "Hide Dying Light IGT\'s window (next launch)";
            this.chkNoGUI.UseVisualStyleBackColor = true;
            this.chkNoGUI.CheckedChanged += new System.EventHandler(this.chkNoGUI_CheckedChanged);
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Checked = true;
            this.chkAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoStart.Location = new System.Drawing.Point(3, 26);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.chkAutoStart.Size = new System.Drawing.Size(78, 17);
            this.chkAutoStart.TabIndex = 2;
            this.chkAutoStart.Text = "Auto Start";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // chkAutoReset
            // 
            this.chkAutoReset.AutoSize = true;
            this.chkAutoReset.Checked = true;
            this.chkAutoReset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoReset.Location = new System.Drawing.Point(3, 49);
            this.chkAutoReset.Name = "chkAutoReset";
            this.chkAutoReset.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.chkAutoReset.Size = new System.Drawing.Size(84, 17);
            this.chkAutoReset.TabIndex = 3;
            this.chkAutoReset.Text = "Auto Reset";
            this.chkAutoReset.UseVisualStyleBackColor = true;
            // 
            // gbMisc
            // 
            this.gbMisc.AutoSize = true;
            this.gbMisc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbMisc.Controls.Add(this.tlpMisc);
            this.gbMisc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbMisc.Location = new System.Drawing.Point(3, 97);
            this.gbMisc.Name = "gbMisc";
            this.gbMisc.Size = new System.Drawing.Size(434, 42);
            this.gbMisc.TabIndex = 1;
            this.gbMisc.TabStop = false;
            this.gbMisc.Text = "Misc.";
            // 
            // tlpMisc
            // 
            this.tlpMisc.AutoSize = true;
            this.tlpMisc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMisc.ColumnCount = 1;
            this.tlpMisc.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMisc.Controls.Add(this.chkServerAutoStart, 0, 0);
            this.tlpMisc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMisc.Location = new System.Drawing.Point(3, 16);
            this.tlpMisc.Name = "tlpMisc";
            this.tlpMisc.RowCount = 1;
            this.tlpMisc.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMisc.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tlpMisc.Size = new System.Drawing.Size(428, 23);
            this.tlpMisc.TabIndex = 0;
            // 
            // chkServerAutoStart
            // 
            this.chkServerAutoStart.AutoSize = true;
            this.chkServerAutoStart.Checked = true;
            this.chkServerAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkServerAutoStart.Location = new System.Drawing.Point(3, 3);
            this.chkServerAutoStart.Name = "chkServerAutoStart";
            this.chkServerAutoStart.Size = new System.Drawing.Size(162, 17);
            this.chkServerAutoStart.TabIndex = 0;
            this.chkServerAutoStart.Text = "Start the server automatically";
            this.chkServerAutoStart.UseVisualStyleBackColor = true;
            // 
            // DyingLightIGTSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tabs);
            this.Name = "DyingLightIGTSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 487);
            this.tabs.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tlpGeneral.ResumeLayout(false);
            this.tlpGeneral.PerformLayout();
            this.gbDLIGT.ResumeLayout(false);
            this.gbDLIGT.PerformLayout();
            this.tlpDLIGT.ResumeLayout(false);
            this.tlpDLIGT.PerformLayout();
            this.gbMisc.ResumeLayout(false);
            this.gbMisc.PerformLayout();
            this.tlpMisc.ResumeLayout(false);
            this.tlpMisc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TableLayoutPanel tlpDLIGT;
        private System.Windows.Forms.CheckBox chkServerAutoStart;
        private System.Windows.Forms.CheckBox chkNoGUI;
        private System.Windows.Forms.TableLayoutPanel tlpGeneral;
        private System.Windows.Forms.GroupBox gbDLIGT;
        private System.Windows.Forms.GroupBox gbMisc;
        private System.Windows.Forms.TableLayoutPanel tlpMisc;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkAutoReset;


    }
}

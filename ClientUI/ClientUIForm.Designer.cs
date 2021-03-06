namespace ClientUI
{
    partial class ClientUIForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientUIForm));
            this.ConnectionMenuStrip = new System.Windows.Forms.MenuStrip();
            this.IPAddressMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IPAddressTSTB = new System.Windows.Forms.ToolStripTextBox();
            this.PortMI = new System.Windows.Forms.ToolStripMenuItem();
            this.PortTSTB = new System.Windows.Forms.ToolStripTextBox();
            this.ThemeTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.mooiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.PlayerInfoL = new System.Windows.Forms.Label();
            this.ConnectionMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionMenuStrip
            // 
            this.ConnectionMenuStrip.AccessibleName = "ConnectionMenuStrip";
            this.ConnectionMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IPAddressMI,
            this.IPAddressTSTB,
            this.PortMI,
            this.PortTSTB,
            this.ThemeTSMI});
            this.ConnectionMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.ConnectionMenuStrip.Name = "ConnectionMenuStrip";
            this.ConnectionMenuStrip.Size = new System.Drawing.Size(799, 27);
            this.ConnectionMenuStrip.TabIndex = 0;
            this.ConnectionMenuStrip.Text = "ConnectionMenuStrip";
            // 
            // IPAddressMI
            // 
            this.IPAddressMI.Name = "IPAddressMI";
            this.IPAddressMI.Size = new System.Drawing.Size(76, 23);
            this.IPAddressMI.Text = "IP-Address";
            // 
            // IPAddressTSTB
            // 
            this.IPAddressTSTB.Name = "IPAddressTSTB";
            this.IPAddressTSTB.Size = new System.Drawing.Size(101, 23);
            // 
            // PortMI
            // 
            this.PortMI.Name = "PortMI";
            this.PortMI.Size = new System.Drawing.Size(41, 23);
            this.PortMI.Text = "Port";
            // 
            // PortTSTB
            // 
            this.PortTSTB.Name = "PortTSTB";
            this.PortTSTB.Size = new System.Drawing.Size(101, 23);
            // 
            // ThemeTSMI
            // 
            this.ThemeTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mooiToolStripMenuItem});
            this.ThemeTSMI.Name = "ThemeTSMI";
            this.ThemeTSMI.Size = new System.Drawing.Size(55, 23);
            this.ThemeTSMI.Text = "Theme";
            // 
            // mooiToolStripMenuItem
            // 
            this.mooiToolStripMenuItem.Name = "mooiToolStripMenuItem";
            this.mooiToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.mooiToolStripMenuItem.Text = "Mooi";
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(400, 0);
            this.ConnectBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(90, 28);
            this.ConnectBtn.TabIndex = 1;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // PlayerInfoL
            // 
            this.PlayerInfoL.AutoSize = true;
            this.PlayerInfoL.Dock = System.Windows.Forms.DockStyle.Right;
            this.PlayerInfoL.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PlayerInfoL.Location = new System.Drawing.Point(719, 27);
            this.PlayerInfoL.Name = "PlayerInfoL";
            this.PlayerInfoL.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.PlayerInfoL.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PlayerInfoL.Size = new System.Drawing.Size(80, 16);
            this.PlayerInfoL.TabIndex = 2;
            this.PlayerInfoL.Text = "PlayerInfo:";
            this.PlayerInfoL.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ClientUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 450);
            this.Controls.Add(this.PlayerInfoL);
            this.Controls.Add(this.ConnectBtn);
            this.Controls.Add(this.ConnectionMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ClientUIForm";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.ClientUIForm_Load);
            this.ConnectionMenuStrip.ResumeLayout(false);
            this.ConnectionMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip ConnectionMenuStrip;
        private ToolStripMenuItem IPAddressMI;
        private ToolStripTextBox IPAddressTSTB;
        private ToolStripMenuItem PortMI;
        private ToolStripTextBox PortTSTB;
        private Button ConnectBtn;
        private Label PlayerInfoL;
        private ToolStripMenuItem ThemeTSMI;
        private ToolStripMenuItem mooiToolStripMenuItem;
    }
}
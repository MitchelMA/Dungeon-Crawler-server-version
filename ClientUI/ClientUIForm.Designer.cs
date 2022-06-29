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
            this.ConnectionMenuStrip = new System.Windows.Forms.MenuStrip();
            this.IPAddressMI = new System.Windows.Forms.ToolStripMenuItem();
            this.IPAddressTSTB = new System.Windows.Forms.ToolStripTextBox();
            this.PortMI = new System.Windows.Forms.ToolStripMenuItem();
            this.PortTSTB = new System.Windows.Forms.ToolStripTextBox();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.PlayerInfo = new System.Windows.Forms.Label();
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
            this.PortTSTB});
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
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(349, 0);
            this.ConnectBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(80, 28);
            this.ConnectBtn.TabIndex = 1;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // PlayerInfo
            // 
            this.PlayerInfo.AutoSize = true;
            this.PlayerInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.PlayerInfo.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PlayerInfo.Location = new System.Drawing.Point(719, 27);
            this.PlayerInfo.Name = "PlayerInfo";
            this.PlayerInfo.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.PlayerInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PlayerInfo.Size = new System.Drawing.Size(80, 16);
            this.PlayerInfo.TabIndex = 2;
            this.PlayerInfo.Text = "PlayerInfo:";
            this.PlayerInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ClientUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 450);
            this.Controls.Add(this.PlayerInfo);
            this.Controls.Add(this.ConnectBtn);
            this.Controls.Add(this.ConnectionMenuStrip);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ClientUIForm";
            this.Text = "Client";
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
        private Label PlayerInfo;
    }
}
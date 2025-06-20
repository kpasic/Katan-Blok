namespace ClientApp
{
    partial class ConnectDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnConnect = new Button();
            txtIp = new TextBox();
            txtPort = new TextBox();
            txtUserName = new TextBox();
            btnTesting = new Button();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(266, 30);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(128, 52);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.TextImageRelation = TextImageRelation.ImageAboveText;
            btnConnect.UseMnemonic = false;
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtIp
            // 
            txtIp.Location = new Point(12, 12);
            txtIp.Name = "txtIp";
            txtIp.PlaceholderText = "Ip address";
            txtIp.Size = new Size(219, 23);
            txtIp.TabIndex = 1;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(12, 59);
            txtPort.Name = "txtPort";
            txtPort.PlaceholderText = "Port";
            txtPort.Size = new Size(74, 23);
            txtPort.TabIndex = 2;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(103, 59);
            txtUserName.Name = "txtUserName";
            txtUserName.PlaceholderText = "User Name";
            txtUserName.Size = new Size(139, 23);
            txtUserName.TabIndex = 3;
            // 
            // btnTesting
            // 
            btnTesting.Location = new Point(266, 1);
            btnTesting.Name = "btnTesting";
            btnTesting.Size = new Size(75, 23);
            btnTesting.TabIndex = 4;
            btnTesting.Text = "testig";
            btnTesting.UseVisualStyleBackColor = true;
            btnTesting.Click += btnTesting_Click;
            // 
            // ConnectDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(409, 111);
            Controls.Add(btnTesting);
            Controls.Add(txtUserName);
            Controls.Add(txtPort);
            Controls.Add(txtIp);
            Controls.Add(btnConnect);
            MaximumSize = new Size(425, 150);
            MinimumSize = new Size(425, 150);
            Name = "ConnectDialog";
            Text = "ConnectDialog";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private TextBox txtIp;
        private TextBox txtPort;
        private TextBox txtUserName;
        private Button btnTesting;
    }
}
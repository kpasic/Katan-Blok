namespace ClientApp
{
    partial class MainForm
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
            mainGrid = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnConnectServer = new Button();
            mainGrid.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // mainGrid
            // 
            mainGrid.AutoSize = true;
            mainGrid.ColumnCount = 2;
            mainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainGrid.Controls.Add(tableLayoutPanel2, 1, 0);
            mainGrid.Dock = DockStyle.Fill;
            mainGrid.Location = new Point(0, 0);
            mainGrid.Margin = new Padding(0);
            mainGrid.MinimumSize = new Size(10, 10);
            mainGrid.Name = "mainGrid";
            mainGrid.RowCount = 2;
            mainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            mainGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            mainGrid.Size = new Size(765, 628);
            mainGrid.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(btnConnectServer, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(538, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel2.Size = new Size(224, 496);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // btnConnectServer
            // 
            btnConnectServer.Location = new Point(3, 3);
            btnConnectServer.Name = "btnConnectServer";
            btnConnectServer.Size = new Size(99, 59);
            btnConnectServer.TabIndex = 0;
            btnConnectServer.Text = "Connect to server";
            btnConnectServer.UseVisualStyleBackColor = true;
            btnConnectServer.Click += btnConnectServer_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(765, 628);
            Controls.Add(mainGrid);
            Name = "MainForm";
            Text = "Form1";
            Paint += MainForm_Paint;
            mainGrid.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainGrid;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnConnectServer;
    }
}

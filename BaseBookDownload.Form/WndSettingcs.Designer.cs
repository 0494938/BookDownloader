namespace BaseBookDownload.Frm
{
    partial class WndSettingcs
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.btnSavePathSetting = new System.Windows.Forms.Button();
            this.btnTempPathSetting = new System.Windows.Forms.Button();
            this.txtTempPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Save Path";
            // 
            // txtSavePath
            // 
            this.txtSavePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSavePath.Location = new System.Drawing.Point(179, 57);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.ReadOnly = true;
            this.txtSavePath.Size = new System.Drawing.Size(456, 22);
            this.txtSavePath.TabIndex = 1;
            // 
            // btnSavePathSetting
            // 
            this.btnSavePathSetting.Location = new System.Drawing.Point(641, 57);
            this.btnSavePathSetting.Name = "btnSavePathSetting";
            this.btnSavePathSetting.Size = new System.Drawing.Size(85, 22);
            this.btnSavePathSetting.TabIndex = 2;
            this.btnSavePathSetting.Text = "Browser...";
            this.btnSavePathSetting.UseVisualStyleBackColor = true;
            this.btnSavePathSetting.Click += new System.EventHandler(this.btnSavePathSetting_Click);
            // 
            // btnTempPathSetting
            // 
            this.btnTempPathSetting.Location = new System.Drawing.Point(641, 106);
            this.btnTempPathSetting.Name = "btnTempPathSetting";
            this.btnTempPathSetting.Size = new System.Drawing.Size(85, 22);
            this.btnTempPathSetting.TabIndex = 5;
            this.btnTempPathSetting.Text = "Browser...";
            this.btnTempPathSetting.UseVisualStyleBackColor = true;
            this.btnTempPathSetting.Click += new System.EventHandler(this.btnTempPathSetting_Click);
            // 
            // txtTempPath
            // 
            this.txtTempPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTempPath.Location = new System.Drawing.Point(179, 106);
            this.txtTempPath.Name = "txtTempPath";
            this.txtTempPath.ReadOnly = true;
            this.txtTempPath.Size = new System.Drawing.Size(456, 22);
            this.txtTempPath.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Temporary File Path";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(617, 185);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(99, 22);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // WndSettingcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 223);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnTempPathSetting);
            this.Controls.Add(this.txtTempPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSavePathSetting);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.label1);
            this.Name = "WndSettingcs";
            this.Text = "Save Folder Setting";
            this.Load += new System.EventHandler(this.WndSettingcs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Button btnSavePathSetting;
        private System.Windows.Forms.Button btnTempPathSetting;
        private System.Windows.Forms.TextBox txtTempPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
    }
}
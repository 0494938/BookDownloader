using System.Windows.Forms;

namespace WindowsFormsApp
{
    partial class WindowsFormChrome
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowsFormChrome));
            this.scTop = new System.Windows.Forms.SplitContainer();
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.txtNextUrl = new System.Windows.Forms.TextBox();
            this.cmbNovelType = new System.Windows.Forms.ComboBox();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInitURL = new System.Windows.Forms.TextBox();
            this.scBottom = new System.Windows.Forms.SplitContainer();
            this.scResult = new System.Windows.Forms.SplitContainer();
            this.txtHtml = new System.Windows.Forms.TextBox();
            this.txtContent = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.scTop)).BeginInit();
            this.scTop.Panel1.SuspendLayout();
            this.scTop.Panel2.SuspendLayout();
            this.scTop.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scBottom)).BeginInit();
            this.scBottom.Panel2.SuspendLayout();
            this.scBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scResult)).BeginInit();
            this.scResult.Panel1.SuspendLayout();
            this.scResult.Panel2.SuspendLayout();
            this.scResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // scTop
            // 
            this.scTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTop.Location = new System.Drawing.Point(0, 0);
            this.scTop.Name = "scTop";
            this.scTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTop.Panel1
            // 
            this.scTop.Panel1.Controls.Add(this.panelTop);
            // 
            // scTop.Panel2
            // 
            this.scTop.Panel2.Controls.Add(this.scBottom);
            this.scTop.Panel2.Validated += new System.EventHandler(this.splitContainer1_Panel2_Validated);
            this.scTop.Size = new System.Drawing.Size(1601, 1012);
            this.scTop.SplitterDistance = 200;
            this.scTop.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.AutoSize = true;
            this.panelTop.Controls.Add(this.txtLog);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.btnNextPage);
            this.panelTop.Controls.Add(this.txtNextUrl);
            this.panelTop.Controls.Add(this.cmbNovelType);
            this.panelTop.Controls.Add(this.btnBrowser);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.txtInitURL);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1601, 200);
            this.panelTop.TabIndex = 0;
            this.panelTop.Resize += new System.EventHandler(this.panelTop_Resize);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtLog.Font = new System.Drawing.Font("MS UI Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtLog.Location = new System.Drawing.Point(13, 82);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1576, 115);
            this.txtLog.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.label2.Location = new System.Drawing.Point(315, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Next URL";
            // 
            // btnNextPage
            // 
            this.btnNextPage.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.btnNextPage.Location = new System.Drawing.Point(1410, 44);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(100, 30);
            this.btnNextPage.TabIndex = 5;
            this.btnNextPage.Text = "Browser";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // txtNextUrl
            // 
            this.txtNextUrl.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtNextUrl.Location = new System.Drawing.Point(420, 46);
            this.txtNextUrl.Name = "txtNextUrl";
            this.txtNextUrl.Size = new System.Drawing.Size(963, 27);
            this.txtNextUrl.TabIndex = 4;
            // 
            // cmbNovelType
            // 
            this.cmbNovelType.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbNovelType.FormattingEnabled = true;
            this.cmbNovelType.Items.AddRange(new object[] {
            "0 爱看书吧 m.ikbook8.com",
            "1 QQ电子书 book.qq.com",
            "2 笔趣阁 m.xbiqugew.com",
            "3 笔趣阁2 m.xbiqugew.com",
            "4 无线电子书 www.wxdzs.net",
            "5 苍穹小说 www.cqhhhs.com",
            "6 金庸小说网 www.jinhuaja.com",
            "7 书旗小说网 www.shuqi.com",
            "8 番茄小说网 fanqienovel.com",
            "9 番茄小说网2 fanqienovel.com",
            "10 红袖添香 www.hongxiu.com",
            "11 新小说吧 www.xxs8.com",
            "12 言情小说吧 www.xs8.cn",
            "13 Bad 17小说网 www.17k.com"});
            this.cmbNovelType.Location = new System.Drawing.Point(13, 13);
            this.cmbNovelType.Name = "cmbNovelType";
            this.cmbNovelType.Size = new System.Drawing.Size(289, 28);
            this.cmbNovelType.TabIndex = 3;
            this.cmbNovelType.Text = "无线电子书 www.wxdzs.net";
            this.cmbNovelType.SelectedIndexChanged += new System.EventHandler(this.cmbNovelType_SelectedIndexChanged);
            // 
            // btnBrowser
            // 
            this.btnBrowser.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.btnBrowser.Location = new System.Drawing.Point(1410, 12);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(100, 30);
            this.btnBrowser.TabIndex = 2;
            this.btnBrowser.Text = "Browser";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.label1.Location = new System.Drawing.Point(315, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Init URL";
            // 
            // txtInitURL
            // 
            this.txtInitURL.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtInitURL.Location = new System.Drawing.Point(420, 14);
            this.txtInitURL.Name = "txtInitURL";
            this.txtInitURL.Size = new System.Drawing.Size(963, 27);
            this.txtInitURL.TabIndex = 0;
            this.txtInitURL.Text = "https://www.wxdzs.net/wxread/94612_43816524.html";
            // 
            // scBottom
            // 
            this.scBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scBottom.Location = new System.Drawing.Point(0, 0);
            this.scBottom.Name = "scBottom";
            // 
            // scBottom.Panel2
            // 
            this.scBottom.Panel2.Controls.Add(this.scResult);
            this.scBottom.Size = new System.Drawing.Size(1601, 808);
            this.scBottom.SplitterDistance = 799;
            this.scBottom.TabIndex = 0;
            // 
            // scResult
            // 
            this.scResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scResult.Location = new System.Drawing.Point(0, 0);
            this.scResult.Name = "scResult";
            this.scResult.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scResult.Panel1
            // 
            this.scResult.Panel1.Controls.Add(this.txtHtml);
            this.scResult.Panel1.Resize += new System.EventHandler(this.scResult_Panel1_Resize);
            // 
            // scResult.Panel2
            // 
            this.scResult.Panel2.Controls.Add(this.txtContent);
            this.scResult.Size = new System.Drawing.Size(798, 808);
            this.scResult.SplitterDistance = 354;
            this.scResult.TabIndex = 0;
            // 
            // txtHtml
            // 
            this.txtHtml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHtml.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.txtHtml.Location = new System.Drawing.Point(0, 0);
            this.txtHtml.Multiline = true;
            this.txtHtml.Name = "txtHtml";
            this.txtHtml.Size = new System.Drawing.Size(798, 354);
            this.txtHtml.TabIndex = 0;
            // 
            // txtContent
            // 
            this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContent.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.txtContent.Location = new System.Drawing.Point(0, 0);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(798, 450);
            this.txtContent.TabIndex = 0;
            // 
            // WindowsFormChrome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1601, 1012);
            this.Controls.Add(this.scTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WindowsFormChrome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windows Web Form ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.WindowsForm_Load);
            this.scTop.Panel1.ResumeLayout(false);
            this.scTop.Panel1.PerformLayout();
            this.scTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTop)).EndInit();
            this.scTop.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.scBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scBottom)).EndInit();
            this.scBottom.ResumeLayout(false);
            this.scResult.Panel1.ResumeLayout(false);
            this.scResult.Panel1.PerformLayout();
            this.scResult.Panel2.ResumeLayout(false);
            this.scResult.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scResult)).EndInit();
            this.scResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scTop;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInitURL;
        private System.Windows.Forms.ComboBox cmbNovelType;
        private System.Windows.Forms.TextBox txtNextUrl;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.SplitContainer scBottom;
        private System.Windows.Forms.SplitContainer scResult;
        private System.Windows.Forms.TextBox txtHtml;
        private System.Windows.Forms.TextBox txtContent;
    }
}


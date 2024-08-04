using System.Windows.Forms;

namespace BookDownloadFormApp
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
            this.txtPages = new System.Windows.Forms.TextBox();
            this.btnAutoDownload = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.txtNextUrl = new System.Windows.Forms.TextBox();
            this.cmbNovelType = new System.Windows.Forms.ComboBox();
            this.btnInitURL = new System.Windows.Forms.Button();
            this.txtInitURL = new System.Windows.Forms.TextBox();
            this.scBottom = new System.Windows.Forms.SplitContainer();
            this.scResult = new System.Windows.Forms.SplitContainer();
            this.txtHtml = new System.Windows.Forms.TextBox();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.txtStatus = new System.Windows.Forms.Label();
            this.txtProgress = new System.Windows.Forms.ProgressBar();
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
            this.gridPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // scTop
            // 
            this.scTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTop.Location = new System.Drawing.Point(3, 3);
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
            this.scTop.Size = new System.Drawing.Size(1595, 976);
            this.scTop.SplitterDistance = 191;
            this.scTop.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.AutoSize = true;
            this.panelTop.Controls.Add(this.txtPages);
            this.panelTop.Controls.Add(this.btnAutoDownload);
            this.panelTop.Controls.Add(this.txtLog);
            this.panelTop.Controls.Add(this.btnNextPage);
            this.panelTop.Controls.Add(this.txtNextUrl);
            this.panelTop.Controls.Add(this.cmbNovelType);
            this.panelTop.Controls.Add(this.btnInitURL);
            this.panelTop.Controls.Add(this.txtInitURL);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1595, 191);
            this.panelTop.TabIndex = 0;
            this.panelTop.Resize += new System.EventHandler(this.panelTop_Resize);
            // 
            // txtPages
            // 
            this.txtPages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPages.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.txtPages.Location = new System.Drawing.Point(1457, 46);
            this.txtPages.Name = "txtPages";
            this.txtPages.Size = new System.Drawing.Size(132, 27);
            this.txtPages.TabIndex = 7;
            this.txtPages.Text = "20";
            this.txtPages.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPages.TextChanged += new System.EventHandler(this.txtPages_TextChanged);
            this.txtPages.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPages_KeyPress);
            // 
            // btnAutoDownload
            // 
            this.btnAutoDownload.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAutoDownload.Location = new System.Drawing.Point(1457, 13);
            this.btnAutoDownload.Name = "btnAutoDownload";
            this.btnAutoDownload.Size = new System.Drawing.Size(132, 30);
            this.btnAutoDownload.TabIndex = 4;
            this.btnAutoDownload.Text = "-> Auto ->";
            this.btnAutoDownload.UseVisualStyleBackColor = true;
            this.btnAutoDownload.Click += new System.EventHandler(this.btnAutoDownload_Click);
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
            // btnNextPage
            // 
            this.btnNextPage.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.btnNextPage.Location = new System.Drawing.Point(334, 44);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(129, 30);
            this.btnNextPage.TabIndex = 5;
            this.btnNextPage.Text = "-> Next Url";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // txtNextUrl
            // 
            this.txtNextUrl.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtNextUrl.Location = new System.Drawing.Point(469, 46);
            this.txtNextUrl.Name = "txtNextUrl";
            this.txtNextUrl.Size = new System.Drawing.Size(982, 27);
            this.txtNextUrl.TabIndex = 6;
            // 
            // cmbNovelType
            // 
            this.cmbNovelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            "13 17小说网 www.17k.com",
            "14 可乐小说网 www.keleshuba.net",
            "15 天天小说 m.ttshu8.com",
            "16 天天小说PC版 www.ttshu8.com",
            "17 69电子书 69shuba.cx"});
            this.cmbNovelType.Location = new System.Drawing.Point(13, 14);
            this.cmbNovelType.Name = "cmbNovelType";
            this.cmbNovelType.Size = new System.Drawing.Size(315, 28);
            this.cmbNovelType.TabIndex = 1;
            this.cmbNovelType.SelectedIndexChanged += new System.EventHandler(this.cmbNovelType_SelectedIndexChanged);
            // 
            // btnInitURL
            // 
            this.btnInitURL.Font = new System.Drawing.Font("MS UI Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnInitURL.Location = new System.Drawing.Point(334, 13);
            this.btnInitURL.Name = "btnInitURL";
            this.btnInitURL.Size = new System.Drawing.Size(129, 30);
            this.btnInitURL.TabIndex = 2;
            this.btnInitURL.Text = "-> Init Url";
            this.btnInitURL.UseVisualStyleBackColor = true;
            this.btnInitURL.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // txtInitURL
            // 
            this.txtInitURL.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtInitURL.Location = new System.Drawing.Point(469, 15);
            this.txtInitURL.Name = "txtInitURL";
            this.txtInitURL.Size = new System.Drawing.Size(982, 27);
            this.txtInitURL.TabIndex = 3;
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
            this.scBottom.Size = new System.Drawing.Size(1595, 781);
            this.scBottom.SplitterDistance = 796;
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
            this.scResult.Size = new System.Drawing.Size(795, 781);
            this.scResult.SplitterDistance = 341;
            this.scResult.TabIndex = 0;
            // 
            // txtHtml
            // 
            this.txtHtml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHtml.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.txtHtml.Location = new System.Drawing.Point(0, 0);
            this.txtHtml.Multiline = true;
            this.txtHtml.Name = "txtHtml";
            this.txtHtml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHtml.Size = new System.Drawing.Size(795, 341);
            this.txtHtml.TabIndex = 8;
            // 
            // txtContent
            // 
            this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContent.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.txtContent.Location = new System.Drawing.Point(0, 0);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(795, 436);
            this.txtContent.TabIndex = 9;
            // 
            // gridPanel
            // 
            this.gridPanel.ColumnCount = 1;
            this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.gridPanel.Controls.Add(this.scTop);
            this.gridPanel.Controls.Add(this.statusPanel, 0, 1);
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Location = new System.Drawing.Point(0, 0);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.RowCount = 2;
            this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.gridPanel.Size = new System.Drawing.Size(1601, 1012);
            this.gridPanel.TabIndex = 10;
            // 
            // statusPanel
            // 
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.statusPanel.Controls.Add(this.txtStatus);
            this.statusPanel.Controls.Add(this.txtProgress);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel.Location = new System.Drawing.Point(3, 985);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1595, 24);
            this.statusPanel.TabIndex = 1;
            this.statusPanel.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // txtStatus
            // 
            this.txtStatus.AutoSize = true;
            this.txtStatus.Location = new System.Drawing.Point(3, 0);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(43, 15);
            this.txtStatus.TabIndex = 0;
            this.txtStatus.Text = "label1";
            // 
            // txtProgress
            // 
            this.txtProgress.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.txtProgress.Location = new System.Drawing.Point(52, 3);
            this.txtProgress.Name = "txtProgress";
            this.txtProgress.Size = new System.Drawing.Size(147, 23);
            this.txtProgress.TabIndex = 1;
            // 
            // WindowsFormChrome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1601, 1012);
            this.Controls.Add(this.gridPanel);
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
            this.gridPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scTop;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnInitURL;
        private System.Windows.Forms.TextBox txtInitURL;
        private System.Windows.Forms.ComboBox cmbNovelType;
        private System.Windows.Forms.TextBox txtNextUrl;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.SplitContainer scBottom;
        private System.Windows.Forms.SplitContainer scResult;
        private System.Windows.Forms.TextBox txtHtml;
        private System.Windows.Forms.TextBox txtContent;
        private Button btnAutoDownload;
        private TextBox txtPages;
        private TableLayoutPanel gridPanel;
        private Panel statusPanel;
        private Label txtStatus;
        private ProgressBar txtProgress;
    }
}


using JsonTreeView2.Controls;
namespace PMSJsonConfigWebApp.Forms
{
    partial class JsonEditorMainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            jsonTreeViewSplitContainer = new SplitContainer();
            jTokenTree = new JTokenTreeUserControl();
            jsonValueTextBox = new TextBox();
            label1 = new Label();
            stringEntryptTextBox = new TextBox();
            jsonValueLabel = new Label();
            label2 = new Label();
            newtonsoftJsonTypeTextBox = new TextBox();
            jsonTypeComboBox = new ComboBox();
            labelJsonStringMaHoa = new Label();
            formMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            newJsonObjectToolStripMenuItem = new ToolStripMenuItem();
            newJsonArrayToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            aboutJsonEditorToolStripMenuItem = new ToolStripMenuItem();
            guiStatusStrip = new StatusStrip();
            actionStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            jsonStatusLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)jsonTreeViewSplitContainer).BeginInit();
            jsonTreeViewSplitContainer.Panel1.SuspendLayout();
            jsonTreeViewSplitContainer.Panel2.SuspendLayout();
            jsonTreeViewSplitContainer.SuspendLayout();
            formMenuStrip.SuspendLayout();
            guiStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // jsonTreeViewSplitContainer
            // 
            jsonTreeViewSplitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            jsonTreeViewSplitContainer.Location = new Point(0, 28);
            jsonTreeViewSplitContainer.Margin = new Padding(4, 3, 4, 3);
            jsonTreeViewSplitContainer.Name = "jsonTreeViewSplitContainer";
            // 
            // jsonTreeViewSplitContainer.Panel1
            // 
            jsonTreeViewSplitContainer.Panel1.Controls.Add(jTokenTree);
            jsonTreeViewSplitContainer.Panel1MinSize = 200;
            // 
            // jsonTreeViewSplitContainer.Panel2
            // 
            jsonTreeViewSplitContainer.Panel2.BackColor = Color.Transparent;
            jsonTreeViewSplitContainer.Panel2.Controls.Add(jsonValueTextBox);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(label1);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(stringEntryptTextBox);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(jsonValueLabel);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(label2);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(newtonsoftJsonTypeTextBox);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(jsonTypeComboBox);
            jsonTreeViewSplitContainer.Panel2.Controls.Add(labelJsonStringMaHoa);
            jsonTreeViewSplitContainer.Panel2.Paint += jsonTreeViewSplitContainer_Panel2_Paint;
            jsonTreeViewSplitContainer.Panel2MinSize = 320;
            jsonTreeViewSplitContainer.Size = new Size(1176, 637);
            jsonTreeViewSplitContainer.SplitterDistance = 784;
            jsonTreeViewSplitContainer.SplitterWidth = 5;
            jsonTreeViewSplitContainer.TabIndex = 8;
            // 
            // jTokenTree
            // 
            jTokenTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            jTokenTree.CollapsedFont = new Font("Microsoft Sans Serif", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            jTokenTree.ExpandedFont = new Font("Microsoft Sans Serif", 7.8F, FontStyle.Underline);
            jTokenTree.Location = new Point(0, 23);
            jTokenTree.Margin = new Padding(7);
            jTokenTree.Name = "jTokenTree";
            jTokenTree.Size = new Size(777, 609);
            jTokenTree.TabIndex = 2;
            jTokenTree.AfterSelect += jTokenTree_AfterSelect;
            // 
            // jsonValueTextBox
            // 
            jsonValueTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            jsonValueTextBox.Font = new Font("Consolas", 8.25F);
            jsonValueTextBox.Location = new Point(4, 22);
            jsonValueTextBox.Margin = new Padding(4, 3, 4, 3);
            jsonValueTextBox.Multiline = true;
            jsonValueTextBox.Name = "jsonValueTextBox";
            jsonValueTextBox.ScrollBars = ScrollBars.Vertical;
            jsonValueTextBox.Size = new Size(377, 352);
            jsonValueTextBox.TabIndex = 6;
            jsonValueTextBox.TextChanged += jsonValueTextBox_TextChanged_1;
            jsonValueTextBox.Enter += jsonValueTextBox_Enter;
            jsonValueTextBox.Leave += jsonValueTextBox_Leave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 49);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 1;
            label1.Text = "JSON Type";
            label1.Visible = false;
            // 
            // stringEntryptTextBox
            // 
            stringEntryptTextBox.Location = new Point(4, 395);
            stringEntryptTextBox.Margin = new Padding(4, 3, 4, 3);
            stringEntryptTextBox.Multiline = true;
            stringEntryptTextBox.Name = "stringEntryptTextBox";
            stringEntryptTextBox.ScrollBars = ScrollBars.Vertical;
            stringEntryptTextBox.Size = new Size(379, 237);
            stringEntryptTextBox.TabIndex = 7;
            stringEntryptTextBox.TextChanged += stringEntryptTextBox_TextChanged;
            // 
            // jsonValueLabel
            // 
            jsonValueLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            jsonValueLabel.AutoSize = true;
            jsonValueLabel.Location = new Point(47, 5);
            jsonValueLabel.Margin = new Padding(4, 0, 4, 0);
            jsonValueLabel.Name = "jsonValueLabel";
            jsonValueLabel.Size = new Size(39, 15);
            jsonValueLabel.TabIndex = 5;
            jsonValueLabel.Text = "Giá Trị";
            jsonValueLabel.TextChanged += jsonValueTextBox_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(103, 5);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(123, 15);
            label2.TabIndex = 3;
            label2.Text = "NewtonSoft.Json Type";
            label2.Visible = false;
            // 
            // newtonsoftJsonTypeTextBox
            // 
            newtonsoftJsonTypeTextBox.Location = new Point(47, 23);
            newtonsoftJsonTypeTextBox.Margin = new Padding(4, 3, 4, 3);
            newtonsoftJsonTypeTextBox.Name = "newtonsoftJsonTypeTextBox";
            newtonsoftJsonTypeTextBox.ReadOnly = true;
            newtonsoftJsonTypeTextBox.Size = new Size(179, 23);
            newtonsoftJsonTypeTextBox.TabIndex = 4;
            newtonsoftJsonTypeTextBox.Visible = false;
            // 
            // jsonTypeComboBox
            // 
            jsonTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            jsonTypeComboBox.Enabled = false;
            jsonTypeComboBox.FormattingEnabled = true;
            jsonTypeComboBox.Location = new Point(47, 67);
            jsonTypeComboBox.Margin = new Padding(4, 3, 4, 3);
            jsonTypeComboBox.Name = "jsonTypeComboBox";
            jsonTypeComboBox.Size = new Size(179, 23);
            jsonTypeComboBox.TabIndex = 7;
            jsonTypeComboBox.Visible = false;
            // 
            // labelJsonStringMaHoa
            // 
            labelJsonStringMaHoa.AutoSize = true;
            labelJsonStringMaHoa.Location = new Point(15, 377);
            labelJsonStringMaHoa.Margin = new Padding(4, 0, 4, 0);
            labelJsonStringMaHoa.Name = "labelJsonStringMaHoa";
            labelJsonStringMaHoa.Size = new Size(83, 15);
            labelJsonStringMaHoa.TabIndex = 0;
            labelJsonStringMaHoa.Text = "String Mã Hóa";
            labelJsonStringMaHoa.Click += label3_Click;
            // 
            // formMenuStrip
            // 
            formMenuStrip.ImageScalingSize = new Size(20, 20);
            formMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem });
            formMenuStrip.Location = new Point(0, 0);
            formMenuStrip.Name = "formMenuStrip";
            formMenuStrip.Padding = new Padding(5, 2, 0, 2);
            formMenuStrip.Size = new Size(1176, 24);
            formMenuStrip.TabIndex = 0;
            formMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(57, 20);
            fileToolStripMenuItem.Text = "&Tập Tin";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newJsonObjectToolStripMenuItem, newJsonArrayToolStripMenuItem });
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShowShortcutKeys = false;
            newToolStripMenuItem.Size = new Size(177, 22);
            newToolStripMenuItem.Text = "&New";
            newToolStripMenuItem.Visible = false;
            // 
            // newJsonObjectToolStripMenuItem
            // 
            newJsonObjectToolStripMenuItem.Name = "newJsonObjectToolStripMenuItem";
            newJsonObjectToolStripMenuItem.Size = new Size(162, 22);
            newJsonObjectToolStripMenuItem.Text = "New Json &Object";
            newJsonObjectToolStripMenuItem.Click += newJsonObjectToolStripMenuItem_Click;
            // 
            // newJsonArrayToolStripMenuItem
            // 
            newJsonArrayToolStripMenuItem.Name = "newJsonArrayToolStripMenuItem";
            newJsonArrayToolStripMenuItem.Size = new Size(162, 22);
            newJsonArrayToolStripMenuItem.Text = "New Json &Array";
            newJsonArrayToolStripMenuItem.Click += newJsonArrayToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(177, 22);
            openToolStripMenuItem.Text = "&Mở";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(177, 22);
            saveToolStripMenuItem.Text = "&Lưu";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(177, 22);
            saveAsToolStripMenuItem.Text = "Lưu &Với";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutJsonEditorToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(24, 20);
            aboutToolStripMenuItem.Text = "?";
            // 
            // aboutJsonEditorToolStripMenuItem
            // 
            aboutJsonEditorToolStripMenuItem.Name = "aboutJsonEditorToolStripMenuItem";
            aboutJsonEditorToolStripMenuItem.Size = new Size(167, 22);
            aboutJsonEditorToolStripMenuItem.Text = "About Json Editor";
            aboutJsonEditorToolStripMenuItem.Click += aboutJsonEditorToolStripMenuItem_Click;
            // 
            // guiStatusStrip
            // 
            guiStatusStrip.ImageScalingSize = new Size(20, 20);
            guiStatusStrip.Items.AddRange(new ToolStripItem[] { actionStatusLabel, toolStripStatusLabel1, jsonStatusLabel });
            guiStatusStrip.Location = new Point(0, 671);
            guiStatusStrip.Name = "guiStatusStrip";
            guiStatusStrip.Padding = new Padding(1, 0, 16, 0);
            guiStatusStrip.Size = new Size(1176, 22);
            guiStatusStrip.TabIndex = 9;
            guiStatusStrip.Text = "statusStrip";
            // 
            // actionStatusLabel
            // 
            actionStatusLabel.Name = "actionStatusLabel";
            actionStatusLabel.Size = new Size(39, 17);
            actionStatusLabel.Text = "Status";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(1058, 17);
            toolStripStatusLabel1.Spring = true;
            // 
            // jsonStatusLabel
            // 
            jsonStatusLabel.Name = "jsonStatusLabel";
            jsonStatusLabel.Size = new Size(62, 17);
            jsonStatusLabel.Text = "JsonStatus";
            // 
            // JsonEditorMainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1176, 693);
            Controls.Add(guiStatusStrip);
            Controls.Add(jsonTreeViewSplitContainer);
            Controls.Add(formMenuStrip);
            MainMenuStrip = formMenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            Name = "JsonEditorMainForm";
            Text = "Json Editor";
            jsonTreeViewSplitContainer.Panel1.ResumeLayout(false);
            jsonTreeViewSplitContainer.Panel2.ResumeLayout(false);
            jsonTreeViewSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)jsonTreeViewSplitContainer).EndInit();
            jsonTreeViewSplitContainer.ResumeLayout(false);
            formMenuStrip.ResumeLayout(false);
            formMenuStrip.PerformLayout();
            guiStatusStrip.ResumeLayout(false);
            guiStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip formMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox newtonsoftJsonTypeTextBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label jsonValueLabel;
        private System.Windows.Forms.SplitContainer jsonTreeViewSplitContainer;
        private System.Windows.Forms.TextBox jsonValueTextBox;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newJsonObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newJsonArrayToolStripMenuItem;
        private System.Windows.Forms.ComboBox jsonTypeComboBox;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutJsonEditorToolStripMenuItem;
        private JTokenTreeUserControl jTokenTree;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.StatusStrip guiStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel actionStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel jsonStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label labelJsonStringMaHoa;
        private System.Windows.Forms.TextBox stringEntryptTextBox;
    }
}
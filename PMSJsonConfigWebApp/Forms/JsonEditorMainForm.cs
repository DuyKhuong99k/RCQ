using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using JsonTreeView2.Views;
using Security.Crypt;

namespace PMSJsonConfigWebApp.Forms
{
    public sealed partial class JsonEditorMainForm : Form
    {
        private const string DefaultFileFilters = @"json files (*.json)|*.json";
        private bool isUpdatingTextBoxes = false;
        #region >> Delegates

        private delegate void SetActionStatusDelegate(string text, bool isError);

        private delegate void SetJsonStatusDelegate(string text, bool isError);

        #endregion

        #region >> Fields

        private string internalOpenedFileName;

        private System.Timers.Timer jsonValidationTimer;

        #endregion

        #region >> Properties

        /// <summary>
        /// Accessor to file name of opened file.
        /// </summary>
        string OpenedFileName
        {
            get { return internalOpenedFileName; }
            set
            {
                internalOpenedFileName = value;
                saveToolStripMenuItem.Enabled = internalOpenedFileName != null;
                saveAsToolStripMenuItem.Enabled = internalOpenedFileName != null;
                Text = (internalOpenedFileName ?? "") + @" - Json Editor";
            }
        }

        #endregion

        #region >> Constructor

        public JsonEditorMainForm()
        {
            InitializeComponent();

            jsonTypeComboBox.DataSource = Enum.GetValues(typeof(JTokenType));

            OpenedFileName = null;
            SetActionStatus(@"Empty document.", true);
            SetJsonStatus(@"", false);

            //var commandLineArgs = Environment.GetCommandLineArgs();
            //if (commandLineArgs.Skip(1).Any())
            //{
            OpenedFileName = $@"{System.AppDomain.CurrentDomain.BaseDirectory}\Settings.json";
            try
            {
                using (var stream = new FileStream(OpenedFileName, FileMode.Open))
                {
                    SetJsonSourceStream(stream, OpenedFileName);
                }
            }
            catch
            {
                OpenedFileName = null;
            }
            //}

        }

        #endregion

        #region >> Form

        /// <inheritdoc />
        /// <remarks>
        /// Optimization aiming to reduce flickering on large documents (successfully).
        /// Source: http://stackoverflow.com/a/89125/1774251
        /// </remarks>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"json files (*.json)|*.json|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var stream = openFileDialog.OpenFile())
                {
                    SetJsonSourceStream(stream, openFileDialog.FileName);
                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenedFileName == null)
            {
                return;
            }

            try
            {
                using (var stream = new FileStream(OpenedFileName, FileMode.Open))
                {
                    jTokenTree.SaveJson(stream);
                }
            }
            catch
            {
                MessageBox.Show(this, $"An error occured when saving file as \"{OpenedFileName}\".", @"Save As...");

                OpenedFileName = null;
                SetActionStatus(@"Document NOT saved.", true);

                return;
            }

            SetActionStatus(@"Document successfully saved.", false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = DefaultFileFilters,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                OpenedFileName = saveFileDialog.FileName;
                using (var stream = saveFileDialog.OpenFile())
                {
                    if (stream.CanWrite)
                    {
                        jTokenTree.SaveJson(stream);
                    }
                }
            }
            catch
            {
                MessageBox.Show(this, $"An error occured when saving file as \"{OpenedFileName}\".", @"Save As...");

                OpenedFileName = null;
                SetActionStatus(@"Document NOT saved.", true);

                return;
            }

            SetActionStatus(@"Document successfully saved.", false);
        }

        private void newJsonObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jTokenTree.SetJsonSource("{}");

            saveAsToolStripMenuItem.Enabled = true;
        }

        private void newJsonArrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jTokenTree.SetJsonSource("[]");

            saveAsToolStripMenuItem.Enabled = true;
        }

        private void aboutJsonEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new AboutBox().ShowDialog();
        }

        private void jsonValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingTextBoxes) return; // Nếu đang cập nhật thì không làm gì
            try
            {
                isUpdatingTextBoxes = true; // Bắt đầu cập nhật

                // Lấy giá trị từ jsonValueTextBox
                string jsonText = jsonValueTextBox.Text;

                // Mã hóa chuỗi
                string encryptedText = ED.EncryptString(jsonText, AppViewModels.AppViewModel.Instance.DefaultKey);
                string currentDate = "Version: " + DateTime.Now.ToString("yyyyMMddHHssmm");
                // Cập nhật giá trị cho stringEntryptTextBox
                stringEntryptTextBox.Text = encryptedText + Environment.NewLine + currentDate;
                StartValidationTimer(); // Bắt đầu kiểm tra validation
            }
            finally
            {
                isUpdatingTextBoxes = false; // Hoàn tất cập nhật
            }
        }
        private void stringEntryptTextBox_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingTextBoxes) return; // Nếu đang cập nhật thì không làm gì

            StartValidationTimer(); // Bắt đầu kiểm tra validation

            try
            {
                isUpdatingTextBoxes = true; // Bắt đầu cập nhật

                // Lấy giá trị từ stringEntryptTextBox
                string encryptedText = stringEntryptTextBox.Text;

                // Chỉ lấy dòng đầu tiên của chuỗi (ngắt chuỗi bằng Environment.NewLine hoặc ký tự xuống dòng)
                string firstLine = encryptedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];

                // Giải mã chuỗi từ dòng đầu tiên
                string decryptedText = ED.DecryptString(firstLine, AppViewModels.AppViewModel.Instance.DefaultKey);

                // Cập nhật giá trị cho jsonValueTextBox
                jsonValueTextBox.Text = decryptedText;
            }
            finally
            {
                isUpdatingTextBoxes = false; // Hoàn tất cập nhật
            }
        }
        private void jsonValueTextBox_Leave(object sender, EventArgs e)
        {
            jsonValueTextBox.TextChanged -= jsonValueTextBox_TextChanged;
        }

        private void jsonValueTextBox_Enter(object sender, EventArgs e)
        {
            jsonValueTextBox.TextChanged += jsonValueTextBox_TextChanged;
        }

        private void jTokenTree_AfterSelect(object sender, JsonTreeView2.AfterSelectEventArgs eventArgs)
        {
            newtonsoftJsonTypeTextBox.Text = eventArgs.TypeName;

            jsonTypeComboBox.Text = eventArgs.JTokenTypeName;

            // If jsonValueTextBox is focused then it triggers this event in the update process, so don't update it again ! (risk: infinite loop between events).
            if (!jsonValueTextBox.Focused)
            {
                jsonValueTextBox.Text = eventArgs.GetJsonString();
            }
        }

        private void SetJsonSourceStream(Stream stream, string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            OpenedFileName = fileName;

            try
            {
                jTokenTree.SetJsonSource(stream);
            }
            catch
            {
                MessageBox.Show(this, $"An error occured when reading \"{OpenedFileName}\"", @"Open...");

                OpenedFileName = null;
                SetActionStatus(@"Document NOT loaded.", true);

                return;
            }

            SetActionStatus(@"Document successfully loaded.", false);
            saveAsToolStripMenuItem.Enabled = true;
        }

        private void SetActionStatus(string text, bool isError)
        {
            if (InvokeRequired)
            {
                Invoke(new SetActionStatusDelegate(SetActionStatus), text, isError);
                return;
            }

            actionStatusLabel.Text = text;
            actionStatusLabel.ForeColor = isError ? Color.OrangeRed : Color.Black;
        }

        private void SetJsonStatus(string text, bool isError)
        {
            if (InvokeRequired)
            {
                Invoke(new SetJsonStatusDelegate(SetActionStatus), text, isError);
                return;
            }

            jsonStatusLabel.Text = text;
            jsonStatusLabel.ForeColor = isError ? Color.OrangeRed : Color.Black;
        }

        private void StartValidationTimer()
        {
            jsonValidationTimer?.Stop();

            jsonValidationTimer = new System.Timers.Timer(250);

            jsonValidationTimer.Elapsed += (o, args) =>
            {
                jsonValidationTimer.Stop();

                jTokenTree.Invoke(new Action(JsonValidationTimerHandler));
            };

            jsonValidationTimer.Start();
        }

        private void JsonValidationTimerHandler()
        {
            try
            {
                jTokenTree.UpdateSelected(jsonValueTextBox.Text);

                SetJsonStatus("Json format validated.", false);
            }
            catch (JsonReaderException exception)
            {
                SetJsonStatus(
                    $"INVALID Json format at (line {exception.LineNumber}, position {exception.LinePosition})",
                    true);
            }
            catch
            {
                SetJsonStatus("INVALID Json format", true);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void jsonTreeViewSplitContainer_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void jsonValueTextBox_TextChanged_1(object sender, EventArgs e)
        {

        }

    }
}

using YouTubeDownDlp.Components;
using YouTubeDownDlp.Components.ArgComponents;

namespace YouTubeDownDlp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Url_textBox.Select();
            Log_richTextBox.HideSelection = false;
            IsFormTheStartLivedown_checkBox.Visible = false;
            CookieUseBrowser_comboBox.Visible = false;
            CookieUseBrowser_comboBox.SelectedIndex = 0;
            Global_Variable.AppPath = Components.Components.GetAppPath();

            //もし出力先が保存されてたらそれを代入する
            if (Properties.Settings.Default.OutputFolderPath.Length != 0)
            {
                Global_Variable.Outputfolderpath = Properties.Settings.Default.OutputFolderPath;
                OutputFolder_textBox.Text = Global_Variable.Outputfolderpath;
            }
        }

        /// <summary>
        /// 変換実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ConvertRun_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Global_Variable.Outputfolderpath))
            {
                _ = MessageBox.Show("フォルダを選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Url_textBox.Text))
            {
                _ = MessageBox.Show("URLを入力してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //コマンドを生成するためのデータをまとめる　それを実行時に受け渡す
            ArgData argdata = new()
            {
                CookieBrowserName = CookieUseBrowser_comboBox.Text,
                IsCookie = IsUseCookie_checkBox.Checked,
                IsformStartLive = IsFormTheStartLivedown_checkBox.Checked,
                mode = GetMode(),
                OutputPath = Global_Variable.Outputfolderpath,
                Url = Url_textBox.Text
            };

            if (IsLive_checkBox.Checked)
            {
                DialogResult isok = MessageBox.Show("生放送ダウンロード機能が有効です。録画を停止する際はコマンドプロンプトを選択した状態で<Ctrl + C>で終了できます。" +
                    "本当に実行しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                if (isok == DialogResult.No)
                {
                    return;
                }

                Converts.RunPopupConvert(argdata);

                Url_textBox.Text = "";
                return;
            }

            Log_richTextBox.Text = "";
            main_Control.SelectedTab = logPage;
            Global_Variable.IsConverting = true;

            //変換実行
            await Converts.RunConvertTask(this, Log_richTextBox, argdata);

            Global_Variable.IsConverting = false;
            Url_textBox.Text = "";
            _ = MessageBox.Show("処理が終了しました", "お知らせぇ！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            main_Control.SelectedTab = MainPage;
        }

        private ArgData.Mode GetMode()
        {
            if (mp3_radioButton.Checked)
            {
                return ArgData.Mode.MP3;
            }

            if (mp4_radioButton.Checked)
            {
                return ArgData.Mode.MP4;
            }

            return wav_radioButton.Checked ? ArgData.Mode.WAV : ArgData.Mode.MP3;
        }

        private void OutputFolderSelect_button_Click(object sender, EventArgs e)
        {
            string path = FolderBrowserComponents.FolderSelect();
            if (path.Equals("none", StringComparison.Ordinal))
            {
                return;
            }

            Global_Variable.Outputfolderpath = path;
            OutputFolder_textBox.Text = path;
        }

        private void main_Control_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (Global_Variable.IsConverting)
            {
                e.Cancel = true;
            }
        }

        private void dlpUpdate_toolStripButton_Click(object sender, EventArgs e)
        {
            using UpdateForm updateform = new();
            _ = updateform.ShowDialog();
            updateform.Dispose();
        }

        private void outputpathsave_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutputFolderPath = Global_Variable.Outputfolderpath;
            Properties.Settings.Default.Save();
            _ = MessageBox.Show("保存しました", "お知らせぇ！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void outputpathReset_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            _ = MessageBox.Show("リセットしました。次回起動時から適用されます", "お知らせぇ！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void IsFormTheStartLivedown_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLive_checkBox.Checked)
            {
                IsFormTheStartLivedown_checkBox.Checked = false;
            }
        }

        /// <summary>
        /// 生放送をダウンロードするにチェックが入っていないと"初めからダウンロード"を表示しないようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsLive_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLive_checkBox.Checked)
            {
                IsFormTheStartLivedown_checkBox.Visible = true;
            }
            else
            {
                IsFormTheStartLivedown_checkBox.Visible = false;
                IsFormTheStartLivedown_checkBox.Checked = false;
            }
        }

        /// <summary>
        /// クッキーを使うにチェックが入っていないとコンボボックスを表示しないようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsUseCookie_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (IsUseCookie_checkBox.Checked)
            {
                CookieUseBrowser_comboBox.Visible = true;
            }
            else
            {
                CookieUseBrowser_comboBox.SelectedIndex = 0;
                CookieUseBrowser_comboBox.Visible = false;
            }
        }
    }
}
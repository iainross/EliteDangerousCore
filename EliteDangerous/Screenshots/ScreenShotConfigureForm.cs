﻿/*
 * Copyright © 2020 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EliteDangerousCore.ScreenShots
{
    public partial class ScreenShotConfigureForm : ExtendedControls.DraggableForm
    {
        public string InputFolder { get { return textBoxScreenshotsDir.Text; } }
        public string OutputFolder { get { return textBoxOutputDir.Text; } }
        public bool RemoveOriginal { get { return extCheckBoxRemoveOriginal.Checked; } }
        public ScreenShotConverter.InputTypes InputFileExtension { get { return (ScreenShotConverter.InputTypes)comboBoxScanFor.SelectedIndex; } }
        public ScreenShotImageConverter.OutputTypes OutputFileExtension { get { return (ScreenShotImageConverter.OutputTypes)comboBoxOutputAs.SelectedIndex; } }
        public int FileNameFormat { get { return comboBoxFileNameFormat.SelectedIndex; } }
        public int FolderNameFormat { get { return comboBoxSubFolder.SelectedIndex; } }
        public bool KeepMasterConvertedImage { get { return extCheckBoxKeepMasterConvertedImage.Checked; } }
        public ScreenShotImageConverter.CropResizeOptions CropResizeImage1 { get { return (ScreenShotImageConverter.CropResizeOptions)extComboBoxConvert1.SelectedIndex; } }
        public Rectangle CropResizeArea1 { get { return new Rectangle(numericUpDownLeft1.Value, numericUpDownTop1.Value, numericUpDownWidth1.Value, numericUpDownHeight1.Value); } }
        public ScreenShotImageConverter.CropResizeOptions CropResizeImage2 { get { return (ScreenShotImageConverter.CropResizeOptions)extComboBoxConvert2.SelectedIndex; } }
        public Rectangle CropResizeArea2 { get { return new Rectangle(extNumericUpDownLeft2.Value, extNumericUpDownTop2.Value, extNumericUpDownWidth2.Value, extNumericUpDownHeight2.Value); } }
        public bool HighRes {  get { return extCheckBoxHiRes.Checked; } }
        public bool CopyToClipboard { get { return extCheckBoxCopyClip.Checked; } }

        string initialssfolder;

        public ScreenShotConfigureForm()
        {
            InitializeComponent();
            var theme = ExtendedControls.ThemeableFormsInstance.Instance;
            bool winborder = theme.ApplyStd(this);
            panelTop.Visible = panelTop.Enabled = !winborder;
        }

        public void Init(ScreenShotImageConverter cf , string inputfolder, ScreenShotConverter.InputTypes it, string outputfolder)
        {
            comboBoxOutputAs.Items.AddRange(Enum.GetNames(typeof(ScreenShotImageConverter.OutputTypes)));
            comboBoxOutputAs.SelectedIndex = (int)cf.OutputFileExtension;
            comboBoxScanFor.Items.AddRange(Enum.GetNames(typeof(ScreenShotConverter.InputTypes)));
            comboBoxScanFor.SelectedIndex = (int)it;
            initialssfolder = textBoxScreenshotsDir.Text = inputfolder;
            textBoxOutputDir.Text = outputfolder;
            comboBoxSubFolder.Items.AddRange(ScreenShotImageConverter.SubFolderSelections);
            comboBoxSubFolder.SelectedIndex = cf.FolderNameFormat;
            comboBoxFileNameFormat.Items.AddRange(ScreenShotImageConverter.FileNameFormats);
            comboBoxFileNameFormat.SelectedIndex = cf.FileNameFormat;
            string[] opt = new string[] { "Off", "Crop", "Resize" };
            extComboBoxConvert1.Items.AddRange(opt);
            extComboBoxConvert2.Items.AddRange(opt);

            extCheckBoxHiRes.Checked = cf.HighRes;
            extCheckBoxCopyClip.Checked = cf.CopyToClipboard;

            extComboBoxConvert1.SelectedIndex = (int)cf.CropResizeImage1;
            extComboBoxConvert2.SelectedIndex = (int)cf.CropResizeImage2;

            extCheckBoxRemoveOriginal.Checked = cf.RemoveOriginal;
            extCheckBoxKeepMasterConvertedImage.Checked = cf.KeepMasterConvertedImage;

            numericUpDownTop1.Value = cf.CropResizeArea1.Top;
            numericUpDownLeft1.Value = cf.CropResizeArea1.Left;
            numericUpDownWidth1.Value = cf.CropResizeArea1.Width;
            numericUpDownHeight1.Value = cf.CropResizeArea1.Height;

            extNumericUpDownTop2.Value = cf.CropResizeArea2.Top;
            extNumericUpDownLeft2.Value = cf.CropResizeArea2.Left;
            extNumericUpDownWidth2.Value = cf.CropResizeArea2.Width;
            extNumericUpDownHeight2.Value = cf.CropResizeArea2.Height;

            SetNumEnabled();

            extComboBoxConvert1.SelectedIndexChanged += (s, e) => { SetNumEnabled(); };
            extComboBoxConvert2.SelectedIndexChanged += (s, e) => { SetNumEnabled(); };

            textBoxFileNameExample.Text = ScreenShotImageConverter.CreateFileName("Sol", "Earth", "HighResScreenshot_0000.bmp", comboBoxFileNameFormat.SelectedIndex, extCheckBoxHiRes.Checked, DateTime.Now);

            BaseUtils.Translator.Instance.Translate(this);

            label_index.Text = this.Text;
        }

        private void ScreenShotConfigureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void SetNumEnabled()
        {
            numericUpDownTop1.Enabled = numericUpDownLeft1.Enabled = extComboBoxConvert1.SelectedIndex == 1;
            numericUpDownWidth1.Enabled = numericUpDownHeight1.Enabled = extComboBoxConvert1.SelectedIndex > 0;
            extNumericUpDownTop2.Enabled = extNumericUpDownLeft2.Enabled = extComboBoxConvert2.SelectedIndex == 1;
            extNumericUpDownWidth2.Enabled = extNumericUpDownHeight2.Enabled = extComboBoxConvert2.SelectedIndex > 0;
        }

        private void panel_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void captionControl_MouseDown(object sender, MouseEventArgs e)
        {
            OnCaptionMouseDown((Control)sender, e);
        }

        private void captionControl_MouseUp(object sender, MouseEventArgs e)
        {
            OnCaptionMouseUp((Control)sender, e);
        }

        private void buttonChangeEDScreenshot_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select ED screenshot folder";
                dlg.SelectedPath = textBoxScreenshotsDir.Text;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    initialssfolder = textBoxScreenshotsDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void buttonChangeOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = "Select converted screenshot folder";
            dlg.SelectedPath = textBoxOutputDir.Text;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                textBoxOutputDir.Text = dlg.SelectedPath;
            }
        }

        private void textBoxScreenshotsDir_Leave(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxScreenshotsDir.Text))
            {
                ExtendedControls.MessageBoxTheme.Show(this, "Folder specified does not exist");
                textBoxScreenshotsDir.Text = initialssfolder;
            }
            else
                initialssfolder = textBoxScreenshotsDir.Text;
        }

        private void textBoxScreenshotsDir_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBoxScreenshotsDir_Leave(sender, e);
            }
        }

        private void buttonExtOK_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxScreenshotsDir.Text))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                ExtendedControls.MessageBoxTheme.Show(this, "Folder specified for scanning does not exist, correct or cancel");
        }

        private void buttonExtCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void comboBoxFileNameFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxFileNameExample.Text = ScreenShotImageConverter.CreateFileName("Sol", "Earth", "HighResScreenshot_0000.bmp", comboBoxFileNameFormat.SelectedIndex, extCheckBoxHiRes.Checked, DateTime.Now);
        }

        private void extCheckBoxHiRes_CheckedChanged(object sender, EventArgs e)
        {
            textBoxFileNameExample.Text = ScreenShotImageConverter.CreateFileName("Sol", "Earth", "HighResScreenshot_0000.bmp", comboBoxFileNameFormat.SelectedIndex, extCheckBoxHiRes.Checked, DateTime.Now);
        }
    }
}

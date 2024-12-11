using System;
using System.Media;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace BABOisYOU {
    class Initialize {
        // Console window form consts
        private const int SW_MAXIMIZE = 3;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_SIZEBOX = 0x00800000;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const int SW_SHOWMAXIMIZED = 3;

        // F11 Keydown event consts
        private const int VK_F11 = 0x7A;
        private const uint WM_KEYDOWN = 0x100;
        private const uint WHEEL_DELTA = 120;

        // Ctrl Wheel down event consts
        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;
        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private enum Keys : ushort {
            ControlKey = 0x11
        }

        // DLL imports region
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32")]
        public static extern long SetWindowPos(long hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT {
            public uint type;
            public InputUnion u;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public static void SendMouseWheel(int delta) {
            INPUT[] inputs = new INPUT[2];
            // Control key down
            inputs[0] = new INPUT {
                type = INPUT_KEYBOARD,
                u = new InputUnion {
                    ki = new KEYBDINPUT {
                        wVk = (ushort)Keys.ControlKey,
                        dwFlags = KEYEVENTF_KEYDOWN
                    }
                }
            };
            // Mouse wheel event
            inputs[1] = new INPUT {
                type = INPUT_MOUSE,
                u = new InputUnion {
                    mi = new MOUSEINPUT {
                        mouseData = (uint)delta,
                        dwFlags = MOUSEEVENTF_WHEEL
                    }
                }
            };
            // Send inputs
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            
            // Control key up
            inputs[0].u.ki.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // Dll imports region End

        public static void initWindow() {
            Console.Title = "BABOisYOU";

            IntPtr windowHandle = GetConsoleWindow();
            int style = GetWindowLong(windowHandle, GWL_STYLE);

            SetWindowLong(windowHandle, GWL_STYLE, style & ~(WS_CAPTION | WS_SIZEBOX));
            SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, GetSystemMetrics(0), GetSystemMetrics(1),
                SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            ShowWindow(windowHandle, SW_SHOWMAXIMIZED); Thread.Sleep(20);

            while (true) {
                if (Console.LargestWindowWidth > 600 && Console.LargestWindowHeight > 190) break;
                SendMouseWheel(-(int)WHEEL_DELTA);
                Thread.Sleep(20);
            }
            Thread.Sleep(100);

            int tryCount = 0;
            bool trySuccessed = false;
            while (!trySuccessed && tryCount < 3) {
                try {
                    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                    Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                    trySuccessed = true;
                }
                catch {
                    tryCount++;
                    Thread.Sleep(100);
                }
            }
            Console.CursorVisible = false;
        }

        public static void initScreen() {
            for (int i = 5; i < 10; i++) {
                for (int j = 5; j < Constants.CONST_SCREEN_SIZE_X - 5; j++) {
                    Program.backBuffer.setPos(j, i);
                    Program.backBuffer.setPixel("■", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                }
            } // 상

            for (int i = 5; i < Constants.CONST_SCREEN_SIZE_Y - 5; i++) {
                for (int j = 5; j < 11; j++) {
                    Program.backBuffer.setPos(j, i);
                    Program.backBuffer.setPixel("■", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                }
            } // 좌

            for (int i = 5; i < Constants.CONST_SCREEN_SIZE_Y - 5; i++) {
                for (int j = Constants.CONST_SCREEN_SIZE_X - 11; j < Constants.CONST_SCREEN_SIZE_X - 5; j++) {
                    Program.backBuffer.setPos(j, i);
                    Program.backBuffer.setPixel("■", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                }
            } // 우

            for (int i = Constants.CONST_SCREEN_SIZE_Y - 10; i < Constants.CONST_SCREEN_SIZE_Y - 5; i++) {
                for (int j = 5; j < Constants.CONST_SCREEN_SIZE_X - 5; j++) {
                    Program.backBuffer.setPos(j, i);
                    Program.backBuffer.setPixel("■", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                }
            } // 하
        }

        public static void initBGM() {
            string soundFile = "./sfx/bgm.wav";

            using (SoundPlayer player = new SoundPlayer(soundFile)) {
               player.PlayLooping();
            }
        }
        public static void Ending() {
            Random rand = new Random((int)DateTime.Now.Ticks);
            for (int posY = 12; posY < Constants.CONST_SCREEN_SIZE_Y - 12; posY++) {

                for (int posYLine = 12; posYLine <= posY; posYLine++) {
                    for (int posX = 12; posX < Constants.CONST_SCREEN_SIZE_X - 12; posX++) {
                        if (Program.backBuffer.sBuffer[posYLine, posX].pixel.Equals('■')) {
                            int tempX = posX - 6 + rand.Next(5, 10);
                            if (posYLine < 14 || posX - 6 + rand.Next(5, 10) > Constants.CONST_SCREEN_SIZE_X - 20)
                                Program.backBuffer.sBuffer[posYLine - 1, tempX].Set(' ');
                            else
                                Program.backBuffer.sBuffer[posYLine - 1, tempX].Set(
                                    Program.backBuffer.sBuffer[posYLine, posX].pixel,
                                    Program.backBuffer.sBuffer[posYLine, posX].fg,
                                    Program.backBuffer.sBuffer[posYLine, posX].bg
                                    );
                            Program.backBuffer.sBuffer[posYLine, posX - 1].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                            Program.backBuffer.sBuffer[posYLine, posX].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                            Program.backBuffer.sBuffer[posYLine + 1, posX].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                            Program.backBuffer.sBuffer[posYLine - 1, posX].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                            Program.backBuffer.sBuffer[posYLine, posX + 1].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                            Program.backBuffer.sBuffer[posYLine, posX - 2].Set(' ', ConsoleColor.Black, ConsoleColor.Black);
                        }
                    }
                }
                if (posY - 8 > 12) {
                    for (int k = posY - 8; k < posY; k++) {
                        Console.SetCursorPosition(12, k);
                        string t = "";
                        for (int posX = 15; posX < (Constants.CONST_SCREEN_SIZE_X) / 2; posX++) {
                            t += "  ";
                        }
                        Console.WriteLine(t);
                    }
                }
                Program.frontBuffer.Print(Program.backBuffer, 0);
            }
            for (int posY = 12; posY < Constants.CONST_SCREEN_SIZE_Y - 12; posY++) {
                
                for (int posYLine = posY; posYLine <= posY; posYLine++) {
                    Console.SetCursorPosition(12, posYLine);
                    string t = "";
                    for (int posX = 12; posX < (Constants.CONST_SCREEN_SIZE_X) / 2; posX++) {
                        t += "  ";
                    }
                    Console.WriteLine(t);
                    Thread.Sleep(5);
                }
            }

            Map map = new Map();
            List<SpriteBlock> blocks = map.LoadMap(100);
            Program.backBuffer.initBuffer();
            foreach (var block in blocks) {
                block.Print();
                Program.frontBuffer.Print(Program.backBuffer, 1);
                    Thread.Sleep(1000);
            }
            Thread.Sleep(1500);
            for (int i = 0; i < 3; i++) {
                foreach (var block in blocks) {
                    if (block.posX < 14) block.blockRight();
                    else if (block.posX > 14) block.blockLeft();
                    block.Print();
                }
                Program.frontBuffer.Print(Program.backBuffer, 0);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            Program.backBuffer.initBuffer();
            Program.frontBuffer.Print(Program.backBuffer, 1);
            Program.level = 0;
            Program.isDone = false;
            Thread.Sleep(1000);
        }
    }
}

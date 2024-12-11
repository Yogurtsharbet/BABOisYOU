using System;
using System.Threading;

namespace BABOisYOU {
    class ScreenChar {
        public char pixel { get; private set; }
        public ConsoleColor fg { get; private set; }
        public ConsoleColor bg { get; private set; }

        public ScreenChar() {
            pixel = ' ';
            fg = ConsoleColor.Gray;
            bg = ConsoleColor.Black;
        }
        public ScreenChar(ScreenChar t) {
            pixel = t.pixel;
            fg = t.fg;
            bg = t.bg;
        }
        public void Set(char pixel) {
            this.pixel = pixel;
        }
        public void Set(char pixel, ConsoleColor fg) {
            this.pixel = pixel;
            this.fg = fg;
        }
        public void Set(char pixel, ConsoleColor fg, ConsoleColor bg) {
            this.pixel = pixel;
            this.fg = fg;
            this.bg = bg;
        }
    }

    class ScreenBuffer {
        public  ScreenChar[,] sBuffer { get; private set; } = new ScreenChar[Constants.CONST_SCREEN_SIZE_Y, Constants.CONST_SCREEN_SIZE_X];
        public int posX { get; private set; }
        public int posY { get; private set; }
        
        public void initBuffer() {
            for (int i = 0; i < Constants.CONST_SCREEN_SIZE_Y; i++) {
                for (int j = 0; j < Constants.CONST_SCREEN_SIZE_X; j++) {
                    sBuffer[i, j] = new ScreenChar();
                    sBuffer[i, j].Set(' ');
                }
            }
            Initialize.initScreen();
        }
        
        public void setPixel(char pixel) {
            if (!(posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
             posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X))
                sBuffer[posY, posX].Set(pixel, ConsoleColor.Black, ConsoleColor.Black);
        }
        public void setPixel(char pixel, ConsoleColor fg) {
            if (!(posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
             posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X))
                sBuffer[posY, posX].Set(pixel, fg, ConsoleColor.Black);
        }
        public void setPixel(char pixel, ConsoleColor fg, ConsoleColor bg) {
            if (!(posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
             posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X))
                sBuffer[posY, posX].Set(pixel, fg, bg);
        }
        public void setPixel(string pixel) {
            char[] temp = pixel.ToCharArray();
            for (int i = 0; i < temp.Length; i++) {
                if (posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
                             posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X) continue;
                sBuffer[posY, posX].Set(temp[i], ConsoleColor.DarkGray, ConsoleColor.Black);
                posX++;
                if (posX > Constants.CONST_SCREEN_SIZE_X) break;
            }
        }
        public void setPixel(string pixel, ConsoleColor fg) {
            char[] temp = pixel.ToCharArray();
            for (int i = 0; i < temp.Length; i++) {
                if (posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
                             posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X) continue;
                sBuffer[posY, posX].Set(temp[i], fg, ConsoleColor.Black);
                posX++;
                if (posX > Constants.CONST_SCREEN_SIZE_X) break;
            }
        }
        public void setPixel(string pixel, ConsoleColor fg, ConsoleColor bg) {
            char[] temp = pixel.ToCharArray();
            for (int i = 0; i < temp.Length; i++) {
                if (posY < 0 || posY >= Constants.CONST_SCREEN_SIZE_Y ||
                    posX < 0 || posX >= Constants.CONST_SCREEN_SIZE_X) continue;
                sBuffer[posY, posX].Set(temp[i], fg, bg);
                posX++;
            }
        }
        public void setPos(int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
        }

        public void Print(ScreenBuffer target, int range = 1) {
            Thread disableInputThread = new Thread(disableInput_thread);
            Console.SetCursorPosition(0, 0);
            disableInputThread.Start();

            for (int cellY = 0; cellY < Constants.CONST_SCREEN_SIZE_Y; cellY++) {
                for (int cellX = 0; cellX < Constants.CONST_SCREEN_SIZE_X; cellX++) {

                    if (this.sBuffer[cellY, cellX].pixel != target.sBuffer[cellY, cellX].pixel ||
                        this.sBuffer[cellY, cellX].fg != target.sBuffer[cellY, cellX].fg ||
                        this.sBuffer[cellY, cellX].bg != target.sBuffer[cellY, cellX].bg) {

                        for (int recellY = cellY - range > 0 ? cellY - range : 0; recellY <= cellY + range; recellY++) {
                            for (int recellX = cellX - range > 0 ? cellX - range : 0; recellX <= cellX + range; recellX++) {
                                if (recellY < 0) recellY = 0; if (recellY >= Constants.CONST_SCREEN_SIZE_Y) continue;
                                if (recellX < 0) recellX = 0; if (recellX >= Constants.CONST_SCREEN_SIZE_X) continue;

                                Console.SetCursorPosition(recellX, recellY);
                                Console.ForegroundColor = target.sBuffer[recellY, recellX].fg;
                                Console.BackgroundColor = target.sBuffer[recellY, recellX].bg;
                                Console.Write(target.sBuffer[recellY, recellX].pixel);
                                Console.ResetColor();
                                this.sBuffer[recellY, recellX] = new ScreenChar(target.sBuffer[recellY, recellX]);
                            }
                        }
                    }
                }
            }
            disableInputThread.Abort();
        }

        public void disableInput_thread() {
            while (true) {
                while (Console.KeyAvailable) 
                    Console.ReadKey(true);
                Thread.Sleep(50);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BABOisYOU {
    class Program {
        public static ScreenBuffer frontBuffer = new ScreenBuffer();
        public static ScreenBuffer backBuffer = new ScreenBuffer();
        public static int level = 0;      // MODIFY LEVEL FOR DEBUG #####################
        public static bool isWin = false;
        public static bool isExit = false;
        public static bool isDone = false;
        public static string sound = "";

        [DllImport("winmm.dll")]
        public static extern int mciSendString(string command, string buffer, int bufferSize, IntPtr hwndCallback);
        public static void playSound(string sound) {
            mciSendString("close MySound", null, 0, IntPtr.Zero);

            string command = $"open \"./sfx/{sound}\" type waveaudio alias MySound";
            mciSendString(command, null, 0, IntPtr.Zero);

            command = "play MySound";
            mciSendString(command, null, 0, IntPtr.Zero);
        }

        static void Main(string[] args) {
            backBuffer.initBuffer();
            frontBuffer.initBuffer();
            Initialize.initWindow();
            Random rand = new Random((int)DateTime.Now.Ticks);
            Thread bgmThread = new Thread(() => Initialize.initBGM());
            bgmThread.Start();

            while (true) {
                if (level == 1) {
                    Console.Clear();
                    frontBuffer.initBuffer();
                }
                if (level == 12) level = 0;
                backBuffer.initBuffer();
                Map map = new Map();
                List<List<SpriteBlock>> undoList = new List<List<SpriteBlock>>();
                List<SpriteBlock> blocks = map.LoadMap(level);
                List<SpriteBlock> tempCopy = new List<SpriteBlock>();
                frontBuffer.Print(backBuffer, 0);

                foreach (var each in blocks) {
                    if (each.type.Equals(S_TYPE.type_word_IS))
                        each.checkIS(blocks);
                }
                blocks.ForEach(each => each.Print());
                frontBuffer.Print(backBuffer, 0);

                if (level == 0) 
                    map.PrintLogo();

                while (true) {
                    isWin = false;
                    if (Console.KeyAvailable) {
                        List<SpriteBlock> ControlBlock = SpriteControl.getBlockofType(blocks, P_TYPE._isControl);
                        tempCopy = new List<SpriteBlock>();
                        foreach (var block in blocks) {
                            tempCopy.Add(new SpriteBlock(block));
                        }
                        undoList.Add(new List<SpriteBlock>(tempCopy));
                        while (undoList.Count > 20) {
                            undoList.RemoveAt(0);
                        }

                        sound = "";
                        switch (Console.ReadKey(true).Key) {
                            case ConsoleKey.UpArrow:
                                ControlBlock.ForEach(each => each.blockUp().checkState(blocks));
                                if (level == 0)
                                    foreach (var block in ControlBlock) {
                                        if (block.posY < 10) block.blockDown().checkState(blocks);
                                    }
                                break;
                            case ConsoleKey.DownArrow:
                                ControlBlock.ForEach(each => each.blockDown().checkState(blocks)); break;
                            case ConsoleKey.LeftArrow:
                                ControlBlock.ForEach(each => each.blockLeft().checkState(blocks)); break;
                            case ConsoleKey.RightArrow:
                                ControlBlock.ForEach(each => each.blockRight().checkState(blocks)); break;
                            case ConsoleKey.Z:
                                if (undoList.Count > 1) {
                                    undoList.RemoveAt(undoList.Count - 1);
                                    foreach (var block in blocks) {
                                        block.setXY(100, 100);
                                        block.Print();
                                    }
                                    blocks = new List<SpriteBlock>();
                                    foreach (var block in undoList[undoList.Count - 1]) {
                                        blocks.Add(new SpriteBlock(block));
                                    }
                                    
                                    blocks.ForEach(each => each.Print());
                                    undoList.RemoveAt(undoList.Count - 1);
                                }
                                break;
                            case ConsoleKey.R:
                                backBuffer.initBuffer(); blocks = map.LoadMap(level);
                                ControlBlock = SpriteControl.getBlockofType(blocks, P_TYPE._isControl);
                                undoList.Clear();
                                break;
                                // FOR DEBUGGING OPTIONS #@!!@@!!@#!!@!@#!#@!@#@!#!@#
                                /*
                            case ConsoleKey.E:
                                isDone = true;
                                break;
                            case ConsoleKey.Add: level++; isWin = true; break;
                                */
                        }
                        if (ControlBlock.Count > 0)
                            sound = "move" + rand.Next(1, 3) + ".wav";

                        for (int i = 0; i < blocks.Count; i++) {
                            int tempX = blocks[i].posX, tempY = blocks[i].posY, tempDirection = blocks[i].nowDirection;
                            SpriteBlock nowOverlaped = blocks[i].checkOverlap(blocks);
                            SpriteBlock pastOverlaped;
                            if (blocks[i]._isControl || blocks[i]._isPush)
                                pastOverlaped = blocks[i].moveDirection((blocks[i].nowDirection + 2) % 4).checkOverlap(blocks);
                            else
                                pastOverlaped = null;
                            blocks[i].setXY(tempX, tempY).setDirection(tempDirection);

                            if (pastOverlaped != null) {
                                if (pastOverlaped._isSlip) {
                                    blocks[i].setXY(blocks[i].pastX, blocks[i].pastY).moveDirection(blocks[i].pastDirection).checkState(blocks);
                                    }
                            }
                            if (nowOverlaped != null)
                                if (nowOverlaped._isSlip) {
                                    blocks[i].setPastDirection(blocks[i].nowDirection);
                                    if (blocks[i].moveDirection((blocks[i].nowDirection + 2) % 4).checkOverlap(blocks) != null)
                                        if (blocks[i].checkOverlap(blocks)._isSlip)
                                            if (blocks[i].posX == blocks[i].pastX &&
                                            blocks[i].posY == blocks[i].pastY &&
                                            blocks[i]._isWord)
                                            blocks[i].moveDirection(blocks[i].pastDirection).checkState(blocks);
                                    blocks[i].moveDirection(blocks[i].pastDirection).checkState(blocks);

                                }

                            if (blocks[i]._isMove) {    // checkMove
                                switch (blocks[i].nowDirection) {
                                    case 0: blocks[i].blockUp(); break;
                                    case 1: blocks[i].blockRight(); break;
                                    case 2: blocks[i].blockDown(); break;
                                    case 3: blocks[i].blockLeft(); break;
                                }
                                if(!blocks[i].checkState(blocks) ||
                                    (blocks[i].posX == blocks[i].pastX &&
                                    blocks[i].posY == blocks[i].pastY)) {
                                    blocks[i].setDirection((blocks[i].nowDirection + 2) % 4);
                                }
                            }

                            if (nowOverlaped != null) { // checkLose
                                if (nowOverlaped._isLose &&
                                    !blocks[i]._isWord &&
                                    !blocks[i]._isMove &&
                                    (blocks[i].type != S_TYPE.type_sprite_KEKE || blocks[i]._isControl)) {
                                    var tempRemove = blocks[i];
                                    tempRemove.Print();
                                    ControlBlock.Remove(tempRemove);
                                    blocks[i] = null;
                                    blocks.RemoveAt(i--);
                                    sound = "dead.wav";
                                }
                            }
                        }
                        
                        blocks.ForEach(each => each.clearAll());    // checkIS (Property 부여)
                        for( int i = 0; i < blocks.Count; i++) { 
                            if (blocks[i].type.Equals(S_TYPE.type_word_IS))
                                blocks[i].checkIS(blocks);
                        }

                        playSound(sound);
                        blocks.ForEach(each => each.Print());
                        blocks.ForEach(each => each.Print());
                        SpriteControl.getBlockofType(blocks, P_TYPE._isWord).ForEach(each => each.Print());
                        ControlBlock.ForEach(each => each.Print());
                        frontBuffer.Print(backBuffer);   // MODIFY AREA FOR DEBUG###############
                        
                        foreach (var block in ControlBlock) {   // checkWin
                            if (block.checkWIN(blocks) ||
                                (block._isControl && block._isWin)) {
                                level++;
                                isWin = true;
                            }
                        }
                        if (isWin) {
                            sound = "win.wav";
                            playSound(sound); break;
                        }
                        if (isDone) {
                            Initialize.Ending();
                            break;
                        }
                        if (isExit) Environment.Exit(0);
                    }
                }
            }
        }
    }
}

//!@#@!##@!#@!#!@#@!#@#@!#!##@!#!@#@!#@#@!#!##@!#!@#@!#@#@!#!##@!#!

//[[[[[[[[    RELEASE 할 때 SOUND 주석 해제 할 것    ]]]]]]]]]

//!@@#!@!!!#!!#@@!@!!#@!#!#@!!!#!!#@@!@!!#@!#!#@!!!#!!#@@!@!!#@!#!#

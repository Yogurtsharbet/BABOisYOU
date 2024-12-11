using System;
using System.Collections.Generic;
using static System.ConsoleColor;

namespace BABOisYOU {
    class SpriteData {
        public char[,] pixel;
        public ConsoleColor[,] fg;
        public ConsoleColor[,] bg;

        public SpriteData(char[,] pixel, ConsoleColor[,] fg) {
            this.pixel = pixel;
            this.fg = fg;
            this.bg = fg;
        }
        public SpriteData(char[,] pixel, ConsoleColor[,] fg, ConsoleColor[,] bg) {
            this.pixel = pixel;
            this.fg = fg;
            this.bg = bg;
        }
    }
    class SpriteControl {
        public int posX { get; private set; }
        public int posY { get; private set; }
        public int pastX { get; private set; }
        public int pastY { get; private set; }
        public int nowDirection { get; private set; }
        public int pastDirection { get; private set; }
        public S_TYPE type { get; private set; }

        public SpriteControl() {
            posX = 1000;
            posY = 1000;
            pastX = 1000;
            pastY = 1000;
        }
        public SpriteControl(SpriteControl other) {
            this.posX = other.posX;
            this.posY = other.posY;
            this.pastX = other.pastX;
            this.pastY = other.pastY;
            this.nowDirection = other.nowDirection;
            this.pastDirection = other.pastDirection;
            this.type = other.type;
        }

        public SpriteControl setType(S_TYPE type) {
            this.type = type;
            return this;
        }
        public SpriteControl setXY(int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
            return this;
        }
        public SpriteControl setDirection(int direction) {
            this.nowDirection = direction;
            return this;
        }
        public SpriteControl setPastDirection(int direction) {
            this.pastDirection = direction;
            return this;
        }
        public SpriteControl calculateDirection() {
            if (this.posY < this.pastY) this.nowDirection = 0;
            else if (this.posX > this.pastX) this.nowDirection = 1;
            else if (this.posY > this.pastY) this.nowDirection = 2;
            else if (this.posX < this.pastX) this.nowDirection = 3;
            return this;
        }
        public SpriteControl moveDirection() {
            switch (this.nowDirection) {
                case 0: this.blockUp(); break;
                case 1: this.blockRight(); break;
                case 2: this.blockDown(); break;
                case 3: this.blockLeft(); break;
            }
            return this;
        }
        public SpriteControl moveDirection(int direction) {
            switch (direction) {
                case 0: this.blockUp(); break;
                case 1: this.blockRight(); break;
                case 2: this.blockDown(); break;
                case 3: this.blockLeft(); break;
            }
            return this;
        }
        public SpriteControl blockUp() {
            if (posY - 1 > 0) {
                posY--;
                nowDirection = 0;
            }
            return this;
        }
        public SpriteControl blockDown() {
            if (posY + 1 <= Constants.CONST_TABLE_CELL_Y) {
                posY++;
                nowDirection = 2;
            }
            return this;
        }
        public SpriteControl blockLeft() {
            if (posX - 1 > 0) {
                posX--;
                nowDirection = 3;
            }
            return this;
        }
        public SpriteControl blockRight() {
            if (posX + 1 <= Constants.CONST_TABLE_CELL_X) {
                posX++;
                nowDirection = 1;
            }
            return this;
        }

        public static List<SpriteBlock> getBlockofType(List<SpriteBlock> blocks, S_TYPE type) {
            List<SpriteBlock> temp = new List<SpriteBlock>();
            foreach (var block in blocks) {
                if (block.type == type)
                    temp.Add(block);
            }
            return temp;
        }
        public static List<SpriteBlock> getBlockofType(List<SpriteBlock> blocks, P_TYPE type) {
            List<SpriteBlock> temp = new List<SpriteBlock>();
            foreach (var block in blocks) {
                if (SpriteBlock.propertyMap[type](block)) {
                    temp.Add(block);
                }
            }
            return temp;
        }

        public bool checkState(List<SpriteBlock> blocks) {

            if (!checkPush(checkOverlap(blocks), blocks)) return false;
            if (!checkStop(checkOverlap(blocks))) return false;

            return true;
        }
        public SpriteBlock checkOverlap(List<SpriteBlock> blocks) {
            for (int i = blocks.Count - 1; i >= 0; i--) {
                if (blocks[i].Equals(this)) continue;
                if (this.posX == blocks[i].posX && this.posY == blocks[i].posY)
                    return blocks[i];
            }
            return null;
        }

        public bool checkPush(SpriteBlock block, List<SpriteBlock> blocks) {
            if (block == null) return true;
            if (block._isPush) {
                if (posX > pastX) {
                    block.posX++;
                    if (block.posX > Constants.CONST_TABLE_CELL_X) {
                        block.posX--;
                        this.posX--;
                        return false;
                    }
                    if (!block.checkState(blocks)) {
                        this.posX--;
                        return false;
                    }
                } // 우 충돌
                else if (posX < pastX) {
                    block.posX--;
                    if (block.posX < 1) {
                        block.posX++;
                        this.posX++;
                        return false;
                    }
                    if (!block.checkState(blocks)) {
                        this.posX++;
                        return false;
                    }
                } // 좌 충돌
                else if (posY > pastY) {
                    block.posY++;
                    if (block.posY > Constants.CONST_TABLE_CELL_Y) {
                        block.posY--;
                        this.posY--;
                        return false;
                    }
                    if (!block.checkState(blocks)) {
                        this.posY--;
                        return false;
                    }
                } // 하 충돌
                else if (posY < pastY) {
                    block.posY--;
                    if (block.posY < 1 ||
                        (Program.level == 0 && block.posY < 10)) {
                        block.posY++;
                        this.posY++;
                        return false;
                    }
                    if (!block.checkState(blocks)) {
                        this.posY++;
                        return false;
                    }
                } // 상 충돌
                block.calculateDirection();
                block.Print();
                return true;
            }
            return checkStop(block);
        }

        public bool checkStop(SpriteBlock block) {
            if (block == null) return true;
            if (block._isStop) {
                this.posX = pastX;
                this.posY = pastY;
                return false;
            }
            return true;
        }

        public bool checkWIN(List<SpriteBlock> blocks) {
            if (checkOverlap(blocks) == null) return false;
            return checkOverlap(blocks)._isWin;
        }

        public void checkIS(List<SpriteBlock> blocks) {
            List<SpriteBlock> blocksCopy = new List<SpriteBlock>(blocks.ToArray());
            bool empty = false;
            SpriteBlock emptyA = new SpriteBlock(S_TYPE.type_sprite_EMPTY), emptyB = new SpriteBlock(S_TYPE.type_sprite_EMPTY);
            foreach (var blockA in blocksCopy) {
                foreach (var blockB in blocksCopy) {
                    if ((this.posX == blockA.posX && this.posY - 1 == blockA.posY &&
                        this.posX == blockB.posX && this.posY + 1 == blockB.posY) ||
                        (this.posX - 1 == blockA.posX && this.posY == blockA.posY &&
                        this.posX + 1 == blockB.posX && this.posY == blockB.posY)) {
                        S_TYPE tempType = S_TYPE.type_sprite;
                        switch (blockA.type) {
                            //************************************//
                            //**               NOTE             **//
                            //**     sprite가 추가될 경우 반드시  **//
                            //**        여기에도 추가할 것        **//
                            //************************************//
                            case S_TYPE.type_word_BABA: tempType = S_TYPE.type_sprite_BABA_R; break;
                            case S_TYPE.type_word_FLAG: tempType = S_TYPE.type_sprite_FLAG; break;
                            case S_TYPE.type_word_WALL: tempType = S_TYPE.type_sprite_WALL; break;
                            case S_TYPE.type_word_ROCK: tempType = S_TYPE.type_sprite_ROCK; break;
                            case S_TYPE.type_word_LAVA: tempType = S_TYPE.type_sprite_LAVA; break;
                            case S_TYPE.type_word_BONE: tempType = S_TYPE.type_sprite_BONE; break;
                            case S_TYPE.type_word_KEKE: tempType = S_TYPE.type_sprite_KEKE; break;
                            case S_TYPE.type_word_GRASS: tempType = S_TYPE.type_sprite_GRASS; break;
                            case S_TYPE.type_word_ICE: tempType = S_TYPE.type_sprite_ICE; break;
                            case S_TYPE.type_word_LOVE: tempType = S_TYPE.type_sprite_LOVE; break;
                            case S_TYPE.type_word_EMPTY: tempType = S_TYPE.type_sprite_EMPTY; break;
                        }
                        switch (blockB.type) {
                            case S_TYPE.type_word_WIN:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsWIN());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsWIN());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsWIN());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsWIN());
                                }
                                else
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsWIN());
                                break;
                            case S_TYPE.type_word_STOP:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsSTOP());
                                }
                                else
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsSTOP());
                                break;
                            case S_TYPE.type_word_YOU:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsCONTROL());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsCONTROL());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsCONTROL());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsCONTROL());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsSTOP());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsSTOP());
                                }
                                else {
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsCONTROL());
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsSTOP());
                                }
                                break;
                            case S_TYPE.type_word_PUSH:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsPUSH());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsPUSH());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsPUSH());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsPUSH());
                                }
                                else
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsPUSH());
                                break;
                            case S_TYPE.type_word_KILL:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsLOSE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsLOSE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsLOSE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsLOSE());
                                }
                                else
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsLOSE());
                                break;
                            case S_TYPE.type_word_MOVE:
                                if (tempType >= S_TYPE.type_sprite_BABA_R &&
                                    tempType <= S_TYPE.type_sprite_BABA_D) {
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                                    each => each.setIsMOVE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                                    each => each.setIsMOVE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                                    each => each.setIsMOVE());
                                    getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                                    each => each.setIsMOVE());
                                }
                                else
                                    getBlockofType(blocks, tempType).ForEach(
                                        each => each.setIsMOVE());
                                break;
                            case S_TYPE.type_word_SLIP:
                                getBlockofType(blocks, tempType).ForEach(
                                    each => each.setIsSLIP());
                                break;
                            case S_TYPE.type_word_PLAY:
                                Program.level++;
                                Program.isWin = true;
                                break;
                            case S_TYPE.type_word_EXIT:
                                Program.isExit = true;
                                break;
                            case S_TYPE.type_word_DONE:
                                if (blockA.type == S_TYPE.type_word_GAME)
                                    Program.isDone = true;
                                break;
                            case S_TYPE.type_word_LOSE:
                                List<SpriteBlock> tempList = new List<SpriteBlock>();
                                if (tempType.Equals(S_TYPE.type_sprite_BABA_R)) {
                                    tempList.AddRange(getBlockofType(blocks, S_TYPE.type_sprite_BABA_R));
                                    tempList.AddRange(getBlockofType(blocks, S_TYPE.type_sprite_BABA_L));
                                    tempList.AddRange(getBlockofType(blocks, S_TYPE.type_sprite_BABA_U));
                                    tempList.AddRange(getBlockofType(blocks, S_TYPE.type_sprite_BABA_D));
                                }
                                else
                                    tempList = getBlockofType(blocks, tempType);
                                foreach (var block in tempList) {
                                    if (block._isControl) {
                                        block.setType(S_TYPE.type_sprite_EMPTY);
                                        block.setXY(30, 30);
                                        block.clearAll();
                                    }
                                }
                                break;
                        }
                        if (blockA.type > S_TYPE.type_word && blockA.type < S_TYPE.type_word_IS &&
                            blockB.type > S_TYPE.type_word && blockB.type < S_TYPE.type_word_IS) {
                            empty = true;
                            emptyA = blockA;
                            emptyB = blockB;
                        }
                        if (blockA.type.Equals(S_TYPE.type_word_EMPTY) && blockB.type.Equals(S_TYPE.type_word_WIN)) {
                            Program.level++;
                            Program.isWin = true;
                        }
                    }
                }
            }
            if (empty)
                wordToSprite(emptyA, emptyB, blocks);
        }
        public void wordToSprite(SpriteBlock blockA, SpriteBlock blockB, List<SpriteBlock> blocks) {
            if (blockA.type == S_TYPE.type_word_EMPTY) {
                bool[,] checkPos = new bool[Constants.CONST_TABLE_CELL_Y + 1, Constants.CONST_TABLE_CELL_X + 1];
                foreach (var block in blocks)
                    checkPos[block.posY, block.posX] = true;
                for (int i = 1; i <= Constants.CONST_TABLE_CELL_Y; i++) {
                    for (int j = 1; j <= Constants.CONST_TABLE_CELL_X; j++) {
                        if (!checkPos[i, j]) {
                            SpriteBlock temp = new SpriteBlock(S_TYPE.type_sprite_EMPTY);
                            temp.setXY(j, i);
                            blocks.Add(temp);
                        }
                    }
                }
            }
        
            if (blockA.type.Equals(S_TYPE.type_word_BABA)) {
                getBlockofType(blocks, S_TYPE.type_sprite_BABA_R).ForEach(
                    each => each.setType((S_TYPE)(blockB.type - S_TYPE.type_word)));
                getBlockofType(blocks, S_TYPE.type_sprite_BABA_L).ForEach(
                    each => each.setType((S_TYPE)(blockB.type - S_TYPE.type_word)));
                getBlockofType(blocks, S_TYPE.type_sprite_BABA_U).ForEach(
                    each => each.setType((S_TYPE)(blockB.type - S_TYPE.type_word)));
                getBlockofType(blocks, S_TYPE.type_sprite_BABA_D).ForEach(
                    each => each.setType((S_TYPE)(blockB.type - S_TYPE.type_word)));
            }
            else
                foreach (var block in getBlockofType(blocks, (S_TYPE)(blockA.type - S_TYPE.type_word))) {
                    block.setType((S_TYPE)(blockB.type - S_TYPE.type_word));
                    block.setIsTile();
                }
        }
        
    
    
        public SpriteControl Print() {
            for (int i = 0; i < 11 && i + pastY * 11 + 3 < Constants.CONST_SCREEN_SIZE_Y; i++) {
                for (int j = 0; j < 22 && j + pastX * 22 < Constants.CONST_SCREEN_SIZE_X; j++) {
                    Program.backBuffer.setPos(j + pastX * 22, i + pastY * 11 + 3);
                    Program.backBuffer.setPixel(' ');
                }
            }
            for (int i = 0; i < 11 && i + posY * 11 + 3 < Constants.CONST_SCREEN_SIZE_Y; i++) {
                for (int j = 0; j < 11 && j + posX * 22 < Constants.CONST_SCREEN_SIZE_X; j++) {
                    Program.backBuffer.setPos(j * 2 + posX * 22, i + posY * 11 + 3);
                    S_TYPE tempType = S_TYPE.type_sprite_BABA_R;
                    if (this.type >= S_TYPE.type_sprite_BABA_R &&
                        this.type <= S_TYPE.type_sprite_BABA_D) {
                        switch (this.nowDirection) {
                            case 0: tempType = S_TYPE.type_sprite_BABA_U; break;
                            case 1: tempType = S_TYPE.type_sprite_BABA_R; break;
                            case 2: tempType = S_TYPE.type_sprite_BABA_D; break;
                            case 3: tempType = S_TYPE.type_sprite_BABA_L; break;
                        }
                    }
                    else tempType = this.type;
                    Program.backBuffer.setPixel(
                        sprite_Type[tempType].pixel[i, j] == ' ' ?
                        Program.backBuffer.sBuffer[posY, posX].pixel :
                        sprite_Type[tempType].pixel[i, j],
                        sprite_Type[tempType].fg[i, j],
                        sprite_Type[tempType].bg[i, j]);
                }
            }
            pastX = posX;
            pastY = posY;
            return this;
        }

        // BLOCK SPRITE DATA 

        public static SpriteData sprite_WALL = new SpriteData(
            new char[,]{
            { ' ','■','■','■','■','■','■','■','■','■',' ' },
            { '■',' ',' ',' ',' ',' ',' ',' ',' ',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ','■','■','■','■','■','■','■',' ','■' },
            { '■',' ',' ',' ',' ',' ',' ',' ',' ',' ','■' },
            { ' ','■','■','■','■','■','■','■','■','■',' ' }
            },
            new ConsoleColor[,]{
            { Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black },
            { DarkGray, Black, Black, Black, Black, Black, Black, Black, Black,Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black, DarkGray },
            { DarkGray, Black, Black, Black, Black, Black, Black, Black, Black, Black, DarkGray },
            { Black, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, DarkGray, Black },
            }
            );
        public static SpriteData sprite_EMPTY = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
            }
            );
        public static SpriteData sprite_FLAG = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { ' ',' ',' ',' ','■',' ','■','■',' ','■','■' },
            { ' ',' ',' ','■',' ','■','■','■','■','■','■' },
            { ' ',' ',' ','■',' ','■','■','■','■','■','■' },
            { ' ',' ',' ','■',' ','■','■','■','■','■','■' },
            { ' ',' ',' ','■',' ','■','■','■','■','■',' ' },
            { ' ',' ',' ','■',' ',' ','■','■','■',' ',' ' },
            { ' ',' ','■',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ','■',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ','■',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ','■','■','■','■',' ',' ',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
            { Black, Black, Black, Black, Yellow, Black, Yellow, Black, Black, Black, Yellow },
            { Black, Black, Black, Black, Yellow, Black, Yellow, Yellow, Black, Yellow, Yellow },
            { Black, Black, Black, Yellow, Black, Yellow, Yellow, Yellow, Yellow, Yellow, Yellow },
            { Black, Black, Black, Yellow, Black, Yellow, Yellow, Yellow, Yellow, Yellow, Yellow },
            { Black, Black, Black, Yellow, Black, Yellow, Yellow, Yellow, Yellow, Yellow, Yellow },
            { Black, Black, Black, Yellow, Black, Yellow, Yellow, Yellow, Yellow, Black, Black },
            { Black, Black, Black, Yellow, Black, Black, Yellow, Yellow, Yellow, Black, Black },
            { Black, Black, Yellow, Yellow, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Yellow, Yellow, Black, Black, Black, Black, Black, Black, Black },
            { Black, Black, Yellow, Yellow, Black, Black, Black, Black, Black, Black, Black },
            { Black, Yellow, Yellow, Yellow, Black, Black, Black, Black, Black, Black, Black },
            }
            );
        public static SpriteData sprite_BABA_R = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ',' ','■',' ',' ','■',' ',' ' },
            { ' ',' ',' ',' ',' ','■','■',' ','■',' ',' ' },
            { ' ',' ','■','■','■','■','■','■','■','■',' ' },
            { ' ','■','■','■','■','■','■','■','■','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { ' ','■','■','■','■','■','■','■','■','■',' ' },
            { ' ','■',' ','■',' ',' ',' ','■','■',' ',' ' },
            { ' ','■',' ','■',' ',' ',' ','■',' ','■',' ' },
            { '■','■',' ','■',' ',' ',' ','■',' ',' ','■' }
            },
            new ConsoleColor[,]{
            {  Black,Black,Black,Black,Black,White,Black,Black,White,Black,Black  },
            {  Black,Black,Black,Black,Black,White,White,Black,White,Black,Black },
            {  Black,Black,White,White,White,White,White,White,White,White,Black  },
            {  Black,White,White,White,White,White,White,White,Gray,White,White  },
            {  White,White,White,White,White,White,Gray,White,White,White,White  },
            {  White,White,White,White,White,White,White,White,White,White,White  },
            {  White,White,White,White,White,White,White,White,White,White,White  },
            {  Black,White,White,White,White,White,White,White,White,White,Black },
            {  Black,White,Black,White,Black,Black,Black,White,White,Black,Black },
            {  Black,White,Black,White,Black,Black,Black,White,Black,White,Black },
            {  White,White,Black,White,Black,Black,Black,White,Black,Black,White  },
            }
            );
        public static SpriteData sprite_BABA_L = new SpriteData(
            new char[,]{
            { ' ',' ','■',' ',' ','■',' ',' ',' ',' ',' ' },
            { ' ',' ','■',' ','■','■',' ',' ',' ',' ',' ' },
            { ' ','■','■','■','■','■','■','■','■',' ',' ' },
            { '■','■','■','■','■','■','■','■','■','■',' ' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { ' ','■','■','■','■','■','■','■','■','■',' ' },
            { ' ',' ','■','■',' ',' ',' ','■',' ','■',' ' },
            { ' ','■',' ','■',' ',' ',' ','■',' ','■',' ' },
            { '■',' ',' ','■',' ',' ',' ','■',' ','■','■' }
            },
            new ConsoleColor[,]{
            { Black,Black,White,Black,Black,White,Black,Black,Black,Black,Black },
            { Black,Black,White,Black,White,White,Black,Black,Black,Black,Black },
            { Black,White,White,White,White,White,White,White,White,Black,Black },
            { White,White,Gray,White,White,White,White,White,White,White,Black },
            { White,White,White,White,Gray,White,White,White,White,White,White },
            { White,White,White,White,White,White,White,White,White,White,White },
            { White,White,White,White,White,White,White,White,White,White,White },
            { Black,White,White,White,White,White,White,White,White,White,Black },
            { Black,Black,White,White,Black,Black,Black,White,Black,White,Black },
            { Black,White,Black,White,Black,Black,Black,White,Black,White,Black },
            { White,Black,Black,White,Black,Black,Black,White,Black,White,White }
            }
            );
        public static SpriteData sprite_BABA_U = new SpriteData(
            new char[,]{
                { ' ',' ',' ','■',' ',' ',' ','■',' ',' ',' ' },
                { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Black,Black,White,Black,Black,Black,White,Black,Black,Black },
                { Black,Black,Black,White,White,White,White,White,Black,Black,Black },
                { Black,Black,White,White,White,White,White,White,White,Black,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,Black,White,White,White,White,White,White,White,Black,Black },
                { Black,Black,White,Black,Black,Black,Black,Black,White,Black,Black },
                { Black,Black,White,Black,Black,Black,Black,Black,White,Black,Black }
            }
            );
        public static SpriteData sprite_BABA_D = new SpriteData(
            new char[,]{
                { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ',' ','■','■','■','■','■','■','■','■',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Black,Black,White,White,White,White,White,Black,Black,Black },
                { Black,Black,White,White,White,White,White,White,White,Black,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,White,White,White,White,White,White,Black },
                { Black,White,White,White,Gray,White,White,White,White,White,Black },
                { Black,Black,White,White,White,White,Gray,White,White,White,Black },
                { Black,Black,White,White,White,White,White,White,White,Black,Black },
                { Black,Black,White,Black,Black,Black,Black,Black,White,Black,Black }
            }
        );
        public static SpriteData word_BABA = new SpriteData(
            new char[,]{
            { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■','■','■','■',' ',' ','■','■','■','■','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■','■','■','■',' ',' ','■',' ',' ',' ','■' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■','■','■','■',' ',' ','■','■','■','■','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■','■','■','■',' ',' ','■',' ',' ',' ','■' }
            },
            new ConsoleColor[,]{
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Black,Magenta,Magenta,Magenta,Black },
            {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Black,Black,Black,Magenta },
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Magenta,Magenta,Magenta,Magenta,Magenta },
            {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Black,Black,Black,Magenta },
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Magenta,Black,Black,Black,Magenta },
            {  Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Black,Magenta,Magenta,Magenta,Black },
            {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Black,Black,Black,Magenta },
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Magenta,Magenta,Magenta,Magenta,Magenta },
            {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Black,Black,Black,Magenta },
            {  Magenta,Magenta,Magenta,Magenta,Black,Black,Magenta,Black,Black,Black,Magenta },
            }
            );
        public static SpriteData word_IS = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ','■','■','■',' ',' ','■','■','■',' ',' ' },
            { ' ',' ','■',' ',' ','■',' ',' ',' ','■',' ' },
            { ' ',' ','■',' ',' ','■',' ',' ',' ',' ',' ' },
            { ' ',' ','■',' ',' ','■',' ',' ',' ',' ',' ' },
            { ' ',' ','■',' ',' ',' ','■','■','■',' ',' ' },
            { ' ',' ','■',' ',' ',' ',' ',' ',' ','■',' ' },
            { ' ',' ','■',' ',' ',' ',' ',' ',' ','■',' ' },
            { ' ',' ','■',' ',' ','■',' ',' ',' ','■',' ' },
            { ' ','■','■','■',' ',' ','■','■','■',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            },
            new ConsoleColor[,]{
            { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            { Black,White,White,White,Black,Black,White,White,White,Black,Black },
            { Black,Black,White,Black,Black,White,Black,Black,Black,White,Black },
            { Black,Black,White,Black,Black,White,Black,Black,Black,Black,Black },
            { Black,Black,White,Black,Black,White,Black,Black,Black,Black,Black },
            { Black,Black,White,Black,Black,Black,White,White,White,Black,Black },
            { Black,Black,White,Black,Black,Black,Black,Black,Black,White,Black },
            { Black,Black,White,Black,Black,Black,Black,Black,Black,White,Black },
            { Black,Black,White,Black,Black,White,Black,Black,Black,White,Black },
            { Black,White,White,White,Black,Black,White,White,White,Black,Black },
            { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
            }
            );
        public static SpriteData word_WIN = new SpriteData(
            new char[,]{
            { '■',' ',' ',' ','■',' ',' ',' ',' ',' ',' ' },
            { '■',' ',' ',' ','■',' ',' ',' ',' ',' ',' ' },
            { '■',' ','■',' ','■',' ',' ',' ',' ',' ',' ' },
            { '■','■',' ','■','■',' ',' ',' ',' ',' ',' ' },
            { '■',' ',' ',' ','■','■','■',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ','■',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ','■','■',' ',' ',' ','■' },
            { ' ',' ',' ',' ',' ','■','■','■',' ',' ','■' },
            { ' ',' ',' ',' ','■','■','■',' ','■',' ','■' },
            { ' ',' ',' ',' ',' ',' ','■',' ',' ','■','■' },
            { ' ',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
            },
            new ConsoleColor[,]{
            {Yellow,Black,Black,Black,Yellow,Black,Black,Black,Black,Black,Black},
            {Yellow,Black,Black,Black,Yellow,Black,Black,Black,Black,Black,Black},
            {Yellow,Black,Yellow,Black,Yellow,Black,Black,Black,Black,Black,Black},
            {Yellow,Yellow,Black,Yellow,Yellow,Black,Black,Black,Black,Black,Black},
            {Yellow,Black,Black,Black,Yellow,DarkYellow,DarkYellow,Black,Black,Black,Black},
            {Black,Black,Black,Black,Black,DarkYellow,Black,Black,Black,Black,Black},
            {Black,Black,Black,Black,Black,DarkYellow,Yellow,Black,Black,Black,Yellow},
            {Black,Black,Black,Black,Black,DarkYellow,Yellow,Yellow,Black,Black,Yellow},
            {Black,Black,Black,Black,DarkYellow,DarkYellow,Yellow,Black,Yellow,Black,Yellow},
            {Black,Black,Black,Black,Black,Black,Yellow,Black,Black,Yellow,Yellow},
            {Black,Black,Black,Black,Black,Black,Yellow,Black,Black,Black,Yellow}
            }
            );
        public static SpriteData word_FLAG = new SpriteData(
            new char[,]{
            { '■','■','■','■','■',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■','■','■','■',' ',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■','■','■','■','■' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ','■','■','■',' ',' ',' ','■','■','■','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
            { '■','■','■','■','■',' ','■',' ','■','■','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ','■',' ',' ','■','■','■','■' },
            },
            new ConsoleColor[,]{
            {Yellow,Yellow,Yellow,Yellow,Yellow,Black,Yellow,Black,Black,Black,Black },
            {Yellow,Black,Black,Black,Black,Black,Yellow,Black,Black,Black,Black },
            {Yellow,Yellow,Yellow,Yellow,Black,Black,Yellow,Black,Black,Black,Black },
            {Yellow,Black,Black,Black,Black,Black,Yellow,Black,Black,Black,Black },
            {Yellow,Black,Black,Black,Black,Black,Yellow,Yellow,Yellow,Yellow,Yellow },
            {Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            {Black,Yellow,Yellow,Yellow,Black,Black,Black,Yellow,Yellow,Yellow,Yellow },
            {Yellow,Black,Black,Black,Yellow,Black,Yellow,Black,Black,Black,Black },
            {Yellow,Yellow,Yellow,Yellow,Yellow,Black,Yellow,Black,Yellow,Yellow,Yellow },
            {Yellow,Black,Black,Black,Yellow,Black,Yellow,Black,Black,Black,Yellow },
            {Yellow,Black,Black,Black,Yellow,Black,Black,Yellow,Yellow,Yellow,Yellow }
            }
            );
        public static SpriteData word_YOU = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { '■',' ','■',' ','■','■','■',' ','■',' ','■' },
            { '■',' ','■',' ','■',' ','■',' ','■',' ','■' },
            { '■','■','■',' ','■',' ','■',' ','■',' ','■' },
            { ' ','■',' ',' ','■',' ','■',' ','■',' ','■' },
            { ' ','■',' ',' ','■',' ','■',' ','■',' ','■' },
            { ' ','■',' ',' ','■',' ','■',' ','■',' ','■' },
            { ' ','■',' ',' ','■','■','■',' ','■','■','■' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            },
            new ConsoleColor[,]{
            {Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            {Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            {Magenta,Black,Magenta,Black,Magenta,Magenta,Magenta,Black,Magenta,Black,Magenta },
            {Magenta,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta },
            {Magenta,Magenta,Magenta,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta },
            {Black,Magenta,Black,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta },
            {Black,Magenta,Black,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta },
            {Black,Magenta,Black,Black,Magenta,Black,Magenta,Black,Magenta,Black,Magenta },
            {Black,Magenta,Black,Black,Magenta,Magenta,Magenta,Black,Magenta,Magenta,Magenta },
            {Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            {Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
            }
            );
        public static SpriteData word_WALL = new SpriteData(
            new char[,]{
            { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■',' ','■',' ','■',' ','■','■','■','■','■' },
            { '■','■',' ','■','■',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
            { '■','■','■','■','■',' ','■','■','■','■','■' }
            },
            new ConsoleColor[,]{
            { DarkGray,Black,Black,Black,DarkGray,Black,Black,DarkGray,DarkGray,DarkGray,Black },
            { DarkGray,Black,Black,Black,DarkGray,Black,DarkGray,Black,Black,Black,DarkGray },
            { DarkGray,Black,DarkGray,Black,DarkGray,Black,DarkGray,DarkGray,DarkGray,DarkGray,DarkGray },
            { DarkGray,DarkGray,Black,DarkGray,DarkGray,Black,DarkGray,Black,Black,Black,DarkGray },
            { DarkGray,Black,Black,Black,DarkGray,Black,DarkGray,Black,Black,Black,DarkGray },
            { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            { DarkGray,Black,Black,Black,Black,Black,DarkGray,Black,Black,Black,Black },
            { DarkGray,Black,Black,Black,Black,Black,DarkGray,Black,Black,Black,Black },
            { DarkGray,Black,Black,Black,Black,Black,DarkGray,Black,Black,Black,Black },
            { DarkGray,Black,Black,Black,Black,Black,DarkGray,Black,Black,Black,Black },
            { DarkGray,DarkGray,DarkGray,DarkGray,DarkGray,Black,DarkGray,DarkGray,DarkGray,DarkGray,DarkGray }
            }
            );
        public static SpriteData word_STOP = new SpriteData(
            new char[,]{
            { ' ','■','■','■','■',' ','■','■','■','■','■' },
            { '■',' ',' ',' ',' ',' ',' ',' ','■',' ',' ' },
            { ' ','■','■','■',' ',' ',' ',' ','■',' ',' ' },
            { ' ',' ',' ',' ','■',' ',' ',' ','■',' ',' ' },
            { '■','■','■','■',' ',' ',' ',' ','■',' ',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ','■','■','■',' ',' ','■','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ','■',' ','■','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
            { ' ','■','■','■',' ',' ','■',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
            { Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,DarkGreen  },
            { DarkGreen,Black,Black,Black,Black,Black,Black,Black,DarkGreen,Black,Black  },
            { Black,DarkGreen,DarkGreen,DarkGreen,Black,Black,Black,Black,DarkGreen,Black,Black  },
            { Black,Black,Black,Black,DarkGreen,Black,Black,Black,DarkGreen,Black,Black  },
            { DarkGreen,DarkGreen,DarkGreen,DarkGreen,Black,Black,Black,Black,DarkGreen,Black,Black  },
            { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black  },
            { Black,DarkGreen,DarkGreen,DarkGreen,Black,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,Black  },
            { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,Black,Black,Black,DarkGreen  },
            { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,Black  },
            { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,Black,Black,Black,Black  },
            { Black,DarkGreen,DarkGreen,DarkGreen,Black,Black,DarkGreen,Black,Black,Black,Black  }
            }
            );
        public static SpriteData sprite_ROCK = new SpriteData(
            new char[,]{
            { ' ',' ',' ',' ','■','■','■','■',' ',' ',' ' },
            { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
            { ' ','■','■','■','■','■',' ','■','■','■',' ' },
            { '■','■','■','■','■','■','■',' ',' ','■','■' },
            { '■','■','■','■','■','■','■','■','■','■','■' },
            { ' ','■','■',' ','■','■','■','■','■','■','■' },
            { '■',' ',' ','■','■','■','■',' ','■','■','■' },
            { '■','■','■','■','■','■','■','■',' ',' ',' ' },
            { ' ','■','■','■',' ',' ','■','■','■','■',' ' },
            { ' ',' ','■','■','■','■',' ','■','■','■',' ' },
            { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' }
            },
            new ConsoleColor[,]{
            { Black,Black,Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,Black },
            { Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black },
            { Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,DarkYellow,DarkYellow,DarkYellow,Black },
            { DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,DarkYellow,DarkYellow },
            { DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow },
            { Black,DarkYellow,DarkYellow,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow },
            { DarkYellow,Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,DarkYellow,DarkYellow,DarkYellow },
            { DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,Black },
            { Black,DarkYellow,DarkYellow,DarkYellow,Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black },
            { Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,DarkYellow,DarkYellow,DarkYellow,Black },
            { Black,Black,Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,Black }
            }
            );
        public static SpriteData word_ROCK = new SpriteData(
            new char[,]{
            { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■','■','■','■',' ',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
            { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
            { ' ','■','■','■','■',' ','■',' ',' ',' ','■' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ','■',' ' },
            { '■',' ',' ',' ',' ',' ','■','■','■',' ',' ' },
            { '■',' ',' ',' ',' ',' ','■',' ',' ','■',' ' },
            { ' ','■','■','■','■',' ','■',' ',' ',' ','■' }
            },
            new ConsoleColor[,]{
            { DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,Black,DarkYellow,DarkYellow,DarkYellow,Black },
            { DarkYellow,Black,Black,Black,DarkYellow,Black,DarkYellow,Black,Black,Black,DarkYellow },
            { DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,Black,DarkYellow,Black,Black,Black,DarkYellow },
            { DarkYellow,Black,Black,Black,DarkYellow,Black,DarkYellow,Black,Black,Black,DarkYellow },
            { DarkYellow,Black,Black,Black,DarkYellow,Black,Black,DarkYellow,DarkYellow,DarkYellow,Black },
            { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
            { Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,DarkYellow,Black,Black,Black,DarkYellow },
            { DarkYellow,Black,Black,Black,Black,Black,DarkYellow,Black,Black,DarkYellow,Black },
            { DarkYellow,Black,Black,Black,Black,Black,DarkYellow,DarkYellow,DarkYellow,Black,Black },
            { DarkYellow,Black,Black,Black,Black,Black,DarkYellow,Black,Black,DarkYellow,Black },
            { Black,DarkYellow,DarkYellow,DarkYellow,DarkYellow,Black,DarkYellow,Black,Black,Black,DarkYellow }
            }
            );
        public static SpriteData word_PUSH = new SpriteData(
            new char[,] {
                { '■', '■', '■', '■', ' ', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', ' ', ' ', ' ', '■', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', '■', '■', '■', ' ', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', ' ', ' ', ' ', ' ', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', ' ', ' ', ' ', ' ', ' ', ' ', '■', '■', '■', ' ' },
                { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
                { ' ', '■', '■', '■', '■', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', ' ', ' ', ' ', ' ', ' ', '■', ' ', ' ', ' ', '■' },
                { ' ', '■', '■', '■', ' ', ' ', '■', '■', '■', '■', '■' },
                { ' ', ' ', ' ', ' ', '■', ' ', '■', ' ', ' ', ' ', '■' },
                { '■', '■', '■', '■', ' ', ' ', '■', ' ', ' ', ' ', '■' }
            },
            new ConsoleColor[,] {
                { DarkYellow, DarkYellow, DarkYellow, DarkYellow, Black, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, Black, Black, Black, DarkYellow, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, DarkYellow, DarkYellow, DarkYellow, Black, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, Black, Black, Black, Black, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, Black, Black, Black, Black, Black, Black, DarkYellow, DarkYellow, DarkYellow, Black },
                { Black, Black, Black, Black, Black, Black, Black, Black, Black, Black, Black },
                { Black, DarkYellow, DarkYellow, DarkYellow, DarkYellow, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, Black, Black, Black, Black, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { Black, DarkYellow, DarkYellow, DarkYellow, Black, Black, DarkYellow, DarkYellow, DarkYellow, DarkYellow, DarkYellow },
                { Black, Black, Black, Black, DarkYellow, Black, DarkYellow, Black, Black, Black, DarkYellow },
                { DarkYellow, DarkYellow, DarkYellow, DarkYellow, Black, Black, DarkYellow, Black, Black, Black, DarkYellow }
            }
            );
        public static SpriteData word_LAVA = new SpriteData(
            new char[,]{
                { '■',' ',' ',' ',' ',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■','■','■','■','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■','■','■','■','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ','■',' ',' ','■','■','■','■','■' },
                { '■',' ','■',' ',' ',' ','■',' ',' ',' ','■' },
                { '■','■',' ',' ',' ',' ','■',' ',' ',' ','■' }
            },
            new ConsoleColor[,]{
                {  DarkRed,Black,Black,Black,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                {  DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                {  DarkRed,Black,Black,Black,Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                {  DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                {  DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black,DarkRed,Black,Black,Black,DarkRed },
                {  Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                {  DarkRed,Black,Black,Black,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                {  DarkRed,Black,Black,Black,DarkRed,Black,DarkRed,Black,Black,Black,DarkRed },
                {  DarkRed,Black,Black,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                {  DarkRed,Black,DarkRed,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                {  DarkRed,DarkRed,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed }
            }
        );
        public static SpriteData sprite_LAVA = new SpriteData(
            new char[,]{
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■',' ',' ','■','■',' ',' ','■','■','■' },
                { ' ',' ','■','■',' ',' ','■','■',' ',' ',' ' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { ' ',' ',' ','■','■',' ',' ','■','■',' ',' ' },
                { '■','■','■',' ',' ','■','■',' ',' ','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■',' ',' ','■','■',' ',' ','■','■','■' },
                { ' ',' ','■','■',' ',' ','■','■',' ',' ',' ' },
                { '■','■','■','■','■','■','■','■','■','■','■' }
            },
            new ConsoleColor[,]{
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed },
                { Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black,Black },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { Black,Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black },
                { DarkRed,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed },
                { Black,Black,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,Black,Black,Black },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed }
            }
        );
        public static SpriteData word_KILL = new SpriteData(
            new char[,]{
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { '■',' ',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { '■','■','■',' ',' ',' ',' ',' ','■',' ',' ' },
                { '■',' ',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { '■','■','■','■','■',' ','■','■','■','■','■' }
                    },
            new ConsoleColor[,]{
                { DarkRed,Black,Black,Black,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { DarkRed,Black,Black,DarkRed,Black,Black,Black,Black,DarkRed,Black,Black },
                { DarkRed,DarkRed,DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black },
                { DarkRed,Black,Black,DarkRed,Black,Black,Black,Black,DarkRed,Black,Black },
                { DarkRed,Black,Black,Black,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed }
            }
        );
        public static SpriteData word_BONE = new SpriteData(
            new char[,]{
                { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■','■','■','■',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■','■',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ','■',' ','■',' ','■','■','■','■',' ' },
                { '■',' ',' ','■','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' }
                    },
            new ConsoleColor[,]{
                { DarkRed,DarkRed,DarkRed,DarkRed,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { DarkRed,Black,Black,Black,DarkRed,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,Black,Black,Black,DarkRed,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,DarkRed,Black,Black,DarkRed,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,Black,DarkRed,Black,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,Black },
                { DarkRed,Black,Black,DarkRed,DarkRed,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,Black,Black,Black,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed }
            }
        );
        public static SpriteData sprite_BONE = new SpriteData(
            new char[,]{
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■',' ',' ',' ','■','■','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■','■','■',' ',' ',' ','■' },
                { '■','■',' ','■','■','■','■','■',' ','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { ' ','■','■','■',' ','■',' ','■','■','■',' ' },
                { ' ',' ','■','■','■','■','■','■','■','■',' ' },
                { ' ',' ','■',' ','■','■','■',' ','■',' ',' ' },
                { ' ',' ',' ',' ','■','■','■',' ',' ',' ',' ' }
                    },
            new ConsoleColor[,]{
                { Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black,Black },
                { Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black,DarkRed,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { Black,DarkRed,DarkRed,DarkRed,Black,DarkRed,Black,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,DarkRed,Black,DarkRed,DarkRed,DarkRed,Black,DarkRed,Black,Black },
                { Black,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black,Black,Black,Black }
            }
        );        
        public static SpriteData word_MOVE = new SpriteData(
            new char[,]{
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { '■','■',' ','■','■',' ','■',' ',' ',' ','■' },
                { '■',' ','■',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■',' ' },
                { ' ','■',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { ' ',' ','■',' ',' ',' ','■','■','■','■','■' }
            },
            new ConsoleColor[,]{
                { DarkGreen,Black,Black,Black,DarkGreen,Black,Black,DarkGreen,DarkGreen,DarkGreen,Black },
                { DarkGreen,DarkGreen,Black,DarkGreen,DarkGreen,Black,DarkGreen,Black,Black,Black,DarkGreen },
                { DarkGreen,Black,DarkGreen,Black,DarkGreen,Black,DarkGreen,Black,Black,Black,DarkGreen },
                { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,Black,Black,Black,DarkGreen },
                { DarkGreen,Black,Black,Black,DarkGreen,Black,Black,DarkGreen,DarkGreen,DarkGreen,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,DarkGreen },
                { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,Black,Black,Black,Black },
                { DarkGreen,Black,Black,Black,DarkGreen,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,Black },
                { Black,DarkGreen,Black,DarkGreen,Black,Black,DarkGreen,Black,Black,Black,Black },
                { Black,Black,DarkGreen,Black,Black,Black,DarkGreen,DarkGreen,DarkGreen,DarkGreen,DarkGreen }
            }
        );       
        public static SpriteData word_KEKE = new SpriteData(
            new char[,]{
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■',' ',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { '■','■','■',' ',' ',' ','■','■','■','■',' ' },
                { '■',' ',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■',' ',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { '■','■','■',' ',' ',' ','■','■','■','■',' ' },
                { '■',' ',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' }
            },
            new ConsoleColor[,]{
                { Red,Black,Black,Black,Red,Black,Red,Red,Red,Red,Red },
                { Red,Black,Black,Red,Black,Black,Red,Black,Black,Black,Black },
                { Red,Red,Red,Black,Black,Black,Red,Red,Red,Red,Black },
                { Red,Black,Black,Red,Black,Black,Red,Black,Black,Black,Black },
                { Red,Black,Black,Black,Red,Black,Red,Red,Red,Red,Red },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Red,Black,Black,Black,Red,Black,Red,Red,Red,Red,Red },
                { Red,Black,Black,Red,Black,Black,Red,Black,Black,Black,Black },
                { Red,Red,Red,Black,Black,Black,Red,Red,Red,Red,Black },
                { Red,Black,Black,Red,Black,Black,Red,Black,Black,Black,Black },
                { Red,Black,Black,Black,Red,Black,Red,Red,Red,Red,Red }
            }
        );
        public static SpriteData sprite_KEKE = new SpriteData(
            new char[,]{
                { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ','■','■',' ','■',' ','■','■',' ',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { '■',' ','■','■','■','■','■','■','■',' ','■' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' },
                { ' ',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { ' ',' ',' ','■','■',' ','■','■',' ',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Black,Black,Red,Red,Red,Red,Red,Black,Black,Black },
                { Black,Black,Red,Red,Red,Red,Red,Red,Red,Black,Black },
                { Black,Black,Red,Red,Black,Red,Black,Red,Red,Black,Black },
                { Black,Black,Red,Red,Red,Red,Red,Red,Red,Black,Black },
                { Black,Black,Red,Red,Red,Red,Red,Red,Red,Black,Black },
                { Black,Red,Red,Red,Red,Red,Red,Red,Red,Red,Black },
                { Red,Black,Red,Red,Red,Red,Red,Red,Red,Black,Red },
                { Black,Black,Red,Red,Red,Red,Red,Red,Red,Black,Black },
                { Black,Black,Black,Red,Red,Red,Red,Red,Black,Black,Black },
                { Black,Black,Black,Black,Red,Black,Red,Black,Black,Black,Black },
                { Black,Black,Black,Red,Red,Black,Red,Red,Black,Black,Black }
            }
        );
        public static SpriteData sprite_GRASS = new SpriteData(
            new char[,]{
                { ' ',' ',' ',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ',' ',' ',' ',' ','■',' ','■',' ',' ' },
                { ' ',' ',' ',' ',' ',' ','■',' ','■',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ','■',' ','■',' ',' ',' ',' ',' ',' ' },
                { '■',' ','■',' ','■',' ',' ',' ',' ',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Black,Black,Black,Black,Black,Black,Black,Green,Black,Black },
                { Black,Black,Black,Black,Black,Black,Green,Black,Green,Black,Black },
                { Black,Black,Black,Black,Black,Black,Green,Black,Green,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Black,Green,Black,Black,Black,Black,Black,Black,Black,Black },
                { Green,Black,Green,Black,Green,Black,Black,Black,Black,Black,Black },
                { Green,Black,Green,Black,Green,Black,Black,Black,Black,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Green,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Green,Green,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
            }
        );
        public static SpriteData word_GRASS = new SpriteData(
            new char[,]{
                { ' ','■','■','■','■',' ','■','■','■','■',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ','■','■','■',' ','■','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { ' ','■','■','■','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ',' ','■',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ','■',' ' },
                { '■','■','■','■','■',' ',' ','■',' ',' ','■' },
                { '■',' ',' ',' ','■',' ',' ','■',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ','■',' ' }
            },
            new ConsoleColor[,]{
                { Black,Green,Green,Green,Green,Black,Green,Green,Green,Green,Black },
                { Green,Black,Black,Black,Black,Black,Green,Black,Black,Black,Green },
                { Green,Black,Green,Green,Green,Black,Green,Green,Green,Green,Black },
                { Green,Black,Black,Black,Green,Black,Green,Black,Black,Black,Green },
                { Black,Green,Green,Green,Green,Black,Green,Black,Black,Black,Green },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Green,Green,Green,Black,Black,Black,Green,Black,Black,Green },
                { Green,Black,Black,Black,Green,Black,Green,Black,Black,Green,Black },
                { Green,Green,Green,Green,Green,Black,Black,Green,Black,Black,Green },
                { Green,Black,Black,Black,Green,Black,Black,Green,Black,Black,Green },
                { Green,Black,Black,Black,Green,Black,Green,Black,Black,Green,Black }
            }
        );
        public static SpriteData word_ICE = new SpriteData(
            new char[,]{
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■',' ',' ','■','■',' ',' ','■','■','■' },
                { ' ','■',' ','■',' ',' ','■',' ','■',' ',' ' },
                { ' ','■',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ','■',' ',' ',' ',' ','■','■',' ' },
                { ' ','■',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ','■',' ',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ','■',' ',' ','■',' ','■',' ',' ' },
                { ' ','■',' ',' ','■','■',' ',' ','■','■','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Cyan,Black,Black,Cyan,Cyan,Black,Black,Cyan,Cyan,Cyan },
                { Black,Cyan,Black,Cyan,Black,Black,Cyan,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Black,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Black,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Black,Black,Cyan,Cyan,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Black,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Black,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Cyan,Black,Black,Cyan,Black,Cyan,Black,Black },
                { Black,Cyan,Black,Black,Cyan,Cyan,Black,Black,Cyan,Cyan,Cyan },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
            }
        );
        public static SpriteData sprite_ICE = new SpriteData(
            new char[,]{
                { ' ','■',' ','■','■','■',' ',' ','■','■',' ' },
                { '■',' ',' ',' ',' ',' ',' ','■',' ',' ','■' },
                { '■',' ',' ','■',' ',' ','■',' ',' ',' ','■' },
                { ' ',' ','■',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','■' },
                { '■',' ','■',' ',' ',' ',' ',' ',' ','■',' ' },
                { ' ','■',' ',' ',' ',' ',' ',' ','■',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ','■',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ',' ',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ','■',' ',' ',' ',' ','■' },
                { ' ','■',' ','■','■',' ',' ',' ','■','■',' ' }
            },
            new ConsoleColor[,]{
                { Black,Cyan,Black,Cyan,Cyan,Cyan,Black,Black,Cyan,Cyan,Black },
                { Cyan,Black,Black,Black,Black,Black,Black,Cyan,Black,Black,Cyan },
                { Cyan,Black,Black,Cyan,Black,Black,Cyan,Black,Black,Black,Cyan },
                { Black,Black,Cyan,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Cyan },
                { Cyan,Black,Cyan,Black,Black,Black,Black,Black,Black,Cyan,Black },
                { Black,Cyan,Black,Black,Black,Black,Black,Black,Cyan,Black,Cyan },
                { Black,Black,Black,Black,Black,Black,Black,Cyan,Black,Black,Black },
                { Cyan,Black,Black,Black,Black,Black,Black,Black,Black,Black,Cyan },
                { Cyan,Black,Black,Black,Black,Cyan,Black,Black,Black,Black,Cyan },
                { Black,Cyan,Black,Cyan,Cyan,Black,Black,Black,Cyan,Cyan,Black }
            }
        );
        public static SpriteData word_SLIP = new SpriteData(
            new char[,]{
                { ' ','■','■','■','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ',' ' },
                { ' ',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■','■','■','■',' ',' ','■','■','■','■','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■','■','■','■',' ' },
                { ' ',' ','■',' ',' ',' ','■',' ',' ',' ','■' },
                { ' ',' ','■',' ',' ',' ','■','■','■','■',' ' },
                { ' ',' ','■',' ',' ',' ','■',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ',' ' }
            },
             new ConsoleColor[,]{
                { Black,Cyan,Cyan,Cyan,Cyan,Black,Cyan,Black,Black,Black,Black },
                { Cyan,Black,Black,Black,Black,Black,Cyan,Black,Black,Black,Black },
                { Black,Cyan,Cyan,Cyan,Black,Black,Cyan,Black,Black,Black,Black },
                { Black,Black,Black,Black,Cyan,Black,Cyan,Black,Black,Black,Black },
                { Cyan,Cyan,Cyan,Cyan,Black,Black,Cyan,Cyan,Cyan,Cyan,Cyan },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Cyan,Cyan,Cyan,Black,Black,Cyan,Cyan,Cyan,Cyan,Black },
                { Black,Black,Cyan,Black,Black,Black,Cyan,Black,Black,Black,Cyan },
                { Black,Black,Cyan,Black,Black,Black,Cyan,Cyan,Cyan,Cyan,Black },
                { Black,Black,Cyan,Black,Black,Black,Cyan,Black,Black,Black,Black },
                { Black,Cyan,Cyan,Cyan,Black,Black,Cyan,Black,Black,Black,Black }
             }
        );
        public static SpriteData word_LOVE = new SpriteData(
            new char[,]{
                { '■',' ',' ',' ',' ',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■','■','■','■','■',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■',' ' },
                { ' ','■',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { ' ',' ','■',' ',' ',' ','■','■','■','■','■' }
            },
             new ConsoleColor[,]{
                {  Magenta,Black,Black,Black,Black,Black,Black,Magenta,Magenta,Magenta,Black },
                {  Magenta,Black,Black,Black,Black,Black,Magenta,Black,Black,Black,Magenta },
                {  Magenta,Black,Black,Black,Black,Black,Magenta,Black,Black,Black,Magenta },
                {  Magenta,Black,Black,Black,Black,Black,Magenta,Black,Black,Black,Magenta },
                {  Magenta,Magenta,Magenta,Magenta,Magenta,Black,Black,Magenta,Magenta,Magenta,Black },
                {  Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Magenta,Magenta,Magenta,Magenta },
                {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Black,Black,Black,Black },
                {  Magenta,Black,Black,Black,Magenta,Black,Magenta,Magenta,Magenta,Magenta,Black },
                {  Black,Magenta,Black,Magenta,Black,Black,Magenta,Black,Black,Black,Black },
                {  Black,Black,Magenta,Black,Black,Black,Magenta,Magenta,Magenta,Magenta,Magenta }
             }
        );
        public static SpriteData sprite_LOVE = new SpriteData(
            new char[,]{
                { ' ','■','■','■',' ',' ',' ','■','■','■',' ' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { '■','■','■','■','■','■','■','■','■','■','■' },
                { ' ','■','■','■','■','■','■','■','■','■',' ' },
                { ' ',' ','■','■','■','■','■','■','■',' ',' ' },
                { ' ',' ',' ','■','■','■','■','■',' ',' ',' ' },
                { ' ',' ',' ',' ','■','■','■',' ',' ',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' }
            },
            new ConsoleColor[,]{
                { Black,Magenta,Magenta,Magenta,Black,Black,Black,Magenta,Magenta,Magenta,Black },
                { Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta },
                { Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta },
                { Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta },
                { Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta },
                { Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta },
                { Black,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Black },
                { Black,Black,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Magenta,Black,Black },
                { Black,Black,Black,Magenta,Magenta,Magenta,Magenta,Magenta,Black,Black,Black },
                { Black,Black,Black,Black,Magenta,Magenta,Magenta,Black,Black,Black,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
            }
        );
        public static SpriteData word_EMPTY = new SpriteData(
            new char[,]{
                { '■','■','■','■','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■','■',' ','■','■' },
                { '■','■','■','■',' ',' ','■',' ','■',' ','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■','■','■','■','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■','■','■',' ','■','■','■',' ','■',' ','■' },
                { '■',' ','■',' ',' ','■',' ',' ','■',' ','■' },
                { '■','■','■',' ',' ','■',' ',' ','■','■','■' },
                { '■',' ',' ',' ',' ','■',' ',' ',' ','■',' ' },
                { '■',' ',' ',' ',' ','■',' ',' ',' ','■',' ' }
            },
            new ConsoleColor[,]{
                { White,White,White,White,White,Black,White,Black,Black,Black,White },
                { White,Black,Black,Black,Black,Black,White,White,Black,White,White },
                { White,White,White,White,Black,Black,White,Black,White,Black,White },
                { White,Black,Black,Black,Black,Black,White,Black,Black,Black,White },
                { White,White,White,White,White,Black,White,Black,Black,Black,White },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { White,White,White,Black,White,White,White,Black,White,Black,White },
                { White,Black,White,Black,Black,White,Black,Black,White,Black,White },
                { White,White,White,Black,Black,White,Black,Black,White,White,White },
                { White,Black,Black,Black,Black,White,Black,Black,Black,White,Black },
                { White,Black,Black,Black,Black,White,Black,Black,Black,White,Black }
            }
        );
        public static SpriteData word_PLAY = new SpriteData(
            new char[,]{
                { '■','■','■','■',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■','■','■','■',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ','■','■','■','■','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ',' ','■',' ','■',' ' },
                { '■','■','■','■','■',' ',' ',' ','■',' ',' ' },
                { '■',' ',' ',' ','■',' ',' ',' ','■',' ',' ' },
                { '■',' ',' ',' ','■',' ',' ',' ','■',' ',' ' }
            },
            new ConsoleColor[,]{
                { DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,Black,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,Black,Black,DarkCyan,DarkCyan,DarkCyan,DarkCyan,DarkCyan },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,DarkCyan,DarkCyan,DarkCyan,Black,Black,DarkCyan,Black,Black,Black,DarkCyan },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,Black,DarkCyan,Black,DarkCyan,Black },
                { DarkCyan,DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black,Black,Black,DarkCyan,Black,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,Black,Black,DarkCyan,Black,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,Black,Black,DarkCyan,Black,Black }
            }
        );
        public static SpriteData word_EXIT = new SpriteData(
            new char[,]{
                { '■','■','■','■','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ',' ','■',' ','■',' ' },
                { '■','■','■','■',' ',' ',' ',' ','■',' ',' ' },
                { '■',' ',' ',' ',' ',' ',' ','■',' ','■',' ' },
                { '■','■','■','■','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■','■','■','■','■' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ','■',' ',' ',' ',' ',' ','■',' ',' ' },
                { ' ','■','■','■',' ',' ',' ',' ','■',' ',' ' }
            },
            new ConsoleColor[,]{
                { Red,Red,Red,Red,Red,Black,Red,Black,Black,Black,Red },
                { Red,Black,Black,Black,Black,Black,Black,Red,Black,Red,Black },
                { Red,Red,Red,Red,Black,Black,Black,Black,Red,Black,Black },
                { Red,Black,Black,Black,Black,Black,Black,Red,Black,Red,Black },
                { Red,Red,Red,Red,Red,Black,Red,Black,Black,Black,Red },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,Red,Red,Red,Black,Black,Red,Red,Red,Red,Red },
                { Black,Black,Red,Black,Black,Black,Black,Black,Red,Black,Black },
                { Black,Black,Red,Black,Black,Black,Black,Black,Red,Black,Black },
                { Black,Black,Red,Black,Black,Black,Black,Black,Red,Black,Black },
                { Black,Red,Red,Red,Black,Black,Black,Black,Red,Black,Black }
            }
        ); 
        public static SpriteData word_GAME = new SpriteData(
             new char[,]{
                { ' ','■','■','■','■',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ','■','■','■',' ','■','■','■','■','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { ' ','■','■','■','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■','■',' ','■','■',' ','■',' ',' ',' ',' ' },
                { '■',' ','■',' ','■',' ','■','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' }
             },
             new ConsoleColor[,]{
                { Black,Cyan,Cyan,Cyan,Cyan,Black,Black,Cyan,Cyan,Cyan,Black },
                { Cyan,Black,Black,Black,Black,Black,Cyan,Black,Black,Black,Cyan },
                { Cyan,Black,Cyan,Cyan,Cyan,Black,Cyan,Cyan,Cyan,Cyan,Cyan },
                { Cyan,Black,Black,Black,Cyan,Black,Cyan,Black,Black,Black,Cyan },
                { Black,Cyan,Cyan,Cyan,Cyan,Black,Cyan,Black,Black,Black,Cyan },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Cyan,Black,Black,Black,Cyan,Black,Cyan,Cyan,Cyan,Cyan,Cyan },
                { Cyan,Cyan,Black,Cyan,Cyan,Black,Cyan,Black,Black,Black,Black },
                { Cyan,Black,Cyan,Black,Cyan,Black,Cyan,Cyan,Cyan,Cyan,Black },
                { Cyan,Black,Black,Black,Cyan,Black,Cyan,Black,Black,Black,Black },
                { Cyan,Black,Black,Black,Cyan,Black,Cyan,Cyan,Cyan,Cyan,Cyan }
             }
         );
        public static SpriteData word_DONE = new SpriteData(
             new char[,]{
                { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■','■','■','■',' ',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' },
                { '■','■',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ','■',' ','■',' ','■','■','■','■',' ' },
                { '■',' ',' ','■','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■','■' }
             },
             new ConsoleColor[,]{
                { DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black,Black,Black,DarkCyan,DarkCyan,DarkCyan,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,Black,Black,Black,DarkCyan },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,Black,Black,Black,DarkCyan },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,Black,Black,Black,DarkCyan },
                { DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black,Black,Black,DarkCyan,DarkCyan,DarkCyan,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,DarkCyan,DarkCyan,DarkCyan,DarkCyan },
                { DarkCyan,DarkCyan,Black,Black,DarkCyan,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,Black,DarkCyan,Black,DarkCyan,Black,DarkCyan,DarkCyan,DarkCyan,DarkCyan,Black },
                { DarkCyan,Black,Black,DarkCyan,DarkCyan,Black,DarkCyan,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,DarkCyan,Black,DarkCyan,DarkCyan,DarkCyan,DarkCyan,DarkCyan }
             }
         );
        public static SpriteData word_END = new SpriteData(
             new char[,]{
                { '■','■','■',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■',' ',' ','■',' ',' ',' ','■',' ',' ',' ' },
                { '■','■',' ','■','■',' ',' ','■',' ',' ',' ' },
                { '■',' ',' ','■','■',' ',' ','■','■','■',' ' },
                { '■',' ',' ','■',' ','■',' ','■','■',' ','■' },
                { '■','■','■','■',' ',' ','■','■','■',' ','■' },
                { ' ',' ',' ','■',' ',' ','■','■','■',' ','■' },
                { ' ',' ',' ','■',' ',' ',' ','■','■',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ','■',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ','■','■',' ' }
             },
             new ConsoleColor[,]{
                { DarkCyan,DarkCyan,DarkCyan,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { DarkCyan,Black,Black,Blue,Black,Black,Black,Blue,Black,Black,Black },
                { DarkCyan,DarkCyan,Black,Blue,Blue,Black,Black,Blue,Black,Black,Black },
                { DarkCyan,Black,Black,Blue,Blue,Black,Black,Blue,DarkBlue,DarkBlue,Black },
                { DarkCyan,Black,Black,Blue,Black,Blue,Black,Blue,DarkBlue,Black,DarkBlue },
                { DarkCyan,DarkCyan,DarkCyan,Blue,Black,Black,Blue,Blue,DarkBlue,Black,DarkBlue },
                { Black,Black,Black,Blue,Black,Black,Blue,Blue,DarkBlue,Black,DarkBlue },
                { Black,Black,Black,Blue,Black,Black,Black,Blue,DarkBlue,Black,DarkBlue },
                { Black,Black,Black,Black,Black,Black,Black,Black,DarkBlue,Black,DarkBlue },
                { Black,Black,Black,Black,Black,Black,Black,Black,DarkBlue,DarkBlue,Black }
             }
         );
        public static SpriteData word_MADE = new SpriteData(
             new char[,]{
                { '■',' ',' ',' ','■',' ',' ','■','■','■',' ' },
                { '■','■',' ','■','■',' ','■',' ',' ',' ','■' },
                { '■',' ','■',' ','■',' ','■','■','■','■','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { '■','■','■','■',' ',' ','■','■','■','■','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■',' ',' ',' ','■',' ','■','■','■','■',' ' },
                { '■',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■','■','■','■',' ',' ','■','■','■','■','■' }
             },
             new ConsoleColor[,]{
                { White,Black,Black,Black,White,Black,Black,White,White,White,Black },
                { White,White,Black,White,White,Black,White,Black,Black,Black,White },
                { White,Black,White,Black,White,Black,White,White,White,White,White },
                { White,Black,Black,Black,White,Black,White,Black,Black,Black,White },
                { White,Black,Black,Black,White,Black,White,Black,Black,Black,White },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { White,White,White,White,Black,Black,White,White,White,White,White },
                { White,Black,Black,Black,White,Black,White,Black,Black,Black,Black },
                { White,Black,Black,Black,White,Black,White,White,White,White,Black },
                { White,Black,Black,Black,White,Black,White,Black,Black,Black,Black },
                { White,White,White,White,Black,Black,White,White,White,White,White }
             }
         );
        public static SpriteData word_LOSE = new SpriteData(
             new char[,]{
                { '■',' ',' ',' ',' ',' ',' ','■','■','■',' ' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ','■' },
                { '■','■','■','■','■',' ',' ','■','■','■',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■','■',' ','■','■','■','■','■' },
                { '■',' ',' ',' ',' ',' ','■',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■','■','■','■',' ' },
                { ' ',' ',' ',' ','■',' ','■',' ',' ',' ',' ' },
                { '■','■','■','■',' ',' ','■','■','■','■','■' }
             },
             new ConsoleColor[,]{
                { DarkRed,Black,Black,Black,Black,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,DarkRed },
                { DarkRed,DarkRed,DarkRed,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,DarkRed,DarkRed,DarkRed,DarkRed,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed },
                { DarkRed,Black,Black,Black,Black,Black,DarkRed,Black,Black,Black,Black },
                { Black,DarkRed,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,Black },
                { Black,Black,Black,Black,DarkRed,Black,DarkRed,Black,Black,Black,Black },
                { DarkRed,DarkRed,DarkRed,DarkRed,Black,Black,DarkRed,DarkRed,DarkRed,DarkRed,DarkRed }
             }
         );
        public static SpriteData word_BY = new SpriteData(
             new char[,]{
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ','■' },
                { ' ','■',' ',' ','■',' ','■',' ',' ',' ','■' },
                { ' ','■',' ',' ','■',' ','■',' ',' ',' ','■' },
                { ' ','■',' ',' ','■',' ','■','■',' ','■','■' },
                { ' ','■',' ','■',' ',' ',' ','■','■','■',' ' },
                { ' ','■',' ',' ','■',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ',' ','■',' ',' ',' ','■',' ',' ' },
                { ' ','■',' ',' ','■',' ',' ',' ','■',' ',' ' },
                { ' ','■','■','■',' ',' ',' ',' ','■',' ',' ' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' }
             },
             new ConsoleColor[,]{
                 { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                 { Black,Gray,Gray,Gray,Black,Black,Gray,Black,Black,Black,Gray },
                 { Black,Gray,Black,Black,Gray,Black,Gray,Black,Black,Black,Gray },
                 { Black,Gray,Black,Black,Gray,Black,Gray,Black,Black,Black,Gray },
                 { Black,Gray,Black,Black,Gray,Black,Gray,Gray,Black,Gray,Gray },
                 { Black,Gray,Black,Gray,Black,Black,Black,Gray,Gray,Gray,Black },
                 { Black,Gray,Black,Black,Gray,Black,Black,Black,Gray,Black,Black },
                 { Black,Gray,Black,Black,Gray,Black,Black,Black,Gray,Black,Black },
                 { Black,Gray,Black,Black,Gray,Black,Black,Black,Gray,Black,Black },
                 { Black,Gray,Gray,Gray,Black,Black,Black,Black,Gray,Black,Black },
                 { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black }
             }
         );
        public static SpriteData word_JEON = new SpriteData(
             new char[,]{
                { ' ','■','■','■','■',' ','■','■','■','■','■' },
                { ' ',' ',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { ' ',' ',' ','■',' ',' ','■','■','■','■',' ' },
                { ' ','■',' ','■',' ',' ','■',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■','■','■','■','■' },
                { ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■','■',' ',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ','■',' ','■' },
                { '■',' ',' ',' ','■',' ','■',' ',' ','■','■' },
                { ' ','■','■','■',' ',' ','■',' ',' ',' ','■' }
             },
             new ConsoleColor[,]{
                { Black,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta,Black,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta },
                { Black,Black,Black,DarkMagenta,Black,Black,DarkMagenta,Black,Black,Black,Black },
                { Black,Black,Black,DarkMagenta,Black,Black,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta,Black },
                { Black,DarkMagenta,Black,DarkMagenta,Black,Black,DarkMagenta,Black,Black,Black,Black },
                { Black,DarkMagenta,DarkMagenta,DarkMagenta,Black,Black,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta,DarkMagenta },
                { Black,Black,Black,Black,Black,Black,Black,Black,Black,Black,Black },
                { Black,DarkMagenta,DarkMagenta,DarkMagenta,Black,Black,DarkMagenta,Black,Black,Black,DarkMagenta },
                { DarkMagenta,Black,Black,Black,DarkMagenta,Black,DarkMagenta,DarkMagenta,Black,Black,DarkMagenta },
                { DarkMagenta,Black,Black,Black,DarkMagenta,Black,DarkMagenta,Black,DarkMagenta,Black,DarkMagenta },
                { DarkMagenta,Black,Black,Black,DarkMagenta,Black,DarkMagenta,Black,Black,DarkMagenta,DarkMagenta },
                { Black,DarkMagenta,DarkMagenta,DarkMagenta,Black,Black,DarkMagenta,Black,Black,Black,DarkMagenta }
             }
         );
        //********************************************************//
        //**                         NOTE                       **//
        //**    sprite 추가시 반드시 checkIS blockA Switch 작성  **//
        //**      word 추가시 반드시 checkIS blockB Switch 작성  **//
        //**          하단에 Enum / Dictionary 도 작성!!         **//
        //********************************************************//

        // BLOCK SPRITE DATA END

        public static Dictionary<S_TYPE, SpriteData> sprite_Type = new Dictionary<S_TYPE, SpriteData>() {
            { S_TYPE.type_sprite_EMPTY, sprite_EMPTY },
            { S_TYPE.type_sprite_WALL, sprite_WALL },
            { S_TYPE.type_sprite_FLAG, sprite_FLAG },
            { S_TYPE.type_sprite_BABA_R, sprite_BABA_R },
            { S_TYPE.type_sprite_BABA_L, sprite_BABA_L },
            { S_TYPE.type_sprite_BABA_U, sprite_BABA_U },
            { S_TYPE.type_sprite_BABA_D, sprite_BABA_D },
            { S_TYPE.type_word_BABA, word_BABA },
            { S_TYPE.type_word_IS, word_IS },
            { S_TYPE.type_word_WIN, word_WIN },
            { S_TYPE.type_word_FLAG, word_FLAG },
            { S_TYPE.type_word_YOU, word_YOU },
            { S_TYPE.type_word_WALL, word_WALL },
            { S_TYPE.type_word_STOP, word_STOP },
            { S_TYPE.type_sprite_ROCK, sprite_ROCK },
            { S_TYPE.type_word_ROCK, word_ROCK },
            { S_TYPE.type_word_PUSH, word_PUSH },
            { S_TYPE.type_word_LAVA, word_LAVA },
            { S_TYPE.type_sprite_LAVA, sprite_LAVA },
            { S_TYPE.type_word_KILL, word_KILL },
            { S_TYPE.type_word_BONE, word_BONE },
            { S_TYPE.type_sprite_BONE, sprite_BONE },
            { S_TYPE.type_word_MOVE, word_MOVE },
            { S_TYPE.type_word_KEKE, word_KEKE },
            { S_TYPE.type_sprite_KEKE, sprite_KEKE },
            { S_TYPE.type_sprite_GRASS, sprite_GRASS },
            { S_TYPE.type_word_GRASS, word_GRASS },
            { S_TYPE.type_word_ICE, word_ICE },
            { S_TYPE.type_sprite_ICE, sprite_ICE },
            { S_TYPE.type_word_SLIP, word_SLIP },
            { S_TYPE.type_word_LOVE, word_LOVE },
            { S_TYPE.type_sprite_LOVE, sprite_LOVE },
            { S_TYPE.type_word_EMPTY, word_EMPTY },
            { S_TYPE.type_word_PLAY, word_PLAY },
            { S_TYPE.type_word_EXIT, word_EXIT },
            { S_TYPE.type_word_GAME, word_GAME },
            { S_TYPE.type_word_DONE, word_DONE },
            { S_TYPE.type_word_LOSE, word_LOSE },
            { S_TYPE.type_word_END, word_END }, 
            { S_TYPE.type_word_MADE, word_MADE }, 
            { S_TYPE.type_word_BY, word_BY }, 
            { S_TYPE.type_word_JEON, word_JEON }
        };
    }
    enum S_TYPE { // 순서 변경 금지. 추가만 할 것

        type_sprite,
        type_sprite_FLAG,
        type_sprite_WALL,
        type_sprite_ROCK,
        type_sprite_LAVA,
        type_sprite_BONE,
        type_sprite_KEKE,
        type_sprite_ICE,
        type_sprite_LOVE,
        type_sprite_GRASS,
        type_sprite_EMPTY,
        type_sprite_BABA_R,
        type_sprite_BABA_L,
        type_sprite_BABA_U,
        type_sprite_BABA_D,

        type_word,
        type_word_FLAG,
        type_word_WALL,
        type_word_ROCK,
        type_word_LAVA,
        type_word_BONE,
        type_word_KEKE,
        type_word_ICE,
        type_word_LOVE,
        type_word_GRASS,
        type_word_EMPTY,
        type_word_BABA,
        type_word_IS,
        type_word_WIN,
        type_word_YOU,
        type_word_STOP,
        type_word_PUSH,
        type_word_KILL,
        type_word_MOVE,
        type_word_SLIP,
        type_word_PLAY,
        type_word_EXIT,
        type_word_GAME,
        type_word_DONE,
        type_word_LOSE,
        type_word_END,
        type_word_MADE,
        type_word_JEON,
        type_word_BY
    }
}


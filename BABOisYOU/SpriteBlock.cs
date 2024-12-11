using System;
using System.Collections.Generic;

namespace BABOisYOU {
    enum P_TYPE {
        _isControl,
        _isTile,
        _isWord,
        _isPush,
        _isStop,
        _isWin,
        _isLose,
        _isSlip
    }
    class SpriteBlock : SpriteControl {
        public bool _isControl { get; private set; }
        public bool _isTile { get; private set; }
        public bool _isWord { get; private set; }
        public bool _isPush { get; private set; }
        public bool _isStop { get; private set; }
        public bool _isWin { get; private set; }
        public bool _isLose { get; private set; }
        public bool _isMove { get; private set; }
        public bool _isSlip { get; private set; }

        public static readonly Dictionary<P_TYPE, Func<SpriteBlock, bool>> propertyMap = new Dictionary<P_TYPE, Func<SpriteBlock, bool>> {
        { P_TYPE._isControl, block => block._isControl },
        { P_TYPE._isTile, block => block._isTile },
        { P_TYPE._isWord, block => block._isWord },
        { P_TYPE._isPush, block => block._isPush },
        { P_TYPE._isStop, block => block._isStop },
        { P_TYPE._isWin, block => block._isWin },
        { P_TYPE._isLose, block => block._isLose }
        };

        public void clearAll() {
            // Property Word 추가 될 때
            // set()    ->  checkIS() blockB switch{} 에
            // clear()  ->  clearAll() 에 적을 것
            clearIsCONTROL();
            clearIsSTOP();
            clearIsWIN();
            clearIsLOSE();
            clearIsMOVE();
            clearIsSLIP();
            if (!_isWord) clearIsPUSH();

        }
        public void setIsWord() { _isWord = true; }
        public void setIsTile() { _isTile = true; }
        public void setIsCONTROL() { _isControl = true; }
        public void clearIsCONTROL() { _isControl = false; }
        public void setIsSTOP() { _isStop = true; }
        public void clearIsSTOP() { _isStop = false; }
        public void setIsWIN() { _isWin = true; }
        public void clearIsWIN() { _isWin = false; }
        public void setIsPUSH() { _isPush = true; }
        public void clearIsPUSH() { _isPush = false; }
        public void setIsLOSE() { _isLose = true; }
        public void clearIsLOSE() { _isLose = false; }
        public void setIsMOVE() { _isMove = true; }
        public void clearIsMOVE() { _isMove = false; }
        public void setIsSLIP() { _isSlip = true; }
        public void clearIsSLIP() { _isSlip = false; }

        public SpriteBlock(S_TYPE type) {
            setType(type);
            setXY(30, 30);
            if (type > S_TYPE.type_word) {
                _isWord = true;
                _isPush = true;
            }
            else {
                _isTile = true;
            }
        }

        public SpriteBlock(SpriteBlock other) : base(other) {
            this._isControl = other._isControl;
            this._isTile = other._isTile;
            this._isWord = other._isWord;
            this._isPush = other._isPush;
            this._isStop = other._isStop;
            this._isWin = other._isWin;
            this._isLose = other._isLose;
            this._isMove = other._isMove;
            this._isSlip = other._isSlip;
        }

    }
}

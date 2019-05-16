using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Door : Structure {
        public bool Open { get; set; }
        public readonly bool Locked;

        public Door (Position pos, bool isLocked = false, bool isOpen = false) {
            XY = pos;
            Locked = isLocked;
            Open = isOpen;
            if (Open) {
                Character = ConsoleCharacter.Light;
                FloorToCeiling = false;
                Color = 4;
            } else {
                Character = ConsoleCharacter.BoxDrawingL_H;
                FloorToCeiling = true;
                Color = 8;
            }
            Immovable = true;
        }
    }
}
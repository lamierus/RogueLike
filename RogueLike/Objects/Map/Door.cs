using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Door : Structure {
        public bool Open { get; set; }

        public Door (Position pos, bool isOpen = false) {
            XY = pos;
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
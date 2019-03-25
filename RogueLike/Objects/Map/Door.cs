using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Door : Structure {
        public bool Open { get; set; }

        public Door() {
            MapCharacter = ConsoleCharacter.BoxDrawingL_H;
            Immovable = false;
            FloorToCeiling = true;
            Open = false;
        }
    }
}

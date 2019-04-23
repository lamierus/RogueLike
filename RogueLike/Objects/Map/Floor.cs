using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Floor:Structure {
        public Floor(int x, int y) {
            Color = 5;
            MapCharacter = ConsoleCharacter.Light;
            Immovable = true;
            FloorToCeiling = false;
            AboveOrBelow = false;
            XY.X = x;
            XY.Y = y;
        }
    }
}

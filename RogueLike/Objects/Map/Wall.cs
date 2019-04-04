using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Wall:Structure {

        public Wall(int x, int y) {
            MapCharacter = ConsoleCharacter.Full;
            Immovable = true;
            FloorToCeiling = true;
            XY.X = x;
            XY.Y = y;
        }
    }
}

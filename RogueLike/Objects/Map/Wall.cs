using ConsoleGameEngine;

namespace RogueLike {
    public class Wall : Structure {

        public Wall (int x, int y) {
            Color = 8;
            SolidCharacter = ConsoleCharacter.Full;
            Immovable = true;
            FloorToCeiling = true;
            XY.X = x;
            XY.Y = y;
        }
        public Wall (Position pos) {
            Color = 8;
            SolidCharacter = ConsoleCharacter.Full;
            Immovable = true;
            FloorToCeiling = true;
            XY = pos;
        }
    }
}
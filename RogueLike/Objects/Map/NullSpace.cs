using ConsoleGameEngine;

namespace RogueLike {
    public class NullSpace : Structure {
        public NullSpace (int x, int y) {
            Color = 0;
            SolidCharacter = ConsoleCharacter.Null;
            Immovable = true;
            FloorToCeiling = true;
            XY.X = x;
            XY.Y = y;
        }
        public NullSpace (Position pos) {
            Color = 0;
            SolidCharacter = ConsoleCharacter.Null;
            Immovable = true;
            FloorToCeiling = true;
            XY = pos;
        }
    }
}
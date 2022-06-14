using ConsoleGameEngine;
using System.Collections.Generic;

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

        public bool isCorner (ref FloorGrid floor){
            List<Object> adjacents = floor.GetAdjacentObjects(this);
            if (adjacents[(int)Directions.North].GetType() == adjacents[(int)Directions.South].GetType() ||
                adjacents[(int)Directions.East].GetType() == adjacents[(int)Directions.West].GetType())
                return false;
            return true;
        }
    }
}
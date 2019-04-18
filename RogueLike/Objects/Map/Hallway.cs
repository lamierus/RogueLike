using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public Position[] WallStarts = new Position[2];
        public Position[] WallEnds = new Position[2];
        public int WallColor {
            get { return 2; }
        }
        public ConsoleCharacter Wall {
            get { return ConsoleCharacter.Medium; }
        }
    }
}
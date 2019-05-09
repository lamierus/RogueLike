using System.Collections.Generic;

namespace RogueLike {
    public enum Directions {
        North,
        South,
        East,
        West
    }
    public class Direction {
        public static Position North = new Position (-1, 0);
        public static Position South = new Position (1, 0);
        public static Position East = new Position (0, 1);
        public static Position West = new Position (0, -1);
        public static List<Position> Cardinals = new List<Position> { North, South, East, West };
    }
}
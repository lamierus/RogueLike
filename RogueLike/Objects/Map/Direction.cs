using System.Collections.Generic;

namespace RogueLike {
    public struct Direction {
        public static Position North = new Position (-1, 0);
        public static Position East = new Position (0, 1);
        public static Position South = new Position (1, 0);
        public static Position West = new Position (0, -1);
        public static List<Position> Cardinals = new List<Position> { North, East, South, West };
        public static Position whichDirection(int direction){
            return Cardinals[direction];
        }

        public static Position whichDirection(Position lhs, Position rhs){
            
            int X = lhs.X - rhs.X;
            int Y = lhs.Y - rhs.Y;
            if (X == 0){
                return (Y > 0) ? North : South;
            }
            return (X < 0) ? East : West;
        }
    }
}
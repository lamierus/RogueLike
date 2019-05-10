using System;
using ConsoleGameEngine;

namespace RogueLike {
    /// <summary> A Vector containing two ints. </summary>
    public struct Position : IComparable<Position> {
        public int X { get; set; }
        public int Y { get; set; }
        public const float Rad2Deg = 180f / (float) Math.PI;
        public const float Deg2Rad = (float) Math.PI / 180f;

        /// <summary> new Position(0, 0); </summary>
        public static Position Zero { get; private set; } = new Position (0, 0);

        public Position (int x, int y) {
            X = x;
            Y = y;
        }

        public Vector ToVector () => new Vector ((float) X, (float) Y);
        public override string ToString () => String.Format ("({0}, {1})", X, Y);
        public Point ToPoint () => new Point (X, Y);

        public static Position operator + (Position lhs, Position rhs) {
            return new Position (lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
        public static Position operator - (Position lhs, Position rhs) {
            return new Position (lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
        public static Position operator / (Position lhs, float rhs) {
            return new Position ((int) (lhs.X / rhs), (int) (lhs.Y / rhs));
        }
        public static Position operator * (Position lhs, float rhs) {
            return new Position ((int) (lhs.X * rhs), (int) (lhs.Y * rhs));
        }
        public static Position operator + (Position lhs, int rhs) {
            return new Position (lhs.X + rhs, lhs.Y + rhs);
        }
        public static Position operator - (Position lhs, int rhs) {
            return new Position (lhs.X - rhs, lhs.Y - rhs);
        }
        public static bool operator == (Position lhs, Position rhs) {
            return ((lhs.X == rhs.X) && (lhs.Y == rhs.Y));
        }
        public static bool operator != (Position lhs, Position rhs) {
            return ((lhs.X != rhs.X) || (lhs.Y != rhs.Y));
        }
        public static bool operator < (Position lhs, Position rhs) {

            return (lhs.X < rhs.X || lhs.Y < rhs.Y);
        }
        public static bool operator > (Position lhs, Position rhs) {
            return (lhs.X > rhs.X || lhs.Y > rhs.Y);
        }
        public static bool operator <= (Position lhs, Position rhs) {
            return (lhs < rhs || lhs == rhs);
        }
        public static bool operator >= (Position lhs, Position rhs) {
            return (lhs > rhs || lhs == rhs);
        }
        // public static bool operator < (Position lhs, Position rhs) {
        //     return ((Distance (Zero, lhs) < (Distance (Zero, rhs))));
        // }
        // public static bool operator > (Position lhs, Position rhs) {
        //     return ((Distance (Zero, lhs) > (Distance (Zero, rhs))));
        // }
        // public static bool operator <= (Position lhs, Position rhs) {
        //     return ((Distance (Zero, lhs) <= (Distance (Zero, rhs))));
        // }
        // public static bool operator >= (Position lhs, Position rhs) {
        //     return ((Distance (Zero, lhs) >= (Distance (Zero, rhs))));
        // }

        public int CompareTo (Position that) {
            int result = (Position.Distance (Zero, this).CompareTo (Position.Distance (Zero, that)));
            if (result != 0) {
                return result;
            }
            return 0;
        }

        public override bool Equals (object obj) {
            if (!(obj is Position)) {
                return false;
            }
            return this == (Position) obj;
        }

        public override int GetHashCode () {
            return GetHashCode ();
        }

        /// <summary> Calculates distance between two points. </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns>Distance between A and B</returns>
        public static float Distance (Position a, Position b) {
            Position dV = b - a;
            float d = (float) Math.Sqrt (Math.Pow (dV.X, 2) + Math.Pow (dV.Y, 2));
            return d;
        }

        public void Clamp (Position min, Position max) {
            X = (X > max.X) ? max.X : X;
            X = (X < min.X) ? min.X : X;

            Y = (Y > max.Y) ? max.Y : Y;
            Y = (Y < min.Y) ? min.Y : Y;
        }

        public bool IsInLine (Position other) {
            return (X == other.X || Y == other.Y);
        }

        public bool IsAdjacentOrSame (Position other) {
            if ((this == other) || ((int) (Distance (this, other)) <= 1)) {
                return true;
            }
            return false;
        }
    }
}
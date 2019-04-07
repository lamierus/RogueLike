using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Rectangle : IComparable<Rectangle> {
        public Position TopLeft { get; private set; }
        public Position BottomRight { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int WallColor {
            get { return 2; }
        }
        public ConsoleCharacter Wall {
            get { return ConsoleCharacter.Medium; }
        }

        public Rectangle(int width, int height, int x, int y) {
            TopLeft = new Position(x, y);
            BottomRight = new Position(x + width, y + height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        //public override string ToString() => String.Format("({0}, {1})", X, Y);
        //public Point ToPoint() => new Point(X, Y);


        public static bool operator <(Rectangle lhs, Rectangle rhs) {
            return (lhs.X < rhs.X || lhs.Y < rhs.Y);
        }
        public static bool operator >(Rectangle lhs, Rectangle rhs) {
            return (lhs.X > rhs.X || lhs.Y > rhs.Y);
        }
        public static bool operator <=(Rectangle lhs, Rectangle rhs) {
            return (lhs.X <= rhs.X && lhs.Y <= lhs.Y);
        }
        public static bool operator >=(Rectangle lhs, Rectangle rhs) {
            return (lhs.X >= rhs.X && lhs.Y >= lhs.Y);
        }
        public static bool operator ==(Rectangle lhs, Rectangle rhs) {
            if ((object)lhs == null)
                return (object)lhs == null;
            if ((object)rhs == null)
                return false;
            return (((lhs.X == rhs.X) && (lhs.Y == rhs.Y)) && ((lhs.Width == rhs.Width) && (lhs.Height == rhs.Height)));
        }
        public static bool operator !=(Rectangle lhs, Rectangle rhs) {
            if ((object)lhs == null)
                return (object)lhs == null;
            if ((object)rhs == null)
                return false;
            return (((lhs.X != rhs.X) || (lhs.Y != rhs.Y)) || ((lhs.Width != rhs.Width) || (lhs.Height != rhs.Height)));
        }

        public int CompareTo(Rectangle that) {
            int result = (TopLeft.CompareTo(that.TopLeft));
            if (result != 0) {
                return result;
            }
            result = (BottomRight.CompareTo(that.BottomRight));
            if (result != 0) {
                return result;
            }
            return 0;
        }
        public override bool Equals(object obj) {
            return Equals(obj);
        }
        public override int GetHashCode() {
            return GetHashCode();
        }
    }
}

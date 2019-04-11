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

        public Rectangle (int width, int height, int x, int y) {
            TopLeft = new Position (x, y);
            BottomRight = new Position (x + width, y + height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle (Position topLeft, Position bottomRight) {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            X = topLeft.X;
            Y = topLeft.Y;
            Width = Math.Abs (topLeft.X - bottomRight.X);
            Height = Math.Abs (topLeft.Y - bottomRight.Y);
        }

        public bool CheckParallel (Rectangle other, out bool onXAxis) {
            Position[, ] Parallels = GetXParallels (other);
            onXAxis = true;
            if (Parallels == null) {
                Parallels = GetYParallels (other);
                onXAxis = false;
            }
            return Parallels != null;
        }

        public Position[, ] GetXParallels (Rectangle toCheck) {
            List<Position> thisParallels = new List<Position> ();
            List<Position> thatParallels = new List<Position> ();
            Position[, ] Parallels = null;

            for (int thisX = 0; thisX < Width; thisX++) {
                for (int thatX = 0; thatX < toCheck.Width; thatX++) {
                    if (X + thisX == toCheck.X + thatX) {
                        thisParallels.Add (new Position (X + thisX, Y + Height));
                        thatParallels.Add (new Position (toCheck.X + thatX, Y));
                    }
                }
            }
            if (thisParallels.Count > 0) {
                Parallels = new Position[2, thisParallels.Count];
                for (int i = 0; i < thisParallels.Count; i++) {
                    Parallels[0, i] = thisParallels[i];
                }
                for (int i = 0; i < thatParallels.Count; i++) {
                    Parallels[1, i] = thatParallels[i];
                }
            }
            // if (thisParallels.Count < 2) {
            //     thisParallels.Clear ();
            //     thatParallels.Clear ();
            // }
            return Parallels;
        }

        public Position[, ] GetYParallels (Rectangle toCheck) {
            List<Position> thisParallels = new List<Position> ();
            List<Position> thatParallels = new List<Position> ();
            Position[, ] Parallels = null;

            for (int thisY = 0; thisY < Height; thisY++) {
                for (int thatY = 0; thatY < toCheck.Height; thatY++) {
                    if (Y + thisY == toCheck.Y + thatY) {
                        thisParallels.Add (new Position (X + Width, Y + thisY));
                        thatParallels.Add (new Position (X, toCheck.Y + thatY));
                    }
                }
            }
            if (thisParallels.Count > 0) {
                Parallels = new Position[2, thisParallels.Count];
                for (int i = 0; i < thisParallels.Count; i++) {
                    Parallels[0, i] = thisParallels[i];
                }
                for (int i = 0; i < thatParallels.Count; i++) {
                    Parallels[1, i] = thatParallels[i];
                }
            }
            return Parallels;
        }

        public static bool operator < (Rectangle lhs, Rectangle rhs) {
            return (lhs.TopLeft < rhs.TopLeft);
        }
        public static bool operator > (Rectangle lhs, Rectangle rhs) {
            return (lhs.TopLeft > rhs.TopLeft);
        }
        public static bool operator <= (Rectangle lhs, Rectangle rhs) {
            return (lhs.TopLeft <= rhs.TopLeft);
        }
        public static bool operator >= (Rectangle lhs, Rectangle rhs) {
            return (lhs.TopLeft >= rhs.TopLeft);
        }
        public static bool operator == (Rectangle lhs, Rectangle rhs) {
            if ((object) lhs == null)
                return (object) lhs == null;
            if ((object) rhs == null)
                return false;
            return ((lhs.TopLeft == rhs.TopLeft) && (lhs.BottomRight == rhs.BottomRight));
        }
        public static bool operator != (Rectangle lhs, Rectangle rhs) {
            return !(lhs == rhs);
        }

        public int CompareTo (Rectangle that) {
            int result = (TopLeft.CompareTo (that.TopLeft));
            if (result != 0) {
                return result;
            }
            result = (BottomRight.CompareTo (that.BottomRight));
            if (result != 0) {
                return result;
            }
            return 0;
        }
        public override bool Equals (object obj) {
            return this == (Rectangle) obj;
        }
        public override int GetHashCode () {
            return (int) (Position.Distance (TopLeft, BottomRight));
        }
    }
}
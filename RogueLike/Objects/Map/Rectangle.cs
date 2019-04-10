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

        public bool isParallelTo (Rectangle toCheck, out Position topLeft, out Position bottomRight) {
            bool isParallel = false;
            List<Position> thisParallels = new List<Position> ();
            List<Position> thatParallels = new List<Position> ();

            for (int thisX = 0; 0 < Width; thisX++) {
                for (int thatX = 0; 0 < toCheck.Width; thatX++) {
                    if (X + thisX == toCheck.X + thatX) {
                        thisParallels.Add (new Position (X + thisX, Y));
                        thatParallels.Add (new Position (X + thatX, Y));
                        isParallel = true;
                    }
                }
            }
            if (thisParallels.Count == 0) {
                for (int thisY = 0; 0 < Height; thisY++) {
                    for (int thatY = 0; 0 < toCheck.Height; thatY++) {
                        if (Y + thisY == toCheck.Y + thatY) {
                            thisParallels.Add (new Position (X, Y + thisY));
                            thatParallels.Add (new Position (X, Y + thatY));
                            isParallel = true;
                        }
                    }
                }
                
            } else {

            }

            return isParallel;

            public static bool operator < (Rectangle lhs, Rectangle rhs) {
                return (lhs.X < rhs.X || lhs.Y < rhs.Y);
            }
            public static bool operator > (Rectangle lhs, Rectangle rhs) {
                return (lhs.X > rhs.X || lhs.Y > rhs.Y);
            }
            public static bool operator <= (Rectangle lhs, Rectangle rhs) {
                return (lhs.X <= rhs.X && lhs.Y <= lhs.Y);
            }
            public static bool operator >= (Rectangle lhs, Rectangle rhs) {
                return (lhs.X >= rhs.X && lhs.Y >= lhs.Y);
            }
            public static bool operator == (Rectangle lhs, Rectangle rhs) {
                if ((object) lhs == null)
                    return (object) lhs == null;
                if ((object) rhs == null)
                    return false;
                return ((lhs.X == rhs.X) && (lhs.Y == rhs.Y) && (lhs.Width == rhs.Width) && (lhs.Height == rhs.Height));
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
                return Equals (obj);
            }
            public override int GetHashCode () {
                return GetHashCode ();
            }
        }
    }
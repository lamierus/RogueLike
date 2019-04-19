using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Parallel {
        public Position First { get; set; }
        public Position Second { get; set; }

        public Parallel (Position first, Position second) {
            First = first;
            Second = second;
        }
    }
    public class Room : IComparable<Room> {
        public Position TopLeft { get; private set; }
        public Position BottomRight { get; private set; }
        public List<Parallel> NegXParallels = new List<Parallel> ();
        public List<Parallel> NegYParallels = new List<Parallel> ();
        public List<Parallel> PosXParallels = new List<Parallel> ();
        public List<Parallel> PosYParallels = new List<Parallel> ();
        public Room NegXConnectedRoom,
        NegYConnectedRoom,
        PosXConnectedRoom,
        PosYConnectedRoom;
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
        public bool AllSidesConnected {
            get {
                return (NegXConnectedRoom != null && NegYConnectedRoom != null &&
                    PosXConnectedRoom != null && PosYConnectedRoom != null);
            }
        }

        public Room (int width, int height, int x, int y) {
            TopLeft = new Position (x, y);
            BottomRight = new Position (x + width, y + height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Room (Position topLeft, Position bottomRight) {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            X = topLeft.X;
            Y = topLeft.Y;
            Width = Math.Abs (topLeft.X - bottomRight.X);
            Height = Math.Abs (topLeft.Y - bottomRight.Y);
        }

        public bool CheckXParallel (Room other) {
            if (other == null) {
                return false;
            }
            GetXAxisParallels (other);
            return (PosXParallels.Count == 0 && NegXParallels.Count == 0);
        }

        private void GetXAxisParallels (Room toCheck) {
            for (int thisY = 0; thisY < Height; thisY++) {
                for (int thatY = 0; thatY < toCheck.Height; thatY++) {
                    if (Y + thisY == toCheck.Y + thatY) {
                        if (X < toCheck.X) {
                            PosXParallels.Add (new Parallel (new Position (X + Width, Y + thisY), new Position (toCheck.X, toCheck.Y + thatY)));

                        } else {
                            NegXParallels.Add (new Parallel (new Position (X, Y + thisY), new Position (toCheck.X + toCheck.Width, toCheck.Y + thatY)));
                        }
                    }
                }
            }
        }

        public bool CheckYParallel (Room other) {
            if (other == null) {
                return false;
            }
            GetYAxisParallels (other);
            return (PosYParallels.Count == 0 && NegYParallels.Count == 0);
        }

        private void GetYAxisParallels (Room toCheck) {
            for (int thisX = 0; thisX < Width; thisX++) {
                for (int thatX = 0; thatX < toCheck.Width; thatX++) {
                    if (X + thisX == toCheck.X + thatX) {
                        if (Y < toCheck.Y) {
                            NegYParallels.Add (new Parallel (new Position (X + thisX, Y + Height), new Position (toCheck.X + thatX, toCheck.Y)));
                        } else {
                            PosYParallels.Add (new Parallel (new Position (X + thisX, Y), new Position (toCheck.X + thatX, toCheck.Y + toCheck.Height)));
                        }
                    }
                }
            }
        }

        public static bool operator < (Room lhs, Room rhs) {
            return (lhs.TopLeft < rhs.TopLeft);
        }
        public static bool operator > (Room lhs, Room rhs) {
            return (lhs.TopLeft > rhs.TopLeft);
        }
        public static bool operator <= (Room lhs, Room rhs) {
            return (lhs.TopLeft <= rhs.TopLeft);
        }
        public static bool operator >= (Room lhs, Room rhs) {
            return (lhs.TopLeft >= rhs.TopLeft);
        }

        public static bool operator == (Room lhs, Room rhs) {
            // if (lhs != null && rhs != null) {
            //     return ((lhs.TopLeft == rhs.TopLeft) && (lhs.BottomRight == rhs.BottomRight));
            // } else {
            //     return false;
            // }
            // Check for null
            if (Object.ReferenceEquals (lhs, null)) {
                if (Object.ReferenceEquals (rhs, null)) {
                    return true;
                }
                return false;
            }
            return lhs.Equals (rhs);
        }

        public static bool operator != (Room lhs, Room rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals (object obj) {
            return this.Equals (obj as Room);
        }

        public bool Equals (Room rhs) {
            // If parameter is null, return false.
            if (Object.ReferenceEquals (rhs, null)) {
                return false;
            }
            // Optimization for a common success case.
            if (Object.ReferenceEquals (this, rhs)) {
                return true;
            }
            // If run-time types are not exactly the same, return false.
            if (this.GetType () != rhs.GetType ()) {
                return false;
            }
            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (TopLeft == rhs.TopLeft) && (BottomRight == rhs.BottomRight);
        }

        public int CompareTo (Room that) {
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

        public override int GetHashCode () {
            return (int) (Position.Distance (TopLeft, BottomRight));
        }
    }
}
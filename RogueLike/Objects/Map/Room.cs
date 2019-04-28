using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public Position Start { get; set; }
        public Position End { get; set; }

        public Hallway (Position start, Position end) {
            Start = start;
            End = end;
        }
        public int Color {
            get { return GetMapObject ().Color; }
        }
        public ConsoleCharacter Character {
            get { return GetMapObject ().Character; }
        }
        public Floor GetMapObject (int x, int y) {
            return new Floor (x, y);
        }
        private Floor GetMapObject () {
            return new Floor (0, 0);
        }
    }
    public class Room : IComparable<Room> {
        public Position TopLeft { get; private set; }
        public Position BottomRight { get; private set; }
        public List<Hallway> Parallels { get; private set; }
        public List<Room> ConnectedRooms = new List<Room> ();
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Color {
            get { return 1; }
        }
        public ConsoleCharacter Character {
            get { return ConsoleCharacter.Light; }
        }
        public Floor GetMapObject (int x, int y) {
            return new Floor (x, y);
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
            if (Parallels != null) {
                ConnectedRooms.Add (other);
                other.ConnectedRooms.Add (this);
            }
            return Parallels != null;
        }

        private void GetXAxisParallels (Room toCheck) {
            for (int thisY = 0; thisY < Height; thisY++) {
                for (int thatY = 0; thatY < toCheck.Height; thatY++) {
                    if (Y + thisY == toCheck.Y + thatY) {
                        if (X < toCheck.X) {
                            if (Parallels == null) {
                                Parallels = new List<Hallway> ();
                            }
                            Parallels.Add (new Hallway (new Position (X + Width, Y + thisY), new Position (toCheck.X, toCheck.Y + thatY)));
                        } else {
                            if (Parallels == null) {
                                Parallels = new List<Hallway> ();
                            }
                            Parallels.Add (new Hallway (new Position (X, Y + thisY), new Position (toCheck.X + toCheck.Width, toCheck.Y + thatY)));
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
            if (Parallels != null) {
                ConnectedRooms.Add (other);
                other.ConnectedRooms.Add (this);
            }
            return Parallels != null;
        }

        private void GetYAxisParallels (Room toCheck) {
            for (int thisX = 0; thisX < Width; thisX++) {
                for (int thatX = 0; thatX < toCheck.Width; thatX++) {
                    if (X + thisX == toCheck.X + thatX) {
                        if (Y < toCheck.Y) {
                            if (Parallels == null) {
                                Parallels = new List<Hallway> ();
                            }
                            Parallels.Add (new Hallway (new Position (X + thisX, Y + Height), new Position (toCheck.X + thatX, toCheck.Y)));
                        } else {
                            if (Parallels == null) {
                                Parallels = new List<Hallway> ();
                            }
                            Parallels.Add (new Hallway (new Position (X + thisX, Y), new Position (toCheck.X + thatX, toCheck.Y + toCheck.Height)));
                        }
                    }
                }
            }
        }

        public void ClearParallels () {
            Parallels = null;
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
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public List<Position> Hall { get; private set; } = new List<Position> ();
        public Hallway (Position start, Position end) {
            for (int i = 0; i <= (int) Position.Distance (start, end); i++) {
                if (start.X == end.X) {
                    if (start.Y < end.Y) {
                        Hall.Add (new Position (start.X, start.Y + i));
                    } else {
                        Hall.Add (new Position (start.X, start.Y - i));
                    }
                } else {
                    if (start.X < end.X) {
                        Hall.Add (new Position (start.X + i, start.Y));
                    } else {
                        Hall.Add (new Position (start.X - i, start.Y));
                    }
                }
            }
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
        public bool CheckForAdjacent (List<Hallway> hallsToCheck) {
            return false;
        }
    }
    public class Room : IComparable<Room> {
        public Position TopLeft { get; private set; }
        public Position BottomRight { get; private set; }
        public List<List<Hallway>> Parallels { get; private set; }
        public List<Room> ConnectedRooms { get; set; } = new List<Room> ();
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
            return (Parallels != null && Parallels.Count > ConnectedRooms.Count);
        }

        private void GetXAxisParallels (Room toCheck) {
            List<Hallway> parallels = null;
            for (int thisY = 0; thisY < Height; thisY++) {
                for (int thatY = 0; thatY < toCheck.Height; thatY++) {
                    if (Y + thisY == toCheck.Y + thatY) {
                        if (Parallels == null) {
                            Parallels = new List<List<Hallway>> ();
                        }
                        parallels = new List<Hallway> ();
                        if (X < toCheck.X) {
                            parallels.Add (new Hallway (new Position (X + Width, Y + thisY), new Position (toCheck.X, toCheck.Y + thatY)));
                        } else {
                            parallels.Add (new Hallway (new Position (X, Y + thisY), new Position (toCheck.X + toCheck.Width, toCheck.Y + thatY)));
                        }
                    }
                }
            }
            if (parallels != null) {
                Parallels.Add (parallels);
            }
        }

        public bool CheckYParallel (Room other) {
            if (other == null) {
                return false;
            }
            GetYAxisParallels (other);
            return (Parallels != null && Parallels.Count > ConnectedRooms.Count);
        }

        private void GetYAxisParallels (Room toCheck) {
            // List<Hallway> parallels = null;
            // for (int thisX = 0; thisX < Width; thisX++) {
            //     for (int thatX = 0; thatX < toCheck.Width; thatX++) {
            //         if (X + thisX == toCheck.X + thatX) {
            //             if (Parallels == null) {
            //                 Parallels = new List<List<Hallway>> ();
            //             }
            //             parallels = new List<Hallway> ();
            //             if (Y < toCheck.Y) {
            //                 parallels.Add (new Hallway (new Position (X + thisX, Y + Height), new Position (toCheck.X + thatX, toCheck.Y)));
            //             } else {
            //                 parallels.Add (new Hallway (new Position (X + thisX, Y), new Position (toCheck.X + thatX, toCheck.Y + toCheck.Height)));
            //             }
            //         }
            //     }
            // }
            // if (parallels != null) {
            //     Parallels.Add (parallels);
            // }
        }

        public void ClearParallels () {
            Parallels = null;
        }

        private bool IsIntersectedBy (Room roomToCheck, Hallway hallToCheck) {
            return hallToCheck.Hall.Exists (point => (point > roomToCheck.TopLeft && point < roomToCheck.BottomRight));
        }

        public bool HallsIntersect (Room toCheck) {
            foreach (List<Hallway> halls in Parallels) {
                foreach (Hallway H in halls) {
                    if (IsIntersectedBy (toCheck, H)) {
                        return true;
                    }
                }
            }
            return false;
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
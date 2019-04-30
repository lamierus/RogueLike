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
            return (Parallels != null && ConnectedRooms.Count > Parallels.Count);
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
            return (Parallels != null && ConnectedRooms.Count > Parallels.Count);
        }

        private void GetYAxisParallels (Room toCheck) {
            List<Hallway> parallels = null;
            for (int thisX = 0; thisX < Width; thisX++) {
                for (int thatX = 0; thatX < toCheck.Width; thatX++) {
                    if (X + thisX == toCheck.X + thatX) {
                        if (Parallels == null) {
                            Parallels = new List<List<Hallway>> ();
                        }
                        parallels = new List<Hallway> ();
                        if (Y < toCheck.Y) {
                            parallels.Add (new Hallway (new Position (X + thisX, Y + Height), new Position (toCheck.X + thatX, toCheck.Y)));
                        } else {
                            parallels.Add (new Hallway (new Position (X + thisX, Y), new Position (toCheck.X + thatX, toCheck.Y + toCheck.Height)));
                        }
                    }
                }
            }
            if (parallels != null) {
                Parallels.Add (parallels);
            }
        }

        public void ClearParallels () {
            Parallels = null;
        }

        public bool IsIntersectedBy (Hallway toCheck) {
            List<Position> pointsInHall = new List<Position> ();
            for (int i = 0; i < (int) Position.Distance (toCheck.Start, toCheck.End); i++) {
                if (toCheck.Start.X == toCheck.End.X) {
                    pointsInHall.Add (new Position (toCheck.Start.X, toCheck.Start.Y + i));
                } else {
                    pointsInHall.Add (new Position (toCheck.Start.X + i, toCheck.Start.Y));
                }
                // if (toCheck.Start.X == toCheck.End.X) {
                //     if (toCheck.Start.Y < toCheck.End.Y) {
                //         if (((toCheck.Start.Y + i) >= Y && (toCheck.Start.Y + i) <= (Y + Height)) &&
                //             (toCheck.Start.X >= X && toCheck.Start.X <= (X + Width))) {
                //             return true;
                //         }
                //     } else {
                //         if (((toCheck.Start.Y - i) >= Y && (toCheck.Start.Y - i) <= (Y + Height)) &&
                //             (toCheck.Start.X >= X && toCheck.Start.X <= (X + Width))) {
                //             return true;
                //         }
                //     }
                // } else {
                //     if (toCheck.Start.X < toCheck.End.X) {
                //         if ((toCheck.Start.Y >= Y && toCheck.Start.Y <= (Y + Height) &&
                //             ((toCheck.Start.X + i) >= X && (toCheck.Start.X + i) <= (X + Width)))) {
                //             return true;
                //         }
                //     } else {
                //         if ((toCheck.Start.Y >= Y && toCheck.Start.Y <= (Y + Height)) &&
                //             ((toCheck.Start.X - i) >= X && (toCheck.Start.X - i) <= (X + Width))) {
                //             return true;
                //         }
                //     }
                // }
            }
            return pointsInHall.Exists (point => (point > TopLeft && point < BottomRight));
        }

        public bool IsIntersectedByAnyOF (List<Hallway> toCheck) {
            foreach (Hallway H in toCheck) {
                if (IsIntersectedBy (H)) {
                    return true;
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
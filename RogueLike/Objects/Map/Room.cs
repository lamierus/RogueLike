using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Room : IComparable<Room> {
        public List<Position> Rectangle { get; private set; } = new List<Position> ();
        public Position TopLeft { get; private set; }
        public Position BottomRight { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public Position XY { get; private set; }
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
        public List<Position> Doors { get; set; } = new List<Position>();

        public Room (int width, int height, int x, int y) {
            TopLeft = new Position (x, y);
            XY = TopLeft;
            BottomRight = new Position (x + width, y + height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BuildRect ();
        }

        public Room (Position topLeft, Position bottomRight) {
            TopLeft = topLeft;
            XY = topLeft;
            BottomRight = bottomRight;
            X = topLeft.X;
            Y = topLeft.Y;
            Width = Math.Abs (topLeft.X - bottomRight.X);
            Height = Math.Abs (topLeft.Y - bottomRight.Y);
            BuildRect ();
        }

        private void BuildRect () {
            // Rect = new Position[Width][];
            for (int x = 0; x <= Width; x++) {
                // Rect[x] = new Position[Height];
                for (int y = 0; y <= Height; y++) {
                    // Rect[x][y] = new Position (X + x, Y + y);
                    Rectangle.Add (new Position (X + x, Y + y));
                }
            }
        }

        public List<Position> GetRoomWall (int direction){
            List<Position> Wall = new List<Position>();
            switch (direction){
                case 0: // North
                    Wall = Rectangle.FindAll(point => point.IsInLineX(TopLeft));
                    break;
                case 1: // East
                    Wall = Rectangle.FindAll(point => point.IsInLineY(BottomRight));
                    break;
                case 2: // South
                    Wall = Rectangle.FindAll(point => point.IsInLineX(BottomRight));
                    break;
                case 3: // West
                    Wall = Rectangle.FindAll(point => point.IsInLineY(TopLeft));
                    break;
            }
            return Wall;
        }
        
        public bool IsInBounds (Position pos) {
            return (Rectangle.Contains (pos) || Rectangle.Contains (pos));
        }

        public bool IsInRoom (Position pos) {
            return (IsInBounds (pos) && !(TopLeft.IsInLineX (pos) || TopLeft.IsInLineY (pos) ||
                                            BottomRight.IsInLineX (pos) || BottomRight.IsInLineY (pos)));
        }

        public bool IsIntersectedBy (Room other) {
            return other.Rectangle.Exists (pt => IsInBounds (pt));
        }

        public Position Center(){
            Position center = new Position(TopLeft.X + (Width/2), TopLeft.Y + (Height/2));
            return center;
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

        public bool Equals (Room toCompare) {
            // If parameter is null, return false.
            if (Object.ReferenceEquals (toCompare, null)) {
                return false;
            }
            // Optimization for a common success case.
            if (Object.ReferenceEquals (this, toCompare)) {
                return true;
            }
            // If run-time types are not exactly the same, return false.
            if (this.GetType () != toCompare.GetType ()) {
                return false;
            }
            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (TopLeft == toCompare.TopLeft) && (BottomRight == toCompare.BottomRight);
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
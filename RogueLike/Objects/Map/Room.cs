﻿using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Room : IComparable<Room> {
        // public List<List<Hallway>> Parallels { get; private set; }
        // public List<Room> ConnectedRooms { get; set; } = new List<Room> ();
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
        
        public bool IsInBounds (Position pos) {
            return (Rectangle.Contains (pos) || Rectangle.Contains (pos));
        }

        public bool IsInRoom (Position pos) {
            return (IsInBounds (pos) && !(TopLeft.IsInLine (pos) || BottomRight.IsInLine (pos)));
        }

        public bool IsIntersectedBy (Room other) {
            return other.Rectangle.Exists (pt => IsInBounds (pt));
        }

        private bool IsIntersectedBy (Hallway hallToCheck) {
            return hallToCheck.Hall.Exists (pt => IsInBounds (pt));
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
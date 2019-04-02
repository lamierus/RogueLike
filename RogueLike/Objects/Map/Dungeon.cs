using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Rectangle{
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        /*public int Top { get; private set; }
        public int Left { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }*/
        public int WallColor{ 
            get {return 2;}
        }
        public ConsoleCharacter Wall{
            get {return ConsoleCharacter.Medium;}
        }

        public Rectangle(int width, int height, int x, int y) {
            TopLeft = new Point(x, y);
            BottomRight = new Point(x + width, y + height);
            /*Top = top;
            Left = left;
            Width = width;
            Height = height;*/
        }
    }

    public class Dungeon {
        private const int c_MinWidth = 16;
        private const int c_MinHeight = 8;
        private Random Rand = new Random();
        private int Width, Height, X, Y;
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Rectangle Room { get; private set; }

        public Dungeon(int width, int height, int x, int y) {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        public bool Split() {
            if (LeftBranch != null || RightBranch != null) {
                return false;
            }
            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            //int max = ((vertical) ? Width : Height) - c_MinSize; //find the maximum height/width
            int maxWidth = Width - c_MinWidth;
            int maxHeight = Height - c_MinHeight;
            if (maxWidth <= c_MinWidth || maxHeight <= c_MinHeight) {
                return false;
            }

            int splitPoint;
            if (vertical) {
                splitPoint = Rand.Next(maxWidth);
                if (splitPoint < c_MinWidth) {  // adjust split point so there's at least c_MinSize in both partitions
                    splitPoint = c_MinWidth;
                }
                LeftBranch = new Dungeon(splitPoint, Height, X, Y);
                RightBranch = new Dungeon(Width - splitPoint, Height, X + splitPoint, Y);
            } else {
                splitPoint = Rand.Next(maxHeight);
                if (splitPoint < c_MinHeight) {  // adjust split point so there's at least c_MinSize in both partitions
                    splitPoint = c_MinHeight;
                }
                LeftBranch = new Dungeon(Width, splitPoint, X, Y);
                RightBranch = new Dungeon(Width, Height - splitPoint, X, Y + splitPoint);
            }
            return true;
        }

        public void GenerateRooms(ref List<Rectangle> rooms) {
            if (LeftBranch != null || RightBranch != null) {
                if (LeftBranch != null) {
                    LeftBranch.GenerateRooms(ref rooms);
                }
                if (RightBranch != null) {
                    RightBranch.GenerateRooms(ref rooms);
                }
            } else if (Room == null){
                int roomXOffset = (Width - c_MinWidth <= 0) ? 0 : Rand.Next(Width - c_MinWidth);
                int roomYOffset = (Height - c_MinHeight <= 0) ? 0 : Rand.Next(Height - c_MinHeight);
                int roomWidth = Math.Max(Rand.Next(Width - roomXOffset), c_MinWidth);
                int roomHeight = Math.Max(Rand.Next(Height - roomYOffset), c_MinHeight);
                Room = new Rectangle(roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                rooms.Add(Room);
            }
            //TODO: add connections between the branches
        }
    }
}

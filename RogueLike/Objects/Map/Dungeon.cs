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

        public Rectangle(int width, int height, int top, int left) {
            TopLeft = new Point(top, left);
            BottomRight = new Point(top + width, left + height);
            /*Top = top;
            Left = left;
            Width = width;
            Height = height;*/
        }
    }

    public class Dungeon {
        private const int c_MinSize = 4;
        private Random Rand = new Random();
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        private int Width, Height, Top, Left;
        //private Position TopLeft;
        public Rectangle Room { get; private set; }

        public Dungeon(int width, int height, int top, int left) {
            Width = width;
            Height = height;
            Top = top;
            Left = left;
            //TopLeft = new Position(top, left);
        }

        public bool Split() {
            if (LeftBranch != null) {
                return false;
            }
            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            int max = ((vertical) ? Width : Height) - c_MinSize; //find the maximum height/width
            if (max <= c_MinSize) {
                return false;
            }
            int splitPoint = Rand.Next(max);
            if (splitPoint < c_MinSize)  // adjust split point so there's at least c_MinSize in both partitions
                splitPoint = c_MinSize;
            if (vertical) {
                LeftBranch = new Dungeon(splitPoint, Height, Top, Left);
                RightBranch = new Dungeon(Width - splitPoint, Height, Top, Left + splitPoint);
            } else {
                LeftBranch = new Dungeon(Width, splitPoint, Top, Left);
                RightBranch = new Dungeon(Width, Height - splitPoint, Top + splitPoint, Left);
            }
            return true;
        }

        public void GenerateRooms(ref List<Rectangle> rooms) {
            if (LeftBranch != null) {
                LeftBranch.GenerateRooms(ref rooms);
                RightBranch.GenerateRooms(ref rooms);
            } else {
                int roomTopOffset = (Height - c_MinSize <= 0) ? 0 : Rand.Next(Height - c_MinSize);
                int roomLeftOffset = (Width - c_MinSize <= 0) ? 0 : Rand.Next(Width - c_MinSize);
                int roomWidth = Math.Max(Rand.Next(Width - roomLeftOffset), c_MinSize);
                int roomHeight = Math.Max(Rand.Next(Height - roomTopOffset), c_MinSize);
                Room = new Rectangle(roomWidth, roomHeight, Top + roomTopOffset, Left + roomLeftOffset);
                rooms.Add(Room);
                //rooms.Add(new Rectangle(roomWidth, roomHeight, Top + roomTop, Left + roomLeft));
            }
            //TODO: add connections between the branches
        }
    }
}

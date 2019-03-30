using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Rectangle{
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public int WallColor{ 
            get {return 2;}
        }
        public ConsoleCharacter Wall{
            get {return ConsoleCharacter.Medium;}
        }

        public Rectangle(int width, int height, int top, int left) {
            TopLeft = new Point(top, left);
            BottomRight = new Point(top + height, left + width);
        }
    }

    public class Dungeon {
        private const int c_MinSize = 10;
        private Random Rand = new Random();
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        private int Width, Height;
        private Position TopLeft;
        public Rectangle Room { get; private set; }

        public Dungeon(int width, int height, int top, int left) {
            Width = width;
            Height = height;
            TopLeft = new Position(top, left);
        }

        public bool Split() {
            if (LeftBranch != null) {
                return false;
            }
            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            int max = ((vertical) ? Height : Width) - c_MinSize; //find the maximum height/width
            if (max <= c_MinSize) {
                return false;
            }
            int splitPoint = Rand.Next(max);
            if (splitPoint < c_MinSize)  // adjust split point so there's at least c_MinSize in both partitions
                splitPoint = c_MinSize;
            if (vertical) {
                LeftBranch = new Dungeon(Width - splitPoint, Height, TopLeft.X, TopLeft.Y);
                RightBranch = new Dungeon(Width - splitPoint, Height, TopLeft.X + splitPoint, TopLeft.Y);
            } else {
                LeftBranch = new Dungeon(Width, Height - splitPoint, TopLeft.X, TopLeft.Y);
                RightBranch = new Dungeon(Width, Height - splitPoint, TopLeft.X + splitPoint, TopLeft.Y);
            }
            return true;
        }

        public void GenerateRoom() {
            if (LeftBranch != null) {
                LeftBranch.GenerateRoom();
                RightBranch.GenerateRoom();
            } else {
                int roomTop = (Height - c_MinSize <= 0) ? 0 : Rand.Next(Height - c_MinSize);
                int roomLeft = (Width - c_MinSize <= 0) ? 0 : Rand.Next(Width - c_MinSize);
                int roomWidth = Math.Max(Rand.Next(Width - roomLeft), c_MinSize);
                int roomHeight = Math.Max(Rand.Next(Height - roomTop), c_MinSize);
                Room = new Rectangle(roomWidth, roomHeight, roomTop, roomLeft);
            }
            //TODO: add connections between the branches
        }
    }
}

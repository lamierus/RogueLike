using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Rectangle{
        public Point TopLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int WallColor{ 
            get {return 2;}
        }
        public ConsoleCharacter Wall{
            get {return ConsoleCharacter.Medium;}
        }

        public Rectangle(int width, int height, int x, int y) {
            TopLeft = new Point(x, y);
            BottomRight = new Point(x + width, y + height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void OffsetRectangle(int x, int y) {
            X += x;
            TopLeft = new Point(TopLeft.X + x, TopLeft.Y + y);
            Y += y;
            BottomRight = new Point(BottomRight.X + x, BottomRight.Y + y);
        }
    }

    public class Dungeon {
        private Random Rand = new Random();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight;
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Rectangle Room { get; private set; }

        public Dungeon(int fullWidth, int fullHeight, int x, int y, int minWidth = 12, int minHeight = 8) {
            Width = fullWidth;
            Height = fullHeight;
            X = x;
            Y = y;
            MinWidth = minWidth;
            MinHeight = minHeight;
        }

        public bool Split() {
            if (LeftBranch != null || RightBranch != null) {
                return false;
            }
            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            //int max = ((vertical) ? Width : Height) - c_MinSize; //find the maximum height/width
            int maxWidth = Width - MinWidth;
            int maxHeight = Height - MinHeight;
            if (maxWidth <= MinWidth || maxHeight <= MinHeight) {
                return false;
            }

            int minWidth = MinWidth;//(int)Math.Ceiling(MinWidth * .50);
            int minHeight = MinHeight;//Math.Max((int)Math.Ceiling(MinHeight * .45), 4);

            int splitPoint;
            if (vertical) {
                splitPoint = Rand.Next(maxWidth);
                if (splitPoint < MinWidth) {  // adjust split point so there's at least c_MinSize in both partitions
                    splitPoint = MinWidth;
                }
                LeftBranch = new Dungeon(splitPoint, Height, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon(Width - splitPoint, Height, X + splitPoint, Y, minWidth, minHeight);
            } else {
                splitPoint = Rand.Next(maxHeight);
                if (splitPoint < MinHeight) {  // adjust split point so there's at least c_MinSize in both partitions
                    splitPoint = MinHeight;
                }
                LeftBranch = new Dungeon(Width, splitPoint, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon(Width, Height - splitPoint, X, Y + splitPoint, minWidth, minHeight);
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
                int roomXOffset = (Width - MinWidth <= 0) ? 0 : Rand.Next(Width - MinWidth);
                int roomYOffset = (Height - MinHeight <= 0) ? 0 : Rand.Next(Height - MinHeight);
                int roomWidth = Math.Max(Rand.Next(Width - roomXOffset), MinWidth);
                int roomHeight = Math.Max(Rand.Next(Height - roomYOffset), MinHeight);
                Room = new Rectangle(roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                rooms.Add(Room);
            }
            //TODO: add connections between the branches
        }
    }
}

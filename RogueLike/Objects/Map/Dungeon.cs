using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight;
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Rectangle Room { get; private set; }

        // constructor with minimums built into it, so they can be provided, if you want
        public Dungeon(int fullWidth, int fullHeight, int x, int y, int minWidth = 16, int minHeight = 8) {
            Width = fullWidth;
            Height = fullHeight;
            X = x;
            Y = y;
            MinWidth = minWidth;
            MinHeight = minHeight;
        }

        public bool Split() {
            //bail if the branches aren't null, as it's already split
            if (LeftBranch != null || RightBranch != null) {
                return false;
            }
            //
            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            //find the maximum height/width
            //int max = ((vertical) ? Width : Height) - c_MinSize;
            int maxWidth = Width - MinWidth;
            int maxHeight = Height - MinHeight;
            //bail if the maximum is smaller than or equal to the minimum
            if (maxWidth <= MinWidth || maxHeight <= MinHeight) {
                return false;
            }

            //I was going to use this to randomize the sizes of the nodes even more.  Maybe I'll come back to this, sometime
            int minWidth = MinWidth;//(int)Math.Ceiling(MinWidth * .50);
            int minHeight = MinHeight;//Math.Max((int)Math.Ceiling(MinHeight * .45), 4);

            int splitPoint;
            if (vertical) { //vertical split
                splitPoint = Rand.Next(maxWidth);
                // adjust split point so there's at least c_MinSize in both partitions
                if (splitPoint < MinWidth) {
                    splitPoint = MinWidth;
                }
                LeftBranch = new Dungeon(splitPoint, Height, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon(Width - splitPoint, Height, X + splitPoint, Y, minWidth, minHeight);
            } else { //horizontal split
                splitPoint = Rand.Next(maxHeight);
                // adjust split point so there's at least c_MinSize in both partitions
                if (splitPoint < MinHeight) {
                    splitPoint = MinHeight;
                }
                LeftBranch = new Dungeon(Width, splitPoint, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon(Width, Height - splitPoint, X, Y + splitPoint, minWidth, minHeight);
            }
            return true;
        }

        /// <summary>
        ///     iterate through each of the nodes and generate rooms at the bottom of the branch
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="halls"></param>
        public void GenerateRooms(ref List<Rectangle> rooms) {
            //if neither of the  branches are null, then we'll go into here and attempt to generate rooms
            if (LeftBranch != null || RightBranch != null) {
                LeftBranch.GenerateRooms(ref rooms);
                RightBranch.GenerateRooms(ref rooms);
                if (LeftBranch.Room != null && RightBranch.Room != null) {
                    ConnectRooms(LeftBranch.Room, RightBranch.Room);
                }
            } else if (Room == null) {
                //create a randomly sized room, no bigger than the dungeon node and no smaller than the mimimum size
                int roomXOffset = (Width - MinWidth <= 0) ? 0 : Rand.Next(Width - MinWidth);
                int roomYOffset = (Height - MinHeight <= 0) ? 0 : Rand.Next(Height - MinHeight);
                int roomWidth = Math.Max(Rand.Next(Width - roomXOffset), MinWidth);
                int roomHeight = Math.Max(Rand.Next(Height - roomYOffset), MinHeight);
                Room = new Rectangle(roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                rooms.Add(Room);
            }
            //TODO: add connections between the branches
        }

        private void ConnectRooms(Rectangle left, Rectangle right) {
            bool AboveOrBelow = (left.Y < right.Y) ? true : false;
            bool LeftOrRight = (left.X < right.X) ? true : false;

            Position[] leftRoomX = new Position[left.Width];
            Position[] leftRoomY = new Position[left.Height];

            int Y = left.Y;
            for (int LX = 0; LX <= left.Width; LX++) {
                for (int RX = 0; RX <= right.Width; RX++) {
                    if ((left.X + LX) == (right.X + RX)) {
                        leftRoomX[LX] = new Position(left.X + LX, Y);
                    }
                }
            }

            int X = left.X;
            for (int LY = 0; LY <= left.Height; LY++) {
                for (int RY = 0; RY <= right.Height; RY++) {
                    if ((left.Y + LY) == (right.Y + RY)) {
                        leftRoomY[LY] = new Position(X, left.X + LY);
                    }
                }
            }

            double VorH = Rand.NextDouble();
            bool vertical = (VorH > .5) ? true : false;
            
        }
    }
}

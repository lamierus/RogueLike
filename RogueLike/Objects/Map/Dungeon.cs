using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random ();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight;
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Rectangle Room { get; private set; }

        // constructor with minimums built into it, so they can be provided, if you want
        public Dungeon (int fullWidth, int fullHeight, int x, int y, int minWidth = 16, int minHeight = 8) {
            Width = fullWidth;
            Height = fullHeight;
            X = x;
            Y = y;
            MinWidth = minWidth;
            MinHeight = minHeight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Split () {
            //bail if the branches aren't null, as it's already split
            if (LeftBranch != null || RightBranch != null) {
                return false;
            }

            bool vertical = VorH ();
            //find the maximum height/width
            //int max = ((vertical) ? Width : Height) - c_MinSize;
            int maxWidth = Width - MinWidth;
            int maxHeight = Height - MinHeight;
            //bail if the maximum is smaller than or equal to the minimum
            if (maxWidth <= MinWidth || maxHeight <= MinHeight) {
                return false;
            }

            //I was going to use this to randomize the sizes of the nodes even more.  Maybe I'll come back to this, sometime
            int minWidth = MinWidth; //(int)Math.Ceiling(MinWidth * .50);
            int minHeight = MinHeight; //Math.Max((int)Math.Ceiling(MinHeight * .45), 4);

            int splitPoint;
            if (vertical) { //vertical split
                splitPoint = Rand.Next (maxWidth);
                // adjust split point so there's at least c_MinSize in both partitions
                if (splitPoint < MinWidth) {
                    splitPoint = MinWidth;
                }
                LeftBranch = new Dungeon (splitPoint, Height, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon (Width - splitPoint, Height, X + splitPoint, Y, minWidth, minHeight);
            } else { //horizontal split
                splitPoint = Rand.Next (maxHeight);
                // adjust split point so there's at least c_MinSize in both partitions
                if (splitPoint < MinHeight) {
                    splitPoint = MinHeight;
                }
                LeftBranch = new Dungeon (Width, splitPoint, X, Y, minWidth, minHeight);
                RightBranch = new Dungeon (Width, Height - splitPoint, X, Y + splitPoint, minWidth, minHeight);
            }
            return true;
        }

        /// <summary>
        ///     provide a quick true or false to randomly pick vertical or horizontal
        /// </summary>
        /// <returns></returns>
        private bool VorH () {
            double VorH = Rand.NextDouble ();
            return (VorH >.5) ? true : false;
        }

        /// <summary>
        ///     iterate through each of the nodes and generate rooms at the bottom of the branch
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="halls"></param>
        public void GenerateRooms (ref List<Rectangle> rooms, ref List<Rectangle> halls) {
            //if neither of the  branches are null, then we'll go into here and attempt to generate rooms
            if (LeftBranch != null || RightBranch != null) {
                LeftBranch.GenerateRooms (ref rooms, ref halls);
                RightBranch.GenerateRooms (ref rooms, ref halls);
            } else if (Room == null) {
                //create a randomly sized room, no bigger than the dungeon node and no smaller than the mimimum size
                int roomXOffset = (Width - MinWidth <= 0) ? 0 : Rand.Next (Width - MinWidth);
                int roomYOffset = (Height - MinHeight <= 0) ? 0 : Rand.Next (Height - MinHeight);
                int roomWidth = Math.Max (Rand.Next (Width - roomXOffset), MinWidth);
                int roomHeight = Math.Max (Rand.Next (Height - roomYOffset), MinHeight);
                Room = new Rectangle (roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                rooms.Add (Room);

                if (rooms.Count > 1) {
                    GenerateHalls (ref rooms, ref halls);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallels"></param>
        /// <returns></returns>
        private Rectangle BuildHallway (List<Position>[] parallels) {
            Rectangle hallway;
            int countOfParallels = parallels[0].Count;
            if (countOfParallels > 3) {
                int randomChoice = Rand.Next (2, countOfParallels - 1);
                hallway = new Rectangle (parallels[0][randomChoice - 2], parallels[1][randomChoice]);
            } else if (countOfParallels < 3) {
                hallway = null;
            } else {
                hallway = new Rectangle (parallels[0][0], parallels[1][2]);
            }
            return hallway;
        }

        /// <summary>
        ///     generate halls in 4 directions from random spots on the walls of the room.
        ///     will have to figure out a way to connect them all at a later time...
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="halls"></param>
        public void GenerateHalls (ref List<Rectangle> rooms, ref List<Rectangle> halls) {
            Rectangle thisRoom = rooms[rooms.Count - 1];
            Rectangle lastRoom = rooms[rooms.Count - 2];
            Rectangle hallToAdd = null;
            bool areXParallels = thisRoom.CheckXParallel (lastRoom);
            bool areYParallels = thisRoom.CheckYParallel (lastRoom);
            if (areXParallels) {
                hallToAdd = BuildHallway (thisRoom.GetXAxisParallels (lastRoom));
            }
            if (areYParallels) {
                hallToAdd = BuildHallway (thisRoom.GetYAxisParallels (lastRoom));
            }
            if (hallToAdd == null) {
                List<Rectangle> hallsToAdd = new List<Rectangle> ();
                Position thisHallTopLeft, thisHallBottomRight, lastHallTopLeft, lastHallBottomRight;
                if (thisRoom.Y > lastRoom.Y) {
                    if (thisRoom.X > lastRoom.X) {
                        thisHallBottomRight = new Position (Rand.Next (thisRoom.X + 2, thisRoom.X + thisRoom.Width), thisRoom.Y);
                        lastHallTopLeft = new Position (lastRoom.X + Width, Rand.Next (lastRoom.Y + lastRoom.Width, lastRoom.Y + lastRoom.Width + lastRoom.Height - 2));
                        thisHallTopLeft = new Position (thisHallBottomRight.X - 2, lastHallTopLeft.Y);
                        lastHallBottomRight = thisHallTopLeft + 2;
                    } else {
                        thisHallBottomRight = new Position (Rand.Next (thisRoom.X + 2, thisRoom.X + thisRoom.Width), thisRoom.Y);
                        lastHallBottomRight = new Position (lastRoom.X, Rand.Next (lastRoom.Y + 2, lastRoom.Y + lastRoom.Height));
                        thisHallTopLeft = new Position (thisHallBottomRight.X - 2, lastHallBottomRight.Y - 2);
                        lastHallTopLeft = thisHallTopLeft;
                    }
                } else {
                    if (thisRoom.X > lastRoom.X) {
                        thisHallTopLeft = new Position (Rand.Next (thisRoom.X, thisRoom.X + Width - 2), thisRoom.Y + thisRoom.Height);
                        lastHallTopLeft = new Position (lastRoom.X + Width, Rand.Next (lastRoom.Y, lastRoom.Y + Height - 2));
                        thisHallBottomRight = new Position (thisHallTopLeft.X + 2, lastHallTopLeft.Y + 2);
                        lastHallBottomRight = thisHallBottomRight;
                    } else {
                        thisHallTopLeft = new Position (Rand.Next (thisRoom.X, thisRoom.X + Width - 2), thisRoom.Y + Height);
                        lastHallBottomRight = new Position (lastRoom.X, Rand.Next (lastRoom.Y, lastRoom.Y + Height - 2));
                        lastHallTopLeft = new Position (thisHallTopLeft.X, lastHallBottomRight.Y - 2);
                        thisHallBottomRight = lastHallTopLeft - 2;
                    }
                }
                hallsToAdd.Add (new Rectangle (thisHallTopLeft, thisHallBottomRight));
                hallsToAdd.Add (new Rectangle (lastHallTopLeft, lastHallBottomRight));
                foreach (Rectangle H in hallsToAdd) {
                    halls.Add (H);
                }
            } else {
                halls.Add (hallToAdd);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random ();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight;
        private Dungeon Root { get; set; }
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Rectangle Room { get; private set; }
        public List<Rectangle> Rooms = new List<Rectangle> ();
        public List<Rectangle> Halls = new List<Rectangle> ();

        // constructor with minimums built into it, so they can be provided, if you want
        public Dungeon (int fullWidth, int fullHeight, int x, int y, int minWidth = 16, int minHeight = 8) {
            Width = fullWidth;
            Height = fullHeight;
            X = x;
            Y = y;
            MinWidth = minWidth;
            MinHeight = minHeight;
        }

        public void SetRoot (Dungeon root) {
            Root = root;
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
            LeftBranch.Root = Root;
            RightBranch.Root = Root;
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
        public void GenerateRooms () {
            //if neither of the  branches are null, then we'll go into here and attempt to generate rooms
            if (LeftBranch != null || RightBranch != null) {
                if (LeftBranch != null) {
                    LeftBranch.GenerateRooms ();
                }
                if (RightBranch != null) {
                    RightBranch.GenerateRooms ();
                }
            } else {
                //create a randomly sized room, no bigger than the dungeon node and no smaller than the mimimum size
                int roomXOffset = (Width - MinWidth <= 0) ? 0 : Rand.Next (Width - MinWidth);
                int roomYOffset = (Height - MinHeight <= 0) ? 0 : Rand.Next (Height - MinHeight);
                int roomWidth = Math.Max (Rand.Next (Width - roomXOffset), MinWidth);
                int roomHeight = Math.Max (Rand.Next (Height - roomYOffset), MinHeight);
                Room = new Rectangle (roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                Root.Rooms.Add (Room);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        public void GenerateHalls () {
            if (LeftBranch.Room == null || RightBranch.Room == null) {
                if (LeftBranch.Room == null) {
                    LeftBranch.GenerateHalls ();
                }
                if (RightBranch.Room == null) {
                    RightBranch.GenerateHalls ();
                }
            } else {
                CreateHall (LeftBranch.Room, RightBranch.Room);
            }
        }

        /// <summary>
        ///     generate halls in 4 directions from random spots on the walls of the room.
        ///     will have to figure out a way to connect them all at a later time...
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="halls"></param>
        private void CreateHall (Rectangle leftRoom, Rectangle rightRoom) {
            Rectangle hallToAdd = null;
            if (leftRoom.CheckXParallel (rightRoom)) {
                hallToAdd = BuildHallway (leftRoom.GetXAxisParallels (rightRoom));
            }
            if (leftRoom.CheckYParallel (rightRoom)) {
                hallToAdd = BuildHallway (leftRoom.GetYAxisParallels (rightRoom));
            }
            if (hallToAdd == null) {
                Position leftHallTopLeft, leftHallBottomRight, rightHallTopLeft, rightHallBottomRight;
                if (leftRoom.Y >= rightRoom.Y) {
                    if (leftRoom.X >= rightRoom.X) {
                        leftHallBottomRight = new Position (Rand.Next (leftRoom.X + 2, leftRoom.X + leftRoom.Width), leftRoom.Y);
                        rightHallTopLeft = new Position (rightRoom.X + rightRoom.Width, Rand.Next (rightRoom.Y + rightRoom.Width, rightRoom.Y + rightRoom.Width + rightRoom.Height - 2));
                        leftHallTopLeft = new Position (leftHallBottomRight.X - 2, rightHallTopLeft.Y);
                        rightHallBottomRight = leftHallTopLeft + 2;
                    } else {
                        leftHallBottomRight = new Position (Rand.Next (leftRoom.X + 2, leftRoom.X + leftRoom.Width), leftRoom.Y);
                        rightHallBottomRight = new Position (rightRoom.X, Rand.Next (rightRoom.Y + 2, rightRoom.Y + rightRoom.Height));
                        leftHallTopLeft = new Position (leftHallBottomRight.X - 2, rightHallBottomRight.Y - 2);
                        rightHallTopLeft = leftHallTopLeft;
                    }
                } else {
                    if (leftRoom.X >= rightRoom.X) {
                        leftHallTopLeft = new Position (Rand.Next (leftRoom.X, leftRoom.X + leftRoom.Width - 2), leftRoom.Y + leftRoom.Height);
                        rightHallTopLeft = new Position (rightRoom.X + rightRoom.Width, Rand.Next (rightRoom.Y, rightRoom.Y + rightRoom.Height - 2));
                        leftHallBottomRight = new Position (leftHallTopLeft.X + 2, rightHallTopLeft.Y + 2);
                        rightHallBottomRight = leftHallBottomRight;
                    } else {
                        leftHallTopLeft = new Position (Rand.Next (leftRoom.X, leftRoom.X + leftRoom.Width - 2), leftRoom.Y + leftRoom.Height);
                        rightHallBottomRight = new Position (rightRoom.X, Rand.Next (rightRoom.Y + 2, rightRoom.Y + rightRoom.Height));
                        rightHallTopLeft = new Position (leftHallTopLeft.X, rightHallBottomRight.Y - 2);
                        leftHallBottomRight = rightHallTopLeft + 2;
                    }
                }
                Root.Halls.Add (new Rectangle (leftHallTopLeft, leftHallBottomRight));
                Root.Halls.Add (new Rectangle (rightHallTopLeft, rightHallBottomRight));
            } else {
                Root.Halls.Add (hallToAdd);
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
            if (countOfParallels >= 3) {
                int randomChoice = Rand.Next (2, countOfParallels - 1);
                hallway = new Rectangle (parallels[0][randomChoice - 2], parallels[1][randomChoice]);
            } else if (countOfParallels < 3) {
                hallway = null;
            } else {
                hallway = new Rectangle (parallels[0][0], parallels[1][2]);
            }
            return hallway;
        }
    }
}
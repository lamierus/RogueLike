using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random ();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight;
        public Dungeon (Dungeon root, Dungeon leftBranch, Dungeon rightBranch, Room room) {
            this.Root = root;
            this.LeftBranch = leftBranch;
            this.RightBranch = rightBranch;
            this.Room = room;

        }
        private Dungeon Root { get; set; }
        public Dungeon LeftBranch { get; private set; }
        public Dungeon RightBranch { get; private set; }
        public Room Room { get; private set; }
        public List<Room> Rooms = new List<Room> ();
        public List<Room> Halls = new List<Room> ();

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
                Room = new Room (roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                Root.Rooms.Add (Room);
            }
        }

        /// <summary>
        ///     generate halls in 4 directions from random spots on the walls of the room.
        ///     will have to figure out a way to connect them all at a later time...
        /// </summary>
        public void GenerateHalls () {
            Rooms.Sort ();
            for (int This = 0; This < Rooms.Count; This++) {
                for (int Next = 1; Next < Rooms.Count; Next++) {
                    if (Rooms[This] != Rooms[Next]) {
                        Room hallToAdd = null;
                        if (Rooms[This].CheckXParallel (Rooms[Next])) {
                            if (Rooms[This].Parallels != null) {
                                hallToAdd = BuildStraightHallway (Rooms[This].Parallels);
                            }
                            Rooms[This].ClearParallels ();
                        }
                        if (Rooms[This].CheckYParallel (Rooms[Next])) {
                            if (Rooms[This].Parallels != null) {
                                hallToAdd = BuildStraightHallway (Rooms[This].Parallels);
                            }
                            Rooms[This].ClearParallels ();
                        }
                        if (hallToAdd == null) { } else {
                            Root.Halls.Add (hallToAdd);
                        }
                    }
                }
                //Halls.Sort();
                for (int i = 0; i < Halls.Count; i++) {
                    Room dupe = Halls[i];
                    Halls.RemoveAll (hall => hall == Halls[i]);
                    Halls.Add (dupe);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parallels"></param>
            /// <returns></returns>
            private Room BuildStraightHallway (List<Parallel> parallels, bool negative) {
                Room hallway = null;
                int countOfParallels = parallels.Count;
                if (countOfParallels >= 3) {
                    int randomChoice = Rand.Next (2, countOfParallels - 1);
                    if (negative) {
                        hallway = new Room (parallels[randomChoice - 2].Second, parallels[randomChoice].First);
                    } else {
                        hallway = new Room (parallels[randomChoice - 2].First, parallels[randomChoice].Second);
                    }
                } else if (countOfParallels < 3) {
                    hallway = null;
                } else {
                    if (negative) {
                        hallway = new Room (parallels[0].Second, parallels[1].First);
                    } else {
                        hallway = new Room (parallels[0].First, parallels[1].Second);
                    }
                }
                return hallway;
            }
        }
    }
}
}
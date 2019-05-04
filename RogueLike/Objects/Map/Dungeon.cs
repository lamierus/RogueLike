using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random ();
        private readonly int Width, Height, X, Y, MinWidth, MinHeight, MinRoomWidth, MinRoomHeight;
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
        public List<Hallway> Halls = new List<Hallway> ();

        // constructor with minimums built into it, so they can be provided, if you want
        public Dungeon (int fullWidth, int fullHeight, int x, int y, int minWidth = 16, int minHeight = 8) {
            Width = fullWidth;
            Height = fullHeight;
            X = x;
            Y = y;
            MinWidth = minWidth;
            MinHeight = minHeight;
            MinRoomWidth = MinWidth - 2;
            MinRoomHeight = MinHeight - 2;
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

            bool vertical = isVertical ();
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
        private bool isVertical () {
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
                int roomWidth = Math.Max (Rand.Next (Width - roomXOffset), MinRoomWidth);
                int roomHeight = Math.Max (Rand.Next (Height - roomYOffset), MinRoomHeight);
                Room = new Room (roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
                Root.Rooms.Add (Room);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="floor"></param>
        public void GenerateHalls (ref FloorGrid floor, out string message) {
            message = null;
            for (int Current = 0; Current < Rooms.Count; Current++) {
                for (int Next = 1; Next < Rooms.Count; Next++) {
                    if (Rooms[Current] != Rooms[Next] &&
                        !((Rooms[Current].ConnectedRooms.Contains (Rooms[Next])) ||
                            (Rooms[Next].ConnectedRooms.Contains (Rooms[Current])))) {
                        if (Rooms[Current].CheckXParallel (Rooms[Next])) {
                            Rooms[Current].ConnectedRooms.Add (Rooms[Next]);

                        }
                        if (Rooms[Current].CheckYParallel (Rooms[Next])) {
                            Rooms[Current].ConnectedRooms.Add (Rooms[Next]);

                        }
                    }
                }
                List<Room> parallelRooms = Rooms[Current].ConnectedRooms
                    .FindAll (R => Rooms[Current].HallsIntersect (R)).ToList ();
                for (int i = 0; i < parallelRooms.Count; i++) {
                    Hallway hallToAdd = null;
                    // hallToAdd = BuildStraightHallway (Rooms[Current], Rooms[Current].ConnectedRooms.IndexOf (parallelRooms[i]));
                    if (hallToAdd == null) {
                        List<Hallway> hallsToAdd = ProbeForRoom (Rooms[Current], ref floor, out message);
                        foreach (Hallway H in hallsToAdd) {
                            Root.Halls.Add (H);
                        }
                    } else {
                        Root.Halls.Add (hallToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private Hallway BuildStraightHallway (Room room, int indexOfOther) {
            Hallway hallway = null;
            int randomChoice = Rand.Next (room.Parallels[indexOfOther].Count);
            hallway = new Hallway (room.Parallels[indexOfOther][randomChoice].Hall.First (), room.Parallels[indexOfOther][randomChoice].Hall.Last ());
            if (!(hallway.CheckForAdjacentOrSame (Halls))) {
                return hallway;
            }
            return null;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="room"></param>
        /// <param name="floor"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private List<Hallway> ProbeForRoom (Room room, ref FloorGrid floor, out string message) {
            message = null;
            List<Hallway> hallsFound = null;
            if ((room.X > MinRoomWidth) || (room.X + room.Width < Width - MinRoomWidth) ||
                (room.Y > MinRoomHeight) || (room.Y + room.Height < Height - MinRoomHeight)) {
                hallsFound = new List<Hallway> ();
                bool connected = false;
                Position start = new Position (-1, -1);
                Position end = new Position (-2, -2);
                Object thing;
                bool vertical = isVertical ();
                int turns = 0;

                while (!connected) {
                    getRandomStretch (vertical, ref start, ref end, ref floor, ref room, out thing);
                    vertical = !vertical;
                    var newHallway = new Hallway (start, end);
                    if (!(newHallway.CheckForAdjacentOrSame (Halls))) {
                        hallsFound.Add (newHallway);
                        turns++;
                    }
                    if (turns >= 6) {
                        hallsFound.Clear ();
                        turns = 0;
                        start = new Position (-1, -1);
                        end = new Position (-2, -2);
                        thing = null;
                        hallsFound.Clear();
                    } else if (thing is Floor) {
                        connected = true;
                    } else{
                        start = end;
                    }
                }
            }
            return hallsFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertical"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="floor"></param>
        /// <param name="room"></param>
        /// <param name="thing"></param>
        private void getRandomStretch (bool vertical, ref Position start, ref Position end, ref FloorGrid floor, ref Room room, out Object thing) {
            int index = 0;
            int stretch = 15; //Rand.Next (10, 20);
            bool NorS_EorW = isVertical ();
            if (start != end) {
                if (vertical) {
                    if (NorS_EorW) {
                        start = new Position (Rand.Next (room.X, room.X + room.Width), room.Y);
                    } else {
                        start = new Position (Rand.Next (room.X, room.X + room.Width), room.Y + room.Height);
                    }
                } else {
                    if (NorS_EorW) {
                        start = new Position (room.X, Rand.Next (room.Y, room.Y + room.Height));
                    } else {
                        start = new Position (room.X + room.Width, Rand.Next (room.Y, room.Y + room.Height));
                    }
                }
                end = start;
            }
            do {
                thing = null;
                index++;
                if (vertical) {
                    if (NorS_EorW) {
                        end.Y--;
                    } else {
                        end.Y++;
                    }
                    if (end.Y >= floor.Height || end.Y <= 0) {
                        end = start;
                        break;
                    }
                } else {
                    if (NorS_EorW) {
                        end.X--;
                    } else {
                        end.X++;
                    }
                    if (end.X > floor.Width || end.X < 0) {
                        end = start;
                        break;
                    }
                }
                thing = floor.Grid[end.X][end.Y];
            } while (!(thing is Floor) && index <= stretch);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Dungeon {
        private Random Rand = new Random ();
        private readonly int Width, Height, X = 0, Y = 0, MinWidth = 16, MinHeight = 8,
            MinRoomWidth, MinRoomHeight, NumRoomTries = 300, roomExtraSize = 4, WindingPercent = 0;
        private int CurrentRegion = -1;
        private int[, ] Regions;
        // // private Dungeon Root { get; set; }
        // public Dungeon LeftBranch { get; private set; }
        // public Dungeon RightBranch { get; private set; }
        // public Room Room { get; private set; }
        // public List<Hallway> Halls { get; private set; } = new List<Hallway> ();
        public List<Room> Rooms { get; private set; } = new List<Room> ();

        // constructor with minimums built into it, so they can be provided, if you want
        // public Dungeon (int fullWidth, int fullHeight, int x, int y, int minWidth, int minHeight, int numRoomTries) {
        //     Width = fullWidth;
        //     Height = fullHeight;
        //     Regions = new int[Width, Height];
        //     X = x;
        //     Y = y;
        //     MinWidth = minWidth;
        //     MinHeight = minHeight;
        //     MinRoomWidth = MinWidth - 2;
        //     MinRoomHeight = MinHeight - 2;
        //     NumRoomTries = numRoomTries;
        // }

        // public Dungeon (int fullWidth, int fullHeight, int x, int y, int minWidth, int minHeight) {
        //     Width = fullWidth;
        //     Height = fullHeight;
        //     Regions = new int[Width, Height];
        //     X = x;
        //     Y = y;
        //     MinWidth = minWidth;
        //     MinHeight = minHeight;
        //     MinRoomWidth = MinWidth - 2;
        //     MinRoomHeight = MinHeight - 2;
        // }

        // public Dungeon (int fullWidth, int fullHeight, int x, int y, int numRoomTries) {
        //     Width = fullWidth;
        //     Height = fullHeight;
        //     Regions = new int[Width, Height];
        //     X = x;
        //     Y = y;
        //     MinRoomWidth = MinWidth - 2;
        //     MinRoomHeight = MinHeight - 2;
        //     NumRoomTries = numRoomTries;
        // }
        // public Dungeon (int fullWidth, int fullHeight, int x, int y) {
        //     Width = fullWidth;
        //     Height = fullHeight;
        //     Regions = new int[Width, Height];
        //     X = x;
        //     Y = y;
        //     MinRoomWidth = MinWidth - 2;
        //     MinRoomHeight = MinHeight - 2;
        // }
        public Dungeon (int fullWidth, int fullHeight) {
            Width = fullWidth;
            Height = fullHeight;
            MinRoomWidth = MinWidth;
            MinRoomHeight = MinHeight;
            Regions = new int[Width, Height];
        }

        // public void SetRoot (Dungeon root) {
        //     Root = root;
        // }

        /// <summary>
        ///     provide a quick true or false to randomly pick vertical or horizontal
        /// </summary>
        /// <returns></returns>
        private bool isVertical () {
            double VorH = Rand.NextDouble ();
            return (VorH >.5) ? true : false;
        }

        public void Generate (ref FloorGrid floor) {
            // if (stage.width % 2 == 0 || stage.height % 2 == 0) {
            //     throw new ArgumentError ("The stage must be odd-sized.");
            // }
            //Regions = new int[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Regions[x, y] = CurrentRegion;
                }
            }

            GenerateRooms (ref floor);

            // Fill in all of the empty space with mazes.
            for (int y = 0; y < Height; y += 2) {
                for (int x = 0; x < Width; x += 2) {
                    var pos = new Position (x, y);
                    if (!(floor.GetObject (pos) is NullSpace)) {
                        continue;
                    }
                    GrowMaze (pos, ref floor);
                }
            }

            ConnectRegions (ref floor);
            //RemoveDeadEnds (ref floor);

            //_rooms.forEach (onDecorateRoom);
        }

        private void GenerateRooms (ref FloorGrid floor) {
            /// Places rooms ignoring the existing maze corridors.
            for (int i = 0; i < NumRoomTries; i++) {
                // Pick a random room size. The funny math here does two things:
                // - It makes sure rooms are odd-sized to line up with maze.
                // - It avoids creating rooms that are too rectangular: too tall and
                //   narrow or too wide and flat.
                var size = Rand.Next (2, 3 + roomExtraSize) * 2;
                var rectangularity = Rand.Next (0, 1 + (int) ((size / 2) * 2.5));
                var height = Rand.Next (MinRoomHeight, MinRoomHeight + roomExtraSize);
                var width = Rand.Next (MinRoomWidth, MinRoomWidth + roomExtraSize);
                if (isVertical ()) {
                    width += rectangularity;
                } else {
                    height += rectangularity;
                }

                var x = Rand.Next (Width - width);
                var y = Rand.Next (Height - height);

                var room = new Room (width, height, x, y);

                var overlaps = false;
                foreach (var other in Rooms) {
                    if (other.IsIntersectedBy (room)) {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps) {
                    continue;
                }

                Rooms.Add (room);

                StartRegion ();
                foreach (var pos in room.Rectangle) {
                    if (room.IsInRoom (pos)) {
                        Carve (new Floor (pos), ref floor);
                    } else {
                        Carve (new Wall (pos), ref floor);
                    }
                }
            }
        }

        /// Implementation of the "growing tree" algorithm from here:
        /// http://www.astrolog.org/labyrnth/algrithm.htm.
        private void GrowMaze (Position start, ref FloorGrid floor) {
            List<Position> cells = new List<Position> ();
            Stack<Position> them = new Stack<Position> ();
            Position lastDir = start;

            StartRegion ();
            Carve (new Floor (start), ref floor);

            cells.Add (start);
            while (cells.Count > 0) {
                var cell = cells.Last ();

                // See which adjacent cells are open.
                var unmadeCells = new List<Position> ();

                foreach (var dir in Direction.Cardinals) {
                    if (CanCarve (cell, dir, ref floor)) {
                        unmadeCells.Add (dir);
                    }
                }

                if (unmadeCells.Count > 0) {
                    // Based on how "windy" passages are, try to prefer carving in the same direction.
                    Position dir;
                    if (unmadeCells.Contains (lastDir) && Rand.Next (100) > WindingPercent) {
                        dir = lastDir;
                    } else {
                        dir = unmadeCells[Rand.Next (unmadeCells.Count - 1)];
                    }

                    Carve (new Floor (cell + dir), ref floor);
                    unmadeCells.Remove (dir);
                    foreach (var pos in unmadeCells) {
                        Carve (new Wall (cell + pos), ref floor);
                    }
                    //Carve (new Floor (cell + dir * 2), ref floor);

                    //cells.Add (cell + dir * 2);
                    cells.Add (cell + dir);
                    lastDir = dir;
                } else {
                    // No adjacent uncarved cells.
                    cells.RemoveAt (cells.Count - 1);

                    // This path has ended.
                    lastDir = Position.Zero;
                }
            }
        }

        void ConnectRegions (ref FloorGrid floor) {
            //Find all of the tiles that can connect two (or more) regions.
            Dictionary<Position, HashSet<int>> connectorRegions = new Dictionary<Position, HashSet<int>> ();
            //var connectorRegions = <Vec, Set<int>> { };
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Position pos = new Position (x, y);
                    //Can't already be part of a region.
                    if (!(floor.GetObject (pos) is Wall)) {
                        continue;
                    }

                    var regions = new HashSet<int> ();
                    foreach (var dir in Direction.Cardinals) {
                        if (floor.InBounds (pos + dir)) {
                            var eachDirection = new Position (pos + dir);
                            var region = Regions[eachDirection.X, eachDirection.Y];
                            if (region != -1) {
                                regions.Add (region);
                            }
                        }
                    }

                    if (regions.Count < 2) {
                        continue;
                    }
                    connectorRegions.Add (pos, regions);
                    // connectorRegions[pos] = regions;
                }
            }

            var connectors = connectorRegions.Keys.ToList ();

            // Keep track of which regions have been merged. This maps an original
            // region index to the one it has been merged to.
            var merged = new List<int> ();
            var openRegions = new HashSet<int> ();
            for (int i = 0; i <= CurrentRegion; i++) {
                merged.Add (i);
                openRegions.Add (i);
            }

            //Keep connecting regions until we're down to one.
            while (openRegions.Count > 1) {
                var connector = connectors[Rand.Next (connectors.Count - 1)];

                // Carve the connection.
                AddJunction (connector, ref floor);

                // Merge the connected regions. We'll pick one region (arbitrarily) and
                // map all of the other regions to its index.
                var regions = connectorRegions[connector].Select (region => merged[region]);
                var dest = regions.First ();
                var sources = regions.Skip (1).ToList ();

                // Merge all of the affected regions. We have to look at *all* of the
                // regions because other regions may have previously been merged with
                // some of the ones we're merging now.
                for (var i = 0; i <= CurrentRegion; i++) {
                    if (sources.Contains (merged[i])) {
                        merged[i] = dest;
                    }
                }

                // The sources are no longer in use.
                openRegions.RemoveWhere (i => sources.Contains (i));

                // Remove any connectors that aren't needed anymore.
                connectors.RemoveAll (pos => {
                    // Don't allow connectors right next to each other.
                    if (Position.Distance (connector, pos) < 2) return true;

                    // If the connector no longer spans different regions, we don't need it.
                    regions = connectorRegions[pos].Select ((region) => merged[region]);

                    if (regions.Count () > 1) return false;

                    // // This connecter isn't needed, but connect it occasionally so that the
                    // // dungeon isn't singly-connected.
                    // if (rng.oneIn (extraConnectorChance)) AddJunction (pos);

                    return true;
                });
            }
        }

        void AddJunction (Position pos, ref FloorGrid floor) {
            // if (Rand.Next (3) == 0) {
            //     floor.AddItem (new Floor (pos));
            // } else {
            floor.SetObject (new Door (pos));
            // }
        }

        void RemoveDeadEnds (ref FloorGrid floor) {
            var done = false;

            while (!done) {
                done = true;

                for (int x = 0; x < floor.Width; x++) {
                    for (int y = 0; y < floor.Height; y++) {
                        Position pos = new Position (x, y);
                        //foreach (var pos in bounds.inflate (-1))
                        if (floor.GetObject (pos) is Wall) continue;

                        // If it only has one exit, it's a dead end.
                        var exits = 0;
                        foreach (var dir in Direction.Cardinals) {
                            if (floor.InBounds (pos + dir)) {
                                if (floor.GetObject (pos + dir) is Floor) {
                                    exits++;
                                }
                            }
                        }

                        if (exits != 1) continue;

                        done = false;
                        floor.SetObject (new Wall (pos));
                    }
                }
            }
        }

        /// Gets whether or not an opening can be carved from the given starting
        /// [Cell] at [pos] to the adjacent Cell facing [direction]. Returns `true`
        /// if the starting Cell is in bounds and the destination Cell is filled
        /// (or out of bounds).
        bool CanCarve (Position pos, Position direction, ref FloorGrid floor) {
            // Must end in bounds.
            if (!floor.InBounds (pos + direction)) {
                return false;
            }

            // Destination must not be open.
            return floor.GetObject (pos + direction) is NullSpace;
        }

        private void StartRegion () {
            CurrentRegion++;
        }

        private void Carve (Object obj, ref FloorGrid floor) {
            floor.SetObject (obj);
            Regions[obj.XY.X, obj.XY.Y] = CurrentRegion;
        }

        // /// <summary>
        // ///     
        // /// </summary>
        // /// <returns></returns>
        // public bool Split () {
        //     //bail if the branches aren't null, as it's already split
        //     if (LeftBranch != null || RightBranch != null) {
        //         return false;
        //     }

        //     bool vertical = isVertical ();
        //     //find the maximum height/width
        //     //int max = ((vertical) ? Width : Height) - c_MinSize;
        //     int maxWidth = Width - MinWidth;
        //     int maxHeight = Height - MinHeight;
        //     //bail if the maximum is smaller than or equal to the minimum
        //     if (maxWidth <= MinWidth || maxHeight <= MinHeight) {
        //         return false;
        //     }

        //     //I was going to use this to randomize the sizes of the nodes even more.  Maybe I'll come back to this, sometime
        //     int minWidth = MinWidth; //(int)Math.Ceiling(MinWidth * .50);
        //     int minHeight = MinHeight; //Math.Max((int)Math.Ceiling(MinHeight * .45), 4);

        //     int splitPoint;
        //     if (vertical) { //vertical split
        //         splitPoint = Rand.Next (maxWidth);
        //         // adjust split point so there's at least c_MinSize in both partitions
        //         if (splitPoint < MinWidth) {
        //             splitPoint = MinWidth;
        //         }
        //         LeftBranch = new Dungeon (splitPoint, Height, X, Y, minWidth, minHeight);
        //         RightBranch = new Dungeon (Width - splitPoint, Height, X + splitPoint, Y, minWidth, minHeight);
        //     } else { //horizontal split
        //         splitPoint = Rand.Next (maxHeight);
        //         // adjust split point so there's at least c_MinSize in both partitions
        //         if (splitPoint < MinHeight) {
        //             splitPoint = MinHeight;
        //         }
        //         LeftBranch = new Dungeon (Width, splitPoint, X, Y, minWidth, minHeight);
        //         RightBranch = new Dungeon (Width, Height - splitPoint, X, Y + splitPoint, minWidth, minHeight);
        //     }
        //     LeftBranch.Root = Root;
        //     RightBranch.Root = Root;
        //     return true;
        // }

        // /// <summary>
        // ///     iterate through each of the nodes and generate rooms at the bottom of the branch
        // /// </summary>
        // /// <param name="rooms"></param>
        // /// <param name="halls"></param>
        // public void GenerateRooms () {
        //     //if neither of the  branches are null, then we'll go into here and attempt to generate rooms
        //     if (LeftBranch != null || RightBranch != null) {
        //         if (LeftBranch != null) {
        //             LeftBranch.GenerateRooms ();
        //         }
        //         if (RightBranch != null) {
        //             RightBranch.GenerateRooms ();
        //         }
        //     } else {
        //         //create a randomly sized room, no bigger than the dungeon node and no smaller than the mimimum size
        //         int roomXOffset = (Width - MinWidth <= 1) ? 1 : Rand.Next (Width - MinWidth) - 1;
        //         int roomYOffset = (Height - MinHeight <= 1) ? 1 : Rand.Next (Height - MinHeight) - 1;
        //         int roomWidth = Math.Max (Rand.Next (Width - roomXOffset), MinRoomWidth);
        //         int roomHeight = Math.Max (Rand.Next (Height - roomYOffset), MinRoomHeight);
        //         Room = new Room (roomWidth, roomHeight, X + roomXOffset, Y + roomYOffset);
        //         Root.Rooms.Add (Room);
        //     }
        // }

        // /// <summary>
        // ///     
        // /// </summary>
        // /// <param name="floor"></param>
        // public void GenerateHalls (ref FloorGrid floor, out string message) {
        //     message = null;
        //     for (int Current = 0; Current < Rooms.Count; Current++) {
        //         for (int Next = 1; Next < Rooms.Count; Next++) {
        //             if (Rooms[Current] != Rooms[Next] &&
        //                 !((Rooms[Current].ConnectedRooms.Contains (Rooms[Next])) ||
        //                     (Rooms[Next].ConnectedRooms.Contains (Rooms[Current])))) {
        //                 if (Rooms[Current].CheckXParallel (Rooms[Next])) {
        //                     Rooms[Current].ConnectedRooms.Add (Rooms[Next]);

        //                 }
        //                 if (Rooms[Current].CheckYParallel (Rooms[Next])) {
        //                     Rooms[Current].ConnectedRooms.Add (Rooms[Next]);

        //                 }
        //             }
        //         }
        //         List<Room> parallelRooms = Rooms[Current].ConnectedRooms
        //             .FindAll (R => Rooms[Current].HallsIntersect (R)).ToList ();
        //         for (int i = 0; i < parallelRooms.Count; i++) {
        //             Hallway hallToAdd = null;
        //             // hallToAdd = BuildStraightHallway (Rooms[Current], Rooms[Current].ConnectedRooms.IndexOf (parallelRooms[i]));
        //             if (hallToAdd == null) {
        //                 List<Hallway> hallsToAdd = ProbeForRoom (Rooms[Current], ref floor, out message);
        //                 foreach (Hallway H in hallsToAdd) {
        //                     Root.Halls.Add (H);
        //                 }
        //             } else {
        //                 Root.Halls.Add (hallToAdd);
        //             }
        //         }
        //     }
        // }

        // /// <summary>
        // ///     
        // /// </summary>
        // /// <param name="room"></param>
        // /// <returns></returns>
        // private Hallway BuildStraightHallway (Room room, int indexOfOther) {
        //     Hallway hallway = null;
        //     int randomChoice = Rand.Next (room.Parallels[indexOfOther].Count);
        //     hallway = new Hallway (room.Parallels[indexOfOther][randomChoice].Hall.First (), room.Parallels[indexOfOther][randomChoice].Hall.Last ());
        //     if (!(hallway.CheckForAdjacentOrSame (Halls))) {
        //         return hallway;
        //     }
        //     return null;
        // }

        // /// <summary>
        // ///     
        // /// </summary>
        // /// <param name="room"></param>
        // /// <param name="floor"></param>
        // /// <param name="message"></param>
        // /// <returns></returns>
        // private List<Hallway> ProbeForRoom (Room room, ref FloorGrid floor, out string message) {
        //     message = null;
        //     List<Hallway> hallsFound = null;
        //     if ((room.X > MinRoomWidth) || (room.X + room.Width < Width - MinRoomWidth) ||
        //         (room.Y > MinRoomHeight) || (room.Y + room.Height < Height - MinRoomHeight)) {
        //         hallsFound = new List<Hallway> ();
        //         bool connected = false;
        //         Position start = new Position (-1, -1);
        //         Position end = new Position (-2, -2);
        //         Object thing;
        //         bool vertical = isVertical ();
        //         int turns = 0;

        //         while (!connected) {
        //             getRandomStretch (vertical, ref start, ref end, ref floor, ref room, out thing);
        //             vertical = !vertical;
        //             var newHallway = new Hallway (start, end);
        //             if (!(newHallway.CheckForAdjacentOrSame (Halls))) {
        //                 hallsFound.Add (newHallway);
        //                 turns++;
        //             }
        //             if (turns >= 6) {
        //                 hallsFound.Clear ();
        //                 turns = 0;
        //                 start = new Position (-1, -1);
        //                 end = new Position (-2, -2);
        //                 thing = null;
        //             } else if (thing is Floor) {
        //                 connected = true;
        //             } else {
        //                 start = end;
        //             }
        //         }
        //     }
        //     return hallsFound;
        // }

        // /// <summary>
        // ///     
        // /// </summary>
        // /// <param name="vertical"></param>
        // /// <param name="start"></param>
        // /// <param name="end"></param>
        // /// <param name="floor"></param>
        // /// <param name="room"></param>
        // /// <param name="thing"></param>
        // private void getRandomStretch (bool vertical, ref Position start, ref Position end, ref FloorGrid floor, ref Room room, out Object thing) {
        //     int index = 0;
        //     int stretch = 15; //Rand.Next (10, 20);
        //     bool NorS_EorW = isVertical ();
        //     if (start != end) {
        //         if (vertical) {
        //             if (NorS_EorW) {
        //                 start = new Position (Rand.Next (room.X, room.X + room.Width), room.Y);
        //             } else {
        //                 start = new Position (Rand.Next (room.X, room.X + room.Width), room.Y + room.Height);
        //             }
        //         } else {
        //             if (NorS_EorW) {
        //                 start = new Position (room.X, Rand.Next (room.Y, room.Y + room.Height));
        //             } else {
        //                 start = new Position (room.X + room.Width, Rand.Next (room.Y, room.Y + room.Height));
        //             }
        //         }
        //         end = start;
        //     }
        //     do {
        //         thing = null;
        //         index++;
        //         if (vertical) {
        //             if (NorS_EorW) {
        //                 end.Y--;
        //             } else {
        //                 end.Y++;
        //             }
        //             if (end.Y >= floor.Height || end.Y <= 0) {
        //                 end = start;
        //                 break;
        //             }
        //         } else {
        //             if (NorS_EorW) {
        //                 end.X--;
        //             } else {
        //                 end.X++;
        //             }
        //             if (end.X > floor.Width || end.X < 0) {
        //                 end = start;
        //                 break;
        //             }
        //         }
        //         thing = floor.Grid[end.X][end.Y];
        //     } while (!(thing is Floor) && index <= stretch);
        // }
    }
}
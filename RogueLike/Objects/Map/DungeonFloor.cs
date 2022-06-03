using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class DungeonFloor {
        private Random Rand = new Random ();
        private readonly int Width, Height, X = 0, Y = 0, MinWidth = 16, MinHeight = 8,
            MinRoomWidth, MinRoomHeight, NumRoomTries = 50, roomExtraSize = 4, WindingPercent = 0;
        private int CurrentRegion = -1;
        private int[, ] Regions;
        public List<Room> Rooms { get; private set; } = new List<Room> ();

        public DungeonFloor (int fullWidth, int fullHeight) {
            Width = fullWidth;
            Height = fullHeight;
            MinRoomWidth = MinWidth;
            MinRoomHeight = MinHeight;
            Regions = new int[Width, Height];
        }

        /// <summary>
        ///     provide a quick true or false to randomly pick vertical or horizontal
        /// </summary>
        /// <returns> 
        ///     True or false, depending on the generated double (between 0.0 and 1.0)
        /// </returns>
        private bool isVertical () {
            double VorH = Rand.NextDouble ();
            return (VorH >.5) ? true : false;
        }

        public void Generate (ref FloorGrid floor) {
            // if (stage.width % 2 == 0 || stage.height % 2 == 0) {
            //     throw new ArgumentError ("The stage must be odd-sized.");
            // }
            //set all parts of the dungeon floor's regions to -1
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Regions[x, y] = CurrentRegion;
                }
            }

            GenerateRooms (ref floor);
            GenerateHalls (ref floor);
            /*// Fill in all of the empty space with mazes.
            for (int y = 0; y < Height; y += 2) {
                for (int x = 0; x < Width; x += 2) {
                    var pos = new Position (x, y);
                    if (!(floor.GetObject (pos) is NullSpace)) {
                        continue;
                    }
                    else {
                        GrowMaze (pos, ref floor);
                    }
                }
            }*/

            //ConnectRegions (ref floor);
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

                //Check to see if this new room overlaps any previous rooms added
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

                StartNewRegion ();
                //CurrentRegion++;
                foreach (var pos in room.Rectangle) {
                    if (room.IsInRoom (pos)) {
                        Carve (new Floor (pos), ref floor);
                    } else {
                        Carve (new Wall (pos), ref floor);
                    }
                }
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="floor"> referenced floor plan from the program</param>
        public void GenerateHalls (ref FloorGrid floor) {
            
            /*message = null;
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
            }*/
        }

        void AddDoor (Position pos, ref FloorGrid floor) {
            // if (Rand.Next (3) == 0) {
            //     floor.AddItem (new Floor (pos));
            // } else {
            floor.SetObject (new Door (pos));
            // }
        }

        /// Gets whether or not an opening can be carved from the given starting
        /// [Cell] at [pos] to the adjacent Cell facing [direction]. Returns `true`
        /// if the starting Cell is in bounds and the destination Cell is filled
        /// (or out of bounds).
        bool CanCarve (Position pos, Position direction, ref FloorGrid floor) {
            // Must end in bounds.
            if (!floor.IsInBounds (pos + direction)) {
                return false;
            }

            // Destination must not be open.
            return floor.GetObject (pos + direction) is NullSpace;
        }

        private void StartNewRegion () {
            CurrentRegion++;
        }

        private void Carve (Object obj, ref FloorGrid floor) {
            floor.SetObject (obj);
            Regions[obj.XY.X, obj.XY.Y] = CurrentRegion;
        }
        
        /*/// Implementation of the "growing tree" algorithm from here:
        /// http://www.astrolog.org/labyrnth/algrithm.htm.
        private void GrowMaze (Position start, ref FloorGrid floor) {
            List<Position> cells = new List<Position> ();
            //Stack<Position> them = new Stack<Position> ();
            Position lastDir = start;

            StartNewRegion ();
            //CurrentRegion++;
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
        }*/

        /*void ConnectRegions (ref FloorGrid floor) {
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
                        if (floor.IsInBounds (pos + dir)) {
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
        }*/

        /*void RemoveDeadEnds (ref FloorGrid floor) {
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
                            if (floor.IsInBounds (pos + dir)) {
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
        }*/
    }
}
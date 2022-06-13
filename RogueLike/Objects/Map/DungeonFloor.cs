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
        public List<Hallway> Halls { get; private set; } = new List<Hallway>();

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
            CarveRooms(ref floor);
            GenerateHalls (ref floor);
            CarveHalls(ref floor);
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
            }
            Rooms.Sort();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="floor"> referenced floor plan from the program</param>
        private void CarveRooms(ref FloorGrid floor){
            foreach (Room r in Rooms){
                foreach (Position pos in r.Rectangle) {
                    if (r.IsInRoom (pos)) {
                        if (isVertical()){
                            Carve (new Floor (pos), ref floor);
                        }
                        else Carve (new Rug(pos, Rooms.IndexOf(r).ToString()), ref floor);
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
            //go through each room and probe out from each direction to attempt to find hallways in straight lines
            foreach (Room R in Rooms){
                for (int dir = 0; dir < 4; dir++){
                    List<Position> Wall = R.GetRoomWall(dir);
                    int index = Rand.Next(1, Wall.Count - 1);
                    Position start = Wall[index];
                    Hallway newHall;
                    if (SendProbe(start, Direction.whichDirection(dir), ref floor, out newHall)){
                        R.Doors.Add(start);
                        Halls.Add(newHall);
                    }
                }
            }
        }

        private bool SendProbe(Position start, Position direction, ref FloorGrid floor, out Hallway newHall){
            Position probe = start + direction;
            newHall = new Hallway(start, probe);
            while (floor.IsInBounds(probe)){
                Object obj = floor.GetObject(probe);
                if (obj is Wall){
                    newHall = new Hallway(start, probe);
                    return true;
                }
                probe += direction;
            }
            return false;
        }

        private void CarveHalls(ref FloorGrid floor){
            foreach (Hallway H in Halls){
                foreach (Floor F in H.Hall) {
                    if (F.XY == H.Start || F.XY == H.End){
                        Carve(new Door(F.XY), ref floor);
                    } else {
                        Carve (F, ref floor);
                    }
                }
            }
        }

        private void StartNewRegion () {
            CurrentRegion++;
        }

        private void Carve (Object obj, ref FloorGrid floor) {
            floor.SetObject (obj);
            Regions[obj.XY.X, obj.XY.Y] = CurrentRegion;
        }
    }
}
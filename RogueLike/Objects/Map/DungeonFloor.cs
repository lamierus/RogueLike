using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class DungeonFloor {
        private Random Rand = new Random ();
        private readonly int Width, Height, MinRoomWidth = 10, MinRoomHeight = 10, NumRoomTries = 100, roomExtraSize = 4;
        private int CurrentRegion = -1;
        private int[, ] Regions;
        public List<(Room room, int region)> Rooms { get; private set; } = new List<ValueTuple<Room, int>> ();
        public List<(Hallway hall, int region)> Halls { get; private set; } = new List<ValueTuple<Hallway, int>>();

        public DungeonFloor (int fullWidth, int fullHeight) {
            Width = fullWidth;
            Height = fullHeight;
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

                var newRoom = new Room (width, height, x, y);

                //Check to see if this new room overlaps any previous rooms added
                var overlaps = false;
                foreach (var other in Rooms) {
                    if (other.room.IsIntersectedBy (newRoom)) {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps) {
                    continue;
                }

                StartNewRegion ();
                Rooms.Add((newRoom, CurrentRegion));
                //Rooms.Add(new ValueTuple<Room, int>(newRoom, CurrentRegion));
            }
            //Rooms.Sort();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="floor"> referenced floor plan from the program</param>
        public void GenerateHalls (ref FloorGrid floor) {
            //go through each room and probe out from each direction to attempt to find hallways in straight lines
            foreach (var tR in Rooms){
                int dir = 0;
                int retry = roomExtraSize;
                while (dir < 4 && retry > 0){
                    List<Position> Wall = tR.room.GetRoomWall(dir);
                    int index = Rand.Next(1, Wall.Count - 1);  // shouldn't start at a corner
                    Position start = Wall[index];
                    Hallway newHall;
                    if (SendProbe(start, Direction.whichDirection(dir), ref floor, out newHall)){
                        //we can assume the object at the end of the newHall is a wall, at this point
                        //so, we check if it is a corner and decrement the count to try again
                        if (!(floor.GetObject(newHall.End) as Wall).isCorner(ref floor)){
                            tR.room.Doors.Add(start);
                            Halls.Add((newHall,tR.region));
                            dir++;
                            retry = roomExtraSize;
                        }
                    }
                    retry--;
                }
            }
        }

        private bool SendProbe(Position start, Position direction, ref FloorGrid floor, out Hallway newHall){
            Position probe = start + direction;
            newHall = new Hallway();
            while (floor.IsInBounds(probe)){
                Object obj = floor.GetObject(probe);
                if ((obj is NullSpace)){
                    probe += direction;
                } else {
                    newHall = new Hallway(start, probe);
                    return true;
                }
            }
            return false;
        }

        private void StartNewRegion () {
            CurrentRegion++;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="floor"> referenced floor plan from the program</param>
        private void CarveRooms(ref FloorGrid floor){
            foreach (var tR in Rooms){
                foreach (Position pos in tR.room.Rectangle) {
                    if (tR.Item1.IsInRoom (pos)) {
                        if (isVertical()){
                            Carve (new Floor (pos), tR.region, ref floor);
                        }
                        else Carve (new Rug(pos, tR.region.ToString()), tR.region, ref floor);
                    } else {
                        Carve (new Wall (pos), tR.region, ref floor);
                    }
                }
            }
        }

        private void CarveHalls(ref FloorGrid floor){
            foreach (var H in Halls){
                foreach (Floor F in H.hall.Hall) {
                    if (F.XY == H.hall.Start || F.XY == H.hall.End){
                        Carve(new Door(F.XY), H.region, ref floor);
                    } else {
                        //Carve (F, ref floor);
                        Carve (new Rug(F.XY, H.region.ToString()), H.region, ref floor);
                    }
                }
            }
        }

        private void Carve (Object obj, int region, ref FloorGrid floor) {
            floor.SetObject (obj);
            Regions[obj.XY.X, obj.XY.Y] = region;
        }
    }
}
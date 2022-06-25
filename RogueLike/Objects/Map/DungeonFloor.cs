using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Xml.Linq;
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
        //public List<(Hallway hall, int region)> Halls { get; private set; } = new List<ValueTuple<Hallway, int>>();

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
            GenerateHalls (ref floor);
            RemoveDisconnectedRooms(ref floor);
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
                foreach (Position pos in newRoom.Rectangle) {
                    if (newRoom.IsInRoom (pos)) {
                        if (isVertical()){
                            Carve (new Floor (pos), CurrentRegion, ref floor);
                        }
                        else Carve (new Rug(pos, CurrentRegion.ToString()), CurrentRegion, ref floor);
                    } else {
                        Carve (new Wall (pos), CurrentRegion, ref floor);
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
            foreach (var tR in Rooms){
                int direction = 0;
                int retry = roomExtraSize;
                while (direction < 4 && retry > 0){
                    List<Position> Wall = tR.room.GetRoomWall(direction);
                    if(!(Wall.Any(pos => tR.room.Doors.Any(door => pos == door)))){
                        int index = Rand.Next(1, Wall.Count - 1);  // shouldn't start at a corner
                        Position start = Wall[index];
                        Hallway newHall;
                        int connectedRegion;
                        if (SendProbe(start, direction, ref floor, out newHall, out connectedRegion)){
                            //we can assume the object at the end of the newHall is a wall, at this point
                            //so, we check if it is a corner and do not add to the direction count to try again
                            if (floor.GetObject(newHall.End) is Wall){
                                //need to check if it is a Wall object, first, then if it is a corner 
                                //this way hallways can connect to hallways, too
                                if (!(floor.GetObject(newHall.End) as Wall).isCorner(ref floor)){
                                    //add the door to the starting room
                                    tR.room.Doors.Add(start);
                                    //add the door at the end of the hallway to the connected room/region
                                    Rooms.ElementAt(connectedRegion).room.Doors.Add(newHall.End);
                                    foreach (Floor F in newHall.Hall) {
                                        if (F.XY == newHall.Start || F.XY == newHall.End){
                                            Carve(new Door(F.XY), tR.region, ref floor);
                                        } else {
                                            //Carve (F, ref floor);
                                            Carve (new Rug(F.XY, tR.region.ToString()), tR.region, ref floor);
                                        }
                                    }
                                    direction++;
                                    retry = roomExtraSize;
                                }
                            }
                        }
                        retry--;
                    }
                    else {
                        direction++;
                    }
                }
            }
        }

        private bool SendProbe(Position start, int direction, ref FloorGrid floor,
                                out Hallway newHall, out int connectedRegion){
            Position probe = start + Direction.whichDirection(direction);
            newHall = new Hallway(start, start);
            connectedRegion = -1;
            while (floor.IsInBounds(probe)){
                Object obj = floor.GetObject(probe);
                if ((obj is NullSpace)){
                    probe += direction;
                } else {
                    newHall = new Hallway(start, probe);
                    if(newHall.Hall.Count <= 2 && 
                        Rooms[Regions[newHall.End.X, newHall.End.Y]].room.IsCorner(newHall.End)){
                        return false;
                    }
                    connectedRegion = Regions[probe.X, probe.Y];
                    return true;
                }
            }
            return false;
        }

        private void StartNewRegion () {
            CurrentRegion++;
        }

        private void RemoveRegion () {
            CurrentRegion--;
        }
        
        private void Carve (Object obj, int region, ref FloorGrid floor) {
            floor.SetObject (obj);
            Regions[obj.XY.X, obj.XY.Y] = region;
        }

        private void RemoveDisconnectedRooms(ref FloorGrid floor){
            foreach(var tR in Rooms){
                if (tR.room.Doors.Count == 0){
                    foreach(Position pos in tR.room.Rectangle){
                        Regions[pos.X, pos.Y] = -1;
                        floor.SetObject(new NullSpace());
                    }
                }
            }
        }
    }
}
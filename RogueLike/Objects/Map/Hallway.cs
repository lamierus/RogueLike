using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public List<Floor> Hall {get; private set;} = new List<Floor>();
        public List<Wall> Walls {get; private set;} = new List<Wall>();
        public Position Start {
            get{ return Hall[0].XY; }
        }
        public Position End {
            get { return Hall[Hall.Count-1].XY; }
        }
        public Hallway (Position start, Position end) {
            for (int i = 0; i <= (int) Position.Distance (start, end); i++) {
                if (start.IsInLineX(end)) {
                    if (start.Y < end.Y) {
                        Hall.Add (new Floor (start.X, start.Y + i)); // Heading South
                        //adding walls
                        Walls.Add (new Wall (start.X + 1, start.Y + i));
                        Walls.Add (new Wall (start.X - 1, start.Y + i));
                    } else {
                        Hall.Add (new Floor (start.X, start.Y - i)); // North
                        //adding walls
                        Walls.Add (new Wall (start.X + 1, start.Y - i));
                        Walls.Add (new Wall (start.X - 1, start.Y - i));
                    }
                } else {
                    if (start.X < end.X) {
                        Hall.Add (new Floor (start.X + i, start.Y)); // East
                        //adding walls
                        Walls.Add (new Wall (start.X + i, start.Y + 1));
                        Walls.Add (new Wall (start.X + i, start.Y - 1));
                    } else {
                        Hall.Add (new Floor (start.X - i, start.Y)); // West
                        //adding walls
                        Walls.Add (new Wall (start.X - i, start.Y + 1));
                        Walls.Add (new Wall (start.X - i, start.Y - 1));
                    }
                }
            }
        }
        
        public int Color {
            get { return GetMapObject (Start).Color; }
        }
        public ConsoleCharacter Character {
            get { return GetMapObject (Start).SolidCharacter; }
        }
        private Floor GetMapObject (Position pos) {
            return new Floor (pos.X, pos.Y);
        }
    }
}
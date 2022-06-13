using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public List<Floor> Hall { get; private set; }
        public Position Start {
            get{ return Hall[0].XY; }
        }
        public Position End {
            get { return Hall[Hall.Count-1].XY; }
        }
        public Hallway (){

        }
        public Hallway (Position start, Position end) {
            Hall = new List<Floor>();
            for (int i = 0; i <= (int) Position.Distance (start, end); i++) {
                if (start.IsInLineX(end)) {
                    if (start.Y < end.Y) {
                        Hall.Add (new Floor (start.X, start.Y + i)); // Heading South
                    } else {
                        Hall.Add (new Floor (start.X, start.Y - i)); // North
                    }
                } else {
                    if (start.X < end.X) {
                        Hall.Add (new Floor (start.X + i, start.Y)); // East
                    } else {
                        Hall.Add (new Floor (start.X - i, start.Y)); // West
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
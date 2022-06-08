using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Hallway {
        public List<Position> Hall { get; private set; } = new List<Position> ();
        public Hallway (Position start, Position end) {
            for (int i = 0; i <= (int) Position.Distance (start, end); i++) {
                if (start.X == end.X) {
                    if (start.Y < end.Y) {
                        Hall.Add (new Position (start.X, start.Y + i));
                    } else {
                        Hall.Add (new Position (start.X, start.Y - i));
                    }
                } else {
                    if (start.X < end.X) {
                        Hall.Add (new Position (start.X + i, start.Y));
                    } else {
                        Hall.Add (new Position (start.X - i, start.Y));
                    }
                }
            }
        }
        public int Color {
            get { return GetMapObject ().Color; }
        }
        public ConsoleCharacter Character {
            get { return GetMapObject ().SolidCharacter; }
        }
        public Floor GetMapObject (int x, int y) {
            return new Floor (x, y);
        }
        private Floor GetMapObject () {
            return new Floor (0, 0);
        }
        /*public bool CheckForAdjacentOrSame (List<Hallway> hallsToCheck) {
            foreach (Hallway H in hallsToCheck) {
                if (IsAdjacentToOrSameAs (H)) {
                    return true;
                }
            }
            return false;
        }
        public bool IsAdjacentToOrSameAs (Hallway hallToCheck) {
            int count = 0;
            foreach (Position p in hallToCheck.Hall) {
                if (Hall.Exists (point => point.IsAdjacentOrSame (p))) {
                    count++;
                }
                if (count >= 2) {
                    return true;
                }
            }
            return false;
        }*/
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Rectangle{
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }
        public int WallColor{ 
            get {return 2;}
            }
        public ConsoleCharacter Wall{
            get {return ConsoleCharacter.Medium;}
        }
    }
    public class Dungeon {
        private const int c_MinSize = 10;
        private Random Rand = new Random();
        private Dungeon LeftBranch;
        private Dungeon RightBranch;
        private int Width, Height;
        private Position TopLeft;
        private List<Rectangle> Rooms = new List<Rectangle>();
        public int MaxBranches;

        public Dungeon(int width, int height, int top, int left) {
            Width = width;
            Height = height;
            TopLeft = new Position(top, left);
        }

        public bool Split() {
            if (LeftBranch == null) {
                return false;
            }
            return true;
        }

        public void GenerateBranches() {
            int VorH;
            int max;
            for (int i = 0; i < MaxBranches; i++) {
                VorH = Rand.Next(0, 1);
                if (VorH > 0) {
                    
                } else {
                    
                }
            }
        }

        public List<Rectangle> GetDungeonLevel (ref ConsoleEngine engine){
            return Rooms;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Dungeon {
        private const int c_MinSize = 10;
        private Random Rand = new Random();
        private Dungeon LeftBranch;
        private Dungeon RightBranch;
        private int Width, Height;
        private Position TopLeft;
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
    }
}

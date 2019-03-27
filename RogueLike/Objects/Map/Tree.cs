using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Tree {
        private Branch LeftBranch;
        private Branch RightBranch;
        private Random Rand = new Random();
        private int Width;
        private int Height;
        public int MaxBranches;

        public Tree(int width = 5, int height = 5, int maxBranches = 1) {
            MaxBranches = maxBranches;
            Width = width;
            Height = height;
            GenerateBranches();
        }

        private void GenerateBranches() {
            int VorH;
            int max;
            for (int i = 0; i < MaxBranches; i++) {
                VorH = Rand.Next(0, 1);
                if (VorH > 0) {
                    max = Rand.Next(1, (Width - (Width % MaxBranches)));
                    LeftBranch = new Branch(Width, max);
                    RightBranch = new Branch(Width, Height - max);
                } else {
                    max = Rand.Next(1, (Height - (Height % MaxBranches)));
                    LeftBranch = new Branch(max, Height);
                    RightBranch = new Branch(Width - max, Height);
                }
            }
        }
    }
}

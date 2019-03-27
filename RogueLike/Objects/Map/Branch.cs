using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Branch{
        private Branch LeftBranch;
        private Branch RightBranch;
        private Random Rand;
        private int Width;
        private int Height;
        private int MaxRoomWidth;
        private int MaxRoomHeight;

        public Branch(int width, int height) {
            Width = width;
            MaxRoomWidth = width - 1;
            Height = height;
            MaxRoomHeight = Height - 1;
        }
        
    }
}

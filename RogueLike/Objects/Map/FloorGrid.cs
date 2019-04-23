using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class FloorGrid {
        public Object[][] Grid { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public FloorGrid (int width, int height) {
            Grid = new Object[width][];
            for (int i = 0; i < width; i++){
                Grid[i] = new Object [height];
                for (int j = 0; j < height; j++){
                    Grid[i][j] = new Wall(i,j);
                }
            }
            Width = width - 1;
            Height = height - 1;
        }

        public bool AddItem (Object objToAdd) {
            if (Grid[objToAdd.XY.X][objToAdd.XY.Y] == null) {
                if (objToAdd is Item) {
                    Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Item;
                } else if (objToAdd is Wall) {
                    Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Wall;
                }
                return true;
            }
            return false;
        }

        private void RemoveObject (Object objToRemove) {
            Grid[objToRemove.XY.X][objToRemove.XY.Y] = null;
        }

        public void PickUpItem (Item itemPickedUp) {
            RemoveObject (itemPickedUp);
        }
    }
}
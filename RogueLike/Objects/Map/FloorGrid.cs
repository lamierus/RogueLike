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
            for (int x = 0; x < width; x++) {
                Grid[x] = new Object[height];
                for (int y = 0; y < height; y++) {
                    Grid[x][y] = new Wall (x, y);
                }
            }
            Width = width;
            Height = height;
        }

        public bool AddItem (Object objToAdd) {
            if (objToAdd is Floor) {
                Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Floor;
                return true;
            }
            if (Grid[objToAdd.XY.X][objToAdd.XY.Y] is Floor) {
                if (objToAdd is Item) {
                    Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Item;
                    return true;
                }
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
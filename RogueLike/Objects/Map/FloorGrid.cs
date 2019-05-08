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
            Width = width - 1;
            Height = height - 1;
        }

        public bool AddItem (Object objToAdd) {
            if (objToAdd is Floor) {
                Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Floor;
                return true;
            }
            if (Grid[objToAdd.XY.X][objToAdd.XY.Y] is Floor) {
                if (objToAdd is Player) {
                    Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Player;
                    return true;
                }
                if (objToAdd is Item) {
                    Grid[objToAdd.XY.X][objToAdd.XY.Y] = objToAdd as Item;
                    return true;
                }
            }
            return false;
        }

        public Object GetItem (Position pos) {
            return Grid[pos.X][pos.Y];
        }

        private void RemoveObject (Object objToRemove) {
            Grid[objToRemove.XY.X][objToRemove.XY.Y] = null;
        }

        public void PickUpItem (Item itemPickedUp) {
            RemoveObject (itemPickedUp);
        }

        public void MoveObject (Object objToMove, Position XY) {
            if (objToMove is Player) {
                Grid[objToMove.XY.X][objToMove.XY.Y] = new Floor (objToMove.XY.X, objToMove.XY.Y);
                Grid[XY.X][XY.Y] = objToMove as Player;
            }
        }
    }
}
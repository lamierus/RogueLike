using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class LevelGrid {
        public Object[,] Grid;

        public LevelGrid(int width, int height) {
            Grid = new Object[width, height];
        }

        public bool AddItem(Object objToAdd) {
            if (Grid[objToAdd.XY.X, objToAdd.XY.Y] == null) {
                if (objToAdd is Item) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Item;
                } else if (objToAdd is Wall) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Wall;
                }
                return true;
            }
            return false;
        }

        private void RemoveObject(Object objToRemove) {
            Grid[objToRemove.XY.X, objToRemove.XY.Y] = null;
        }

        public void PickUpItem(Item itemPickedUp) {
            RemoveObject(itemPickedUp);
        }
    }
}

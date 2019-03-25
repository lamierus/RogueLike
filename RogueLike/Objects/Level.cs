using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Level {
        public Object[,] Grid;

        public Level(int width, int height) {
            Grid = new Object[width, height];
        }

        public bool AddItem(Object objToAdd) {
            if (Grid[objToAdd.XY.X, objToAdd.XY.Y] == null) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Item;
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

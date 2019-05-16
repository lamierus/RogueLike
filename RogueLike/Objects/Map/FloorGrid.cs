using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class FloorGrid {
        private Object[, ] Grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public FloorGrid (int width, int height) {
            Grid = new Object[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Grid[x, y] = new NullSpace (x, y);
                }
            }
            Width = width - 1;
            Height = height - 1;
        }

        public bool InBounds (Position pos) {
            return pos.X >= 0 && pos.Y >= 0 && pos.X <= Width && pos.Y <= Height;
        }

        public bool SetObject (Object objToAdd) {
            if (objToAdd is Floor) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Floor;
                return true;
            }
            if (objToAdd is Wall) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Wall;
                return true;
            }
            if (objToAdd is Door) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Door;
                return true;
            }
            if (Grid[objToAdd.XY.X, objToAdd.XY.Y] is Floor) {
                if (objToAdd is Player) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Player;
                    return true;
                }
                if (objToAdd is Item) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Item;
                    return true;
                }
            }
            return false;
        }

        public Object GetObject (Position pos) {
            return Grid[pos.X, pos.Y];
        }

        private void RemoveObject (Object objToRemove) {
            Grid[objToRemove.XY.X, objToRemove.XY.Y] = null;
        }

        public void PickUpItem (Item itemPickedUp) {
            RemoveObject (itemPickedUp);
        }

        public void MoveObject (Object objToMove, Position newXY) {
            // if (objToMove is Player) {
            //     var oldXY = new Position (objToMove.XY);
            //     var objOnSPace = GetObject (newXY);
            //     if (objOnSPace is Floor) {
            //         Grid[oldXY.X, oldXY.Y] = new Floor (oldXY.X, oldXY.Y);
            //     } else if (objOnSPace is Door) {
            //         if (!(objOnSPace as Door).Locked) {
            //             Grid[oldXY.X, oldXY.Y] = new Door (oldXY, false, true);
            //         }
            //     }
            //     Grid[newXY.X, newXY.Y] = objToMove as Player;
            // }
        }
    }
}
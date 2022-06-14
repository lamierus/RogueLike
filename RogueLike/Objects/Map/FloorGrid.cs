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

        public bool IsInBounds (Position pos) {
            return pos.X >= 0 && pos.Y >= 0 && pos.X <= Width && pos.Y <= Height;
        }

        public bool SetObject (Object objToAdd) {
            if (objToAdd is Floor) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Floor;
                return true;
            }
            else if (objToAdd is Wall) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Wall;
                return true;
            }
            else if (objToAdd is Door) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Door;
                return true;
            }
            else if (objToAdd is Rug) {
                Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Rug;
                return true;
            }
            if (Grid[objToAdd.XY.X, objToAdd.XY.Y] is Floor) {
                if (objToAdd is Player) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Player;
                    return true;
                }
                else if (objToAdd is Monster) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Monster;
                    return true;
                }
                else if (objToAdd is Item) {
                    Grid[objToAdd.XY.X, objToAdd.XY.Y] = objToAdd as Item;
                    return true;
                }
            }
            return false;
        }

        public Object GetObject (Position pos) {
            return Grid[pos.X, pos.Y];
        }

        public Object GetObject (Object obj) {
            return Grid[obj.XY.X, obj.XY.Y];
        }

        public List<Object> GetAdjacentObjects(Position pos){
            List<Object> adjacents = new List<Object>();
            foreach (Position dir in Direction.Cardinals){
                adjacents.Add(GetObject(pos + dir));
            }
            return adjacents;
        }

        public List<Object> GetAdjacentObjects(Object obj){
            List<Object> adjacents = new List<Object>();
            foreach (Position dir in Direction.Cardinals){
                adjacents.Add(GetObject(obj.XY + dir));
            }
            return adjacents;
        }

        private void RemoveObject (Object objToRemove) {
            var removable = new Position(objToRemove.XY.X, objToRemove.XY.Y);
            Grid[removable.X, removable.Y] = new Floor(removable);
        }

        public void PickUpItem (Item itemPickedUp) {
            RemoveObject (itemPickedUp);
        }

        public void MoveObject (Object objToMove, Position newXY) {
            
        }
    }
}
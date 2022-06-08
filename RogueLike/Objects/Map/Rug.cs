using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

/// <summary>
/// ConsoleCharacter
///     .Null - ?
///     .Light - 1/3 pixels
///     .Medium - 1/2 pixels
///     .Dark - 3/4 pixels
///     .BoxDrawingL_XX
///         drawing boxes with pixels, each of the last 2 characters explain the positioning of the lines in the pixel
/// </summary>

namespace RogueLike {
    public class Rug : Structure {
        public int Width {get; private set;}
        public int Length {get; private set;}
        public List<Object> ObjectMatrix = new List<Object>();

        public Rug (Position pos, string texture) {
            Color = 14;
            TextCharacter = texture;
            Immovable = true;
            FloorToCeiling = false;
            AboveOrBelow = false;
            XY = pos;
        }
        public Rug (int x, int y, string texture) {
            Color = 14;
            TextCharacter = texture;
            Immovable = true;
            FloorToCeiling = false;
            AboveOrBelow = false;
            XY.X = x;
            XY.Y = y;
        }

        /*public Rug (int x, int y, int length, int width, string texture) {
            Color = 14;
            Immovable = true;
            FloorToCeiling = false;
            AboveOrBelow = false;
            XY.X = x;
            XY.Y = y;
            Length = length;
            Width = width;
            InitializeObjectMatrix(texture);
        }

        public Rug (Position pos, int length, int width, string texture) {
            Color = 14;
            Immovable = true;
            FloorToCeiling = false;
            AboveOrBelow = false;
            XY = pos;
            Length = length;
            Width = width;
            InitializeObjectMatrix(texture);
        }

        private void InitializeObjectMatrix(string texture){
            Object obj = new Object();
            for (int y = 0; y < Width; y++){
                obj.XY.Y = XY.Y+y;
                for (int x = 0; x < Length; x++){
                    obj.XY.X = XY.X+x;
                    obj.TextCharacter = texture;
                    ObjectMatrix.Add(obj);
                }
            }
        }*/
    }
}
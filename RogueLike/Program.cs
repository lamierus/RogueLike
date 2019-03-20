using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConsoleGameEngine;

namespace RogueLike {
    class Program {

        /// <summary>
        /// ConsoleCharacter
        ///     .Null - ?
        ///     .Light - 1/3 pixels
        ///     .Medium - 1/2 pixels
        ///     .Dark - 3/4 pixels
        ///     .BoxDrawingL_XX
        ///         drawing boxes with pixels, each of the last 2 characters explain the positioning of the lines in the pixel
        /// </summary>

        const int c_WinWidth = 240;
        const int c_WinHeight = 64;
        const int c_PixelWidth = 4;
        const int c_PixelHeight = 8;
        const int intBlankSpot = 0;
        const ConsoleKey keyClose = ConsoleKey.Escape;
        private static bool GameState = true;
        private static int intPlayerColor = 1;
        public static Point pntPlayerPosition = new Point(10, 10);
        public static Point pntPLayerLastPosition = pntPlayerPosition;
        private static Random rndRandomNum = new Random();
        private static ConsoleEngine Engine;
        private static Thread AwaitUserInput = new Thread(() => AwaitUserInput_DoWork());
                

        static void Main(string[] args) {
            //Console.SetBufferSize(c_WinWidth, c_WinHeight);
            Engine = new ConsoleEngine(c_WinWidth, c_WinHeight, c_PixelWidth, c_PixelHeight);
            Engine.SetBackground(0);
            Console.Title = "WHAT IS EVEN GOING ON?!";
            CreateWalls();
            AddItem();

            AwaitUserInput.Start();

            while (!(Engine.GetKey(keyClose))) {
                Engine.SetPixel(pntPlayerPosition, intPlayerColor, ConsoleCharacter.Full);
                Engine.DisplayBuffer();
                Engine.SetPixel(pntPLayerLastPosition, intBlankSpot, ConsoleCharacter.Full);
                pntPLayerLastPosition = pntPlayerPosition;
            }
            GameState = false;
        }

        /// <summary>
        ///     currently only draws the rectangle around the edge, but can be modified to add "doorways" by blanking out sections,
        ///     depending on coordinates from a "map"
        /// </summary>
        static void CreateWalls() {
            Engine.Rectangle(new Point(0, 0), new Point(c_WinWidth - 1, c_WinHeight - 1), 2, ConsoleCharacter.Light);
        }

        static void AddItem() {
            var pntItem = new Point(0,0);
            var itemColor = 0;
            var strItem = ".";
            RandomizePixel(ref pntItem,ref itemColor);
            Engine.WriteText(pntItem, strItem, itemColor);
        }

        /// <summary>
        ///     Thread for capturing player key presses and creating movement
        /// </summary>
        static void AwaitUserInput_DoWork () {
            ConsoleKeyInfo keyPress;
            int moveX = 0, moveY = 0;
            while (GameState) {
                if (Console.KeyAvailable) {
                    keyPress = Console.ReadKey();
                    moveX = 0;
                    moveY = 0;
                    switch (Char.ToUpper(keyPress.KeyChar)) {
                        case 'W':
                            moveY = -1;
                            break;
                        case 'A':
                            moveX = -1;
                            break;
                        case 'S':
                            moveY = 1;
                            break;
                        case 'D':
                            moveX = 1;
                            break;
                        default:
                            break;
                    }
                }
                if ((pntPlayerPosition.X + moveX < c_WinWidth - 1) && (pntPlayerPosition.X + moveX > 0))
                    pntPlayerPosition.X += moveX;
                else
                    moveX *= -1;
                if ((pntPlayerPosition.Y + moveY < c_WinHeight - 1) && (pntPlayerPosition.Y + moveY > 0))
                    pntPlayerPosition.Y += moveY;
                else
                    moveY *= -1;
                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        ///     Used this for testing, but may find a use for it in the future
        /// </summary>
        /// <param name="point">starting point, to be changed to another random point</param>
        /// <param name="color">color of the point, which will be changed</param>
        static void RandomizePixel(ref Point point, ref int color) {
            var newPoint = new Point();
            do {
                newPoint.X = rndRandomNum.Next(1, c_WinWidth - 1);
                newPoint.Y = rndRandomNum.Next(1, c_WinHeight - 1);
            } while (newPoint.X == point.X && newPoint.Y == point.Y);
            point = newPoint;
            var newColor = 0;
            do {
                newColor = rndRandomNum.Next(0, 256);
            } while (newColor == color);
            color = newColor;
        }
    }
}

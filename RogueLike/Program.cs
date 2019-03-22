﻿using System;
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
        private static ConsoleEngine Engine;

        const int c_SideBar = 40;
        const int c_WinWidth = 240;
        const int c_WinHeight = 64;
        const int c_PixelWidth = 4;
        const int c_PixelHeight = 8;
        const int BlankSpotColor = 0;
        const ConsoleKey keyClose = ConsoleKey.Escape;

        public static Player You = new Player();
        public static Position PlayerLastPosition;
        public static Map CurrentLevel = new Map();
        public static Room CurrentRoom;
        
        private static int GameSpeed = 15;
        private static bool GameState = true;
        private static Random RandomNum = new Random();
        private static Thread AwaitUserInput = new Thread(() => AwaitUserInput_DoWork());

        static void Main(string[] args) {
            Engine = new ConsoleEngine(c_WinWidth, c_WinHeight, c_PixelWidth, c_PixelHeight);
            Engine.SetBackground(0);
            Console.Title = "Ummm... Isn't this Snake?  SNAKE!...  SNNNAAAAAAKKKKEEEE!!!";
            CreateWalls();
            AddItems();
            You.XY = new Position(10, 10);
            PlayerLastPosition = You.XY;

            AwaitUserInput.Start();

            while (!(Engine.GetKey(keyClose))) {
                Engine.SetPixel(You.XY.ToPoint(), You.RGBColor.GetHashCode(), ConsoleCharacter.Full);
                Engine.DisplayBuffer();
                Engine.SetPixel(PlayerLastPosition.ToPoint(), BlankSpotColor, ConsoleCharacter.Full);
                PlayerLastPosition = You.XY;
            }
            GameState = false;
        }

        /// <summary>
        ///     currently only draws the rectangle around the edge, but can be modified to add "doorways" by blanking out sections,
        ///     depending on coordinates from a "map"
        /// </summary>
        static void CreateWalls() {
            Engine.Rectangle(new Point(0, 0), new Point(c_WinWidth - c_SideBar - 1, c_WinHeight - 1), 2, ConsoleCharacter.Light);
        }

        static void AddItems() {
            var pntItem = new Position(0,0);
            var itemAmount = RandomNum.Next(1, 99);
            var newRoom = new Room();
            RandomizePixelPoint(ref pntItem);
            Gold item = new Gold(itemAmount, pntItem);
            newRoom.ItemsInRoom.Add(item);
            Engine.WriteText(item.XY.ToPoint(), item.Character.ToString(), item.Color);
            CurrentLevel.Rooms.Add(newRoom);
            CurrentRoom = newRoom;
        }

        static void DrawSideBar() {
            Position GoldCounter = new Position(c_WinWidth - c_SideBar + 2, 0);
            Engine.WriteText(GoldCounter.ToPoint(), "Gold: " /*+ You.*/, 255);
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
                    switch (char.ToUpper(keyPress.KeyChar)) {
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
                    }
                }
                if ((You.XY.X + moveX < c_WinWidth - c_SideBar - 1) && (You.XY.X + moveX > 0))
                    You.XY.X += moveX;
                if ((You.XY.Y + moveY < c_WinHeight - 1) && (You.XY.Y + moveY > 0))
                    You.XY.Y += moveY;
                foreach (Item i in CurrentRoom.ItemsInRoom) {
                    if (You.XY == i.XY) {
                        You.PickUpItem(i);
                        UpdateSideBar();
                    }
                }
                Thread.Sleep(GameSpeed);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        static void UpdateSideBar() {
            
        }

        /// <summary>
        ///     Used this for testing, but may find a use for it in the future
        /// </summary>
        /// <param name="position">starting point, to be changed to another random point</param>
        static void RandomizePixelPoint(ref Position position) {
            var newPosition = new Position();
            do {
                newPosition.X = RandomNum.Next(1, c_WinWidth - c_SideBar - 1);
                newPosition.Y = RandomNum.Next(1, c_WinHeight - c_SideBar - 1);
            } while (newPosition == position);
            position = newPosition;
        }

        /// <summary>
        ///     Used for testing, but may find a use for it in the future
        /// </summary>
        /// <param name="color">color of the point, which will be changed</param>
        static void RandomizePixelColor(ref int color) {
            var newColor = 0;
            do {
                newColor = RandomNum.Next(0, 256);
            } while (newColor == color);
            color = newColor;
        }
    }
}

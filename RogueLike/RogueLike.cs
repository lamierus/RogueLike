using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ConsoleGameEngine;

namespace RogueLike {
    class RogueLike : ConsoleGame {

        /// <summary>
        /// ConsoleCharacter
        ///     .Null - ?
        ///     .Light - 1/3 pixels
        ///     .Medium - 1/2 pixels
        ///     .Dark - 3/4 pixels
        ///     .BoxDrawingL_XX
        ///         drawing boxes with pixels, each of the last 2 characters explain the positioning of the lines in the pixel
        /// </summary>
        /*
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        */
        const int c_SideBar = 40;
        const int c_WinWidth = 170;
        const int c_WinHeight = 50;
        const int c_PixelWidth = 8;
        const int c_PixelHeight = 14;
        const int c_MaxWinWidth = c_WinWidth - 1;
        const int c_MaxWinHeight = c_WinHeight - 1;
        const int BlankSpotColor = 0;
        const ConsoleKey keyClose = ConsoleKey.Escape;

        public Player You = new Player();
        public Position PlayerLastPosition;
        public Map CurrentLevel = new Map();
        public Room CurrentRoom;
        
        private bool GameState = true;
        private Random RandomNum = new Random();

        static void Main(string[] args) {
            new RogueLike().Construct(c_WinWidth, c_WinHeight, c_PixelWidth, c_PixelHeight, FramerateMode.MaxFps);

        }

        public override void Create() {
            Engine.SetPalette(Palettes.Default);
            Engine.SetBackground(0);
            //Engine.Borderless();
            //IntPtr ptr = GetConsoleWindow();
            //MoveWindow(ptr, 0, 0, Console.WindowWidth, Console.WindowHeight, true);
            Console.Title = "Dungeon of IT";
            TargetFramerate = 60;

            DrawGameBoard();
            AddItems();
            AddMobs();
            DrawSideBar();
            Restart();
        }

        /// <summary>
        ///     currently only draws the rectangle around the edge, but can be modified to add "doorways" by blanking out sections,
        ///     depending on coordinates from a "map"
        /// </summary>
        void DrawGameBoard() {
            
            Engine.Rectangle(new Point(0, 0), new Point(c_MaxWinWidth, c_MaxWinHeight), 2, ConsoleCharacter.Light);
            Engine.Line(new Point(c_MaxWinWidth - c_SideBar, 1), new Point(c_MaxWinWidth - c_SideBar, c_MaxWinHeight - 1), 2, ConsoleCharacter.Light);

            //Engine.Rectangle(new Point(0, 0), new Point(c_MaxWinWidth - c_SideBar, c_MaxWinHeight), 2, ConsoleCharacter.Light);
            //Engine.Rectangle(new Point(c_MaxWinWidth - c_SideBar + 1, 0), new Point(c_MaxWinWidth, c_MaxWinHeight), 3, ConsoleCharacter.Light);
            
            int i = 0;
            //for (int y = 1; y <= 8; y++) {
                for (int x = 1; x <= 48; x += 3) {
                    Engine.WriteText(new Point(x, c_MaxWinHeight - 1), i.ToString("###"), i++);
                }
            //}
            
        }

        void AddItems() {
            var pntItem = new Position(0,0);
            var itemAmount = RandomNum.Next(1, 99);
            var newRoom = new Room(c_MaxWinWidth -1, c_MaxWinHeight -1);
            RandomizePixelPoint(ref pntItem);
            Gold item = new Gold(itemAmount, pntItem);
            newRoom.AddItem(item);
            Engine.WriteText(item.XY.ToPoint(), item.Character.ToString(), item.Color);
            CurrentLevel.Rooms.Add(newRoom);
            CurrentRoom = newRoom;
        }

        void AddMobs() {
            //TODO: add code to randomize mobs, based on level, and place in the room
        }

        void DrawSideBar() {
            Position GoldCounter = new Position(c_WinWidth - c_SideBar + 1, 2);
            Engine.WriteText(GoldCounter.ToPoint(), "Gold: " + You.GoldAmt, 3);
        }


        /// <summary>
        ///     
        /// </summary>
        void UpdateSideBar() {
            DrawSideBar();
        }

        public override void Update() {
            if (GameState) {
                if (!(Engine.GetKey(keyClose))) {

                    ConsoleKeyInfo keyPress;
                    int moveX = 0, moveY = 0;
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
                    if (CurrentRoom.Grid[You.XY.X, You.XY.Y] != null) {
                        Object thing = CurrentRoom.Grid[You.XY.X, You.XY.Y];
                        bool attacked;
                        if (You.Interact(thing, out attacked))
                            if (!attacked) {
                                CurrentRoom.PickUpItem(thing as Item);
                            }
                        UpdateSideBar();
                    }
                } else {
                    GameState = false;
                }
            } else {
                Restart();
            }
        }

        public override void Render() {
            Engine.WriteText(You.XY.ToPoint(), You.Character.ToString(), 2);
            Engine.DisplayBuffer();
            Engine.SetPixel(PlayerLastPosition.ToPoint(), BlankSpotColor, ConsoleCharacter.Full);
            PlayerLastPosition = You.XY;
        }

        void Restart() {
            GameState = true;
            You.XY = new Position(10, 10);
            PlayerLastPosition = You.XY;
        }
        
        /// <summary>
        ///     Used this for testing, but may find a use for it in the future
        /// </summary>
        /// <param name="position">starting point, to be changed to another random point</param>
        void RandomizePixelPoint(ref Position position) {
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
        void RandomizePixelColor(ref int color) {
            var newColor = 0;
            do {
                newColor = RandomNum.Next(0, 256);
            } while (newColor == color);
            color = newColor;
        }
    }
}

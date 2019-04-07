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
        const int c_PixelWidth = 8;
        const int c_PixelHeight = 12;
        const int c_MaxWinWidth = 170;
        const int c_MaxWinHeight = 50;
        const int BlankSpotColor = 0;
        const int LevelWallColor = 2;
        const int InventoryTextColor = 3;
        const ConsoleKey keyClose = ConsoleKey.Escape;

        public Player You = new Player();
        public Position PlayerLastPosition;
        public LevelGrid CurrentLevel;

        private Position LogTopLeft = new Position(c_MaxWinWidth - c_SideBar + 1, 37);
        private Position LogBottomRight = new Position(c_MaxWinWidth - 2, 48);
        private bool GameState = true;
        private Random RandomNum = new Random();
        private Queue<string> LogMessages = new Queue<string>();

        static void Main(string[] args) {
            new RogueLike().Construct(c_MaxWinWidth, c_MaxWinHeight, c_PixelWidth, c_PixelHeight, FramerateMode.MaxFps);

        }

        public override void Create() {
            Engine.SetPalette(Palettes.Default);
            Engine.SetBackground(0);
            //Engine.Borderless();
            //IntPtr ptr = GetConsoleWindow();
            //MoveWindow(ptr, 0, 0, Console.WindowWidth, Console.WindowHeight, true);
            Console.Title = "Dungeon of IT";
            TargetFramerate = 10;

            DrawFrame();
            BuildLevel();
            AddItems();
            AddMobs();
            DrawSideBar();
            DrawLog();
            Restart();
        }

        /// <summary>
        ///     currently only draws the rectangle around the edge,  but just use it as a color pallete draw
        /// </summary>
        void DrawFrame() {
            int i = 0;
            //for (int y = 1; y <= 8; y++) {
                for (int x = 1; x <= 51/*96*/; x += 3) {
                    Engine.WriteText(new Point(x, c_MaxWinHeight), i.ToString(" ##"), i++);
                }
            //}
            
        }

        void BuildLevel() {
            int LevelWidth = c_MaxWinWidth - c_SideBar;
            int levelHeight = c_MaxWinHeight;
            CurrentLevel = new LevelGrid(LevelWidth, levelHeight);
            List<Rectangle> rooms = new List<Rectangle>();
            List<Dungeon> dungeonParts = new List<Dungeon>();
            Dungeon dungeon = new Dungeon(LevelWidth - 1, levelHeight - 1, 0, 0);
            dungeonParts.Add(dungeon);

            bool didSplit = true;
            int splitIndex = 0;

            while (didSplit) {
                didSplit = false;
                for (int i = 0; i <= dungeonParts.Count; i++) {
                    Dungeon toSplit = dungeonParts.ElementAt(splitIndex);
                    if (toSplit.LeftBranch == null && toSplit.RightBranch == null) //if this leaf is not already split
                    {
                        if (toSplit.Split()) {
                            //If we did split, add child branches
                            dungeonParts.Add(toSplit.LeftBranch);
                            dungeonParts.Add(toSplit.RightBranch);
                            didSplit = true;
                            i = 0;
                        }
                    } else {
                    }
                    splitIndex = RandomNum.Next(dungeonParts.Count);
                }
            }
            dungeon.GenerateRooms(ref rooms);
            DrawLevel(rooms);
        }

        void DrawLevel(List<Rectangle> Rooms) {
            foreach (Rectangle R in Rooms) {
                //R.OffsetRectangle(1, 1);
                Engine.Rectangle(R.TopLeft, R.BottomRight, R.WallColor, R.Wall);
                for (int x = 0; x <= R.Width; x++) {
                    for (int y = 0; y <= R.Height; y++) {
                        if (!((R.X + x > R.X && R.Y + y > R.Y) && (R.X + x < R.X + R.Width && R.Y + y < R.Y + R.Height))) {
                            CurrentLevel.AddItem(new Wall((R.X + x), (R.Y + y)));
                        }
                    }
                }
            }
        }
        
        void AddItems() {
            var itemPos = new Position(0,0);
            var itemAmount = RandomNum.Next(1, 99);
            RandomizePixelPoint(ref itemPos);
            Gold item = new Gold(itemAmount, itemPos);
            CurrentLevel.AddItem(item);
            Engine.WriteText(item.XY.ToPoint(), item.Character.ToString(), item.Color);
            
        }

        void AddMobs() {
            //TODO: add code to randomize mobs, based on level #, and place them in the room
        }

        void DrawSideBar() {
            Engine.Rectangle(new Point(c_MaxWinWidth - c_SideBar, 0), new Point(c_MaxWinWidth - 1, c_MaxWinHeight - 1), 3, ConsoleCharacter.Light);
            Position TextXY = new Position(c_MaxWinWidth - c_SideBar + 2, 2);
            Engine.WriteText(TextXY.ToPoint(), "Name: " + You.Name, InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), "HP:   ", InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), "MP:   ", InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), "Atk:  ", InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), "Gold: ", InventoryTextColor);
            UpdateSideBar();
        }


        /// <summary>
        ///     
        /// </summary>
        void UpdateSideBar() {
            Position TextXY = new Position(c_MaxWinWidth - c_SideBar + 8, 3);
            Engine.WriteText(TextXY.ToPoint(), You.HP.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.MP.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.Atk.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.GoldAmt.ToString(), InventoryTextColor);
        }

        /// <summary>
        ///     draw the frame for the output log
        /// </summary>
        void DrawLog() {
            Engine.Rectangle(LogTopLeft.ToPoint(), LogBottomRight.ToPoint(), 4, ConsoleCharacter.Light);
            //("this is just a test of the emergency broadcast system. only a test.");
            AddLog("Welcome to the Dungeon of IT!");
            UpdateLog();
        }

        /// <summary>
        ///     add a message to the Log to be displayed on the screen
        /// </summary>
        /// <param name="message">
        ///     the string message, maxiumum is 36 characters
        /// </param>
        void AddLog(string message) {
            int maxLength = 36;
            string lineTwo = null;
            if (message.Length > maxLength) {
                lineTwo = message.Substring(maxLength).Trim();
                lineTwo = lineTwo.Insert(0, "  ");
                message = message.Remove(maxLength, message.Length - maxLength);
            }
            message = message.PadRight(maxLength);
            LogMessages.Enqueue(message);
            if (lineTwo != null) {
                lineTwo = lineTwo.PadRight(maxLength);
                LogMessages.Enqueue(lineTwo);
            }
            while (LogMessages.Count > 10) {
                LogMessages.Dequeue();
            }
            
        }

        /// <summary>
        ///     update the log on the screen with the messages in the LogMessages Queue
        /// </summary>
        void UpdateLog() {
            Position logTextStart = LogTopLeft + 1;
            foreach (string s in LogMessages) {
                Engine.WriteText(logTextStart.ToPoint(), s, 6);
                logTextStart.Y++;
            }
        }

        /// <summary>
        ///     this is the override of the game loop provided by the Console Game Engine
        /// </summary>
        public override void Update() {
            if (GameState) {
                //this is the logic to determine if and where the player is moving
                int moveX = 0, moveY = 0;
                bool moved = false;
                if (Engine.GetKey(ConsoleKey.UpArrow) || Engine.GetKey(ConsoleKey.W)) {
                    moveY = -1;
                    moved = true;
                } else if (Engine.GetKey(ConsoleKey.LeftArrow) || Engine.GetKey(ConsoleKey.A)) {
                    moveX = -1;
                    moved = true;
                } else if (Engine.GetKey(ConsoleKey.DownArrow) || Engine.GetKey(ConsoleKey.S)) {
                    moveY = 1;
                    moved = true;
                } else if (Engine.GetKey(ConsoleKey.RightArrow) || Engine.GetKey(ConsoleKey.D)) {
                    moveX = 1;
                    moved = true;
                } else if (Engine.GetKey(keyClose)){
                    GameState = false;
                }
                //once we know the player has moved, we can calculate where they are going, as well as perform 
                //  logic related to the interaction of what they run into and moving the monsters aroudn the 
                //  map, as it is a turn-based game.
                if (moved) {
                    Position NextMove = You.XY;
                    if ((You.XY.X + moveX < c_MaxWinWidth - c_SideBar) && (You.XY.X + moveX > -1))
                        NextMove.X += moveX;
                    if ((You.XY.Y + moveY < c_MaxWinHeight) && (You.XY.Y + moveY > -1))
                        NextMove.Y += moveY;
                    //find out if they ran into anything
                    if (CurrentLevel.Grid[NextMove.X, NextMove.Y] != null) {
                        Object thing = CurrentLevel.Grid[NextMove.X, NextMove.Y];
                        bool attacked;
                        string message;
                        //make the player interact with the object run into
                        if (You.Interact(thing, out attacked, out message))
                            if (!attacked) {
                                if (thing is Item) {
                                    CurrentLevel.PickUpItem(thing as Item);
                                    MovePlayer(NextMove);
                                }
                                AddLog(message);
                            }//TODO: add code for attacking a monster
                    } else {
                        MovePlayer(NextMove);
                    }
                    //MoveMonsters();
                }
            } else {
                Environment.Exit(0); 
            }
        }

        public void MovePlayer(Position XY) {
            PlayerLastPosition = You.XY;
            You.XY = XY;
        }

        public void MoveMonsters() {
            //TODO: add code to move all monsters on the level, not just ones visible to the player.
        }

        /// <summary>
        ///     Implement the abstract Render() module to update the game screen
        /// </summary>
        public override void Render() {
            Engine.WriteText(You.XY.ToPoint(), You.Character.ToString(), 2);
            if (PlayerLastPosition != You.XY) {
            Engine.SetPixel(PlayerLastPosition.ToPoint(), BlankSpotColor, ConsoleCharacter.Full);
            }
            UpdateSideBar();
            UpdateLog();
            Engine.DisplayBuffer();
        }

        void Restart() {
            GameState = true;
            You.XY = new Position(0, 0);
            PlayerLastPosition = You.XY;
        }
        
        /// <summary>
        ///     Used this for testing, but may find a use for it in the future
        /// </summary>
        /// <param name="position">starting point, to be changed to another random point</param>
        void RandomizePixelPoint(ref Position position) {
            var newPosition = new Position();
            do {
                newPosition.X = RandomNum.Next(c_MaxWinWidth - c_SideBar - 1);
                newPosition.Y = RandomNum.Next(c_MaxWinHeight - c_SideBar - 1);
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

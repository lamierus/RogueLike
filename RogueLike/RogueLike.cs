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
        const int c_WinWidth = c_MaxWinWidth - 1; //170;
        const int c_WinHeight = c_MaxWinHeight - 1; //50;
        const int BlankSpotColor = 0;
        const int LevelWallColor = 2;
        const int InventoryTextColor = 3;
        const ConsoleKey keyClose = ConsoleKey.Escape;

        public Player You = new Player();
        public Position PlayerLastPosition;
        public LevelGrid CurrentLevel;
        
        private bool GameState = true;
        private Random RandomNum = new Random();

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
            Restart();
        }

        /// <summary>
        ///     currently only draws the rectangle around the edge, but can be modified to add "doorways" by blanking out sections,
        ///     depending on coordinates from a "map"
        /// </summary>
        void DrawFrame() {
            Engine.Rectangle(new Point(0, 0), new Point(c_WinWidth - c_SideBar, c_WinHeight), LevelWallColor, ConsoleCharacter.Light);
            
            int i = 0;
            //for (int y = 1; y <= 8; y++) {
                for (int x = 1; x <= 51/*96*/; x += 3) {
                    Engine.WriteText(new Point(x, c_WinHeight), i.ToString(" ##"), i++);
                }
            //}
            
        }

        void BuildLevel() {
            int LevelWidth = c_WinWidth - c_SideBar - 1;
            int levelHeight = c_WinHeight - 1;
            CurrentLevel = new LevelGrid(LevelWidth, levelHeight);
            List<Rectangle> rooms = new List<Rectangle>();
            List<Dungeon> dungeonParts = new List<Dungeon>();
            Dungeon dungeon = new Dungeon(LevelWidth, levelHeight, 1, 1);
            dungeonParts.Add(dungeon);

            bool didSplit = true;
            int splitIndex = 0;

            while (didSplit /*&& rooms.Count < 7*/) {
                didSplit = false;
                for (int i = 0; i <= dungeonParts.Count; i++) {
                    Dungeon toSplit = dungeonParts.ElementAt(splitIndex);
                    //if (dungeonParts[i].LeftBranch == null && dungeonParts[i].RightBranch == null) //if this leaf is not already split
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

            /*
            int splitIndex = 0;
            while (dungeonParts.Count < 9) {
                Dungeon toSplit = dungeonParts.ElementAt(splitIndex);
                if (toSplit.Split()) {
                    dungeonParts.Add(toSplit.LeftBranch);
                    dungeonParts.Add(toSplit.RightBranch);
                }
                splitIndex++;
            }
            */
            DrawLevel(rooms);
        }

        void DrawLevel(List<Rectangle> Rooms) {
            foreach (Rectangle R in Rooms) {
                Engine.Rectangle(R.TopLeft, R.BottomRight, R.WallColor, R.Wall);
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
            Engine.Rectangle(new Point(c_WinWidth - c_SideBar + 1, 0), new Point(c_WinWidth, c_WinHeight), 3, ConsoleCharacter.Light);
            Position TextXY = new Position(c_WinWidth - c_SideBar + 2, 2);
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
            Position TextXY = new Position(c_WinWidth - c_SideBar + 8, 3);
            Engine.WriteText(TextXY.ToPoint(), You.HP.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.MP.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.Atk.ToString(), InventoryTextColor);
            TextXY.Y++;
            Engine.WriteText(TextXY.ToPoint(), You.GoldAmt.ToString(), InventoryTextColor);


        }

        public override void Update() {
            if (GameState) {
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
                if (moved) {
                    Position NextMove = You.XY;
                    if ((You.XY.X + moveX < c_WinWidth - c_SideBar - 1) && (You.XY.X + moveX > 0))
                        NextMove.X += moveX;
                    if ((You.XY.Y + moveY < c_WinHeight - 1) && (You.XY.Y + moveY > 0))
                        NextMove.Y += moveY;
                    if (CurrentLevel.Grid[NextMove.X - 2, NextMove.Y - 2] != null) {
                        Object thing = CurrentLevel.Grid[NextMove.X, NextMove.Y];
                        bool attacked;
                        string message;
                        if (You.Interact(thing, out attacked, out message))
                            if (!attacked) {
                                CurrentLevel.PickUpItem(thing as Item);
                                //TODO: update the message somewhere on the screen
                                MovePlayer(NextMove);
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
            Engine.DisplayBuffer();
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

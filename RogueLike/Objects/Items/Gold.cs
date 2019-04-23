using ConsoleGameEngine;

namespace RogueLike {
    public class Gold : Item {
        public int Amount { get; set; }

        public Gold (int amount) {
            ID = 999;
            MapCharacter = '*';
            Color = 99;
            Amount = amount;
        }

        public Gold (int amount, Position Pos) {
            ID = 999;
            MapCharacter = '*';
            Color = 99;
            Amount = amount;
            XY = Pos;
        }
    }
}
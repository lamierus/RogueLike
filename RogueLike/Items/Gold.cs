using ConsoleGameEngine;

namespace RogueLike {
    public class Gold:Item {
        public int Amount { get; set; }

        public Gold(int amount) {
            Index = 999;
            Character = '*';
            Color = 99;
            Amount = amount;
        }

        public Gold(int amount, Point Pos) {
            Index = 999;
            Character = '*';
            Color = 99;
            Amount = amount;
            Position = Pos;
        }
    }
}

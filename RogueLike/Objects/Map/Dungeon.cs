using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike
{
    public class Dungeon
    {
        public Stack<DungeonFloor> CurrentDungeon;

        public Dungeon () {
            CurrentDungeon = new Stack<DungeonFloor>();
        }

        public bool AddFloor(DungeonFloor floor)
        {
            CurrentDungeon.Push(floor);
            return true;
        }
    }
}
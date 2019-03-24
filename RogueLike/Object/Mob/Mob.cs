using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Mob: Object {
        public string Name { get; set; }
        public int Atk { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public bool MagicUser { get; set; }
        public bool isAlive { get; set; }

        public Mob() {
        }
    }
}

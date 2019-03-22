using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Mob {
        public string Name { get; set; }
        public int ID { get; set; }
        public char Character { get; set; }
        public Color RGBColor { get; set; }
        public int Atk { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public bool MagicUser { get; set; }
        public bool Alive { get; set; }

        public Position XY;
    }
}

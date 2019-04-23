using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Structure : Object {
        public bool Immovable { get; set; }
        public bool FloorToCeiling { get; set; }
        public bool AboveOrBelow { get; set; }
    }
}
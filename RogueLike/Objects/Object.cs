using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Object {
        public ConsoleCharacter SolidCharacter { get; set; }
        public string TextCharacter { get; set; }
        public int Color { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public Position XY;
    }
    
}
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Wall:Structure {

        public Wall() {
            MapCharacter = ConsoleCharacter.Full;
            Immovable = true;
            FloorToCeiling = true;
        }
    }
}

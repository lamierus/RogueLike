using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Room:Map {
        public List<Item> ItemsInRoom = new List<Item>();
        public List<Mob> MobsInRoom = new List<Mob>();
    }
}

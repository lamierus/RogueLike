using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Object {
        public char Character { get; set; }
        public int Color { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }

        public Position XY;
    }
}

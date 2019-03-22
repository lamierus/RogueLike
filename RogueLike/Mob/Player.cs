using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Player : Mob {
        private Backpack Inventory;
        
        public Player() {
            Inventory = new Backpack(8);
        }

        public Backpack GetInventory() {
            return Inventory;
        }

        public bool PickUpItem (Item pickedUp) {
            return Inventory.AddItem(pickedUp);
        }

        public Item DropItem(int slot) {
            return Inventory.RemoveItem(slot);
        }
    }
}

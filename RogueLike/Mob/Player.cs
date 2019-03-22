using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Player : Mob {
        private Backpack Inventory;
        public int GoldAmt = 0;
        
        public Player() {
            Inventory = new Backpack(8);
            ID = 0;
            Name = "Player";
            Character = '@';
            RGBColor = new Color(255, 0, 255);
            Atk = 1;
            HP = 10;
            MP = 5;
            MagicUser = true;
            Alive = true;
        }

        public Backpack GetInventory() {
            return Inventory;
        }

        public bool PickUpItem (Item pickedUp) {
            //if (pickedUp.GetType())
            return Inventory.AddItem(pickedUp);
        }

        public Item DropItem(int slot) {
            return Inventory.RemoveItem(slot);
        }
    }
}

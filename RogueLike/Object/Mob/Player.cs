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
            Color = 2;
            Atk = 1;
            HP = 10;
            MP = 5;
            MagicUser = true;
            isAlive = true;
        }

        public Backpack GetInventory() {
            return Inventory;
        }

        private bool PickUpItem (Item pickedUp) {
            if (pickedUp is Gold) {
                GoldAmt += (pickedUp as Gold).Amount;
                return true;
            }
            return Inventory.AddItem(pickedUp);
        }

        public Item DropItem(int slot) {
            return Inventory.RemoveItem(slot);
        }
        
        public bool Interact(Object obj, out bool attacked) {
            if (obj is Item) {
                attacked = false;
                return PickUpItem(obj as Item);
            }
            attacked = true;
            return false;
        }
    }
}

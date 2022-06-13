using System;
using System.Collections.Generic;
using System.Text;
using ConsoleGameEngine;

namespace RogueLike {
    public class Player : Mob {
        private Backpack Inventory;
        public int GoldAmt = 0;

        public Player () {
            Inventory = new Backpack (8);
            ID = 0;
            Name = "Player";
            TextCharacter = "@";
            Color = 2;
            Atk = 1;
            HP = 10;
            MP = 5;
            MagicUser = true;
            isAlive = true;
        }

        public Backpack GetInventory () {
            return Inventory;
        }

        private bool PickUpItem (Item pickedUp, out string message) {
            if (pickedUp is Gold) {
                GoldAmt += (pickedUp as Gold).Amount;
                message = "Picked up " + (pickedUp as Gold).Amount.ToString () + " pieces of gold!";
                return true;
            }

            return Inventory.AddItem (pickedUp, out message);
        }

        public Item DropItem (int slot) {
            return Inventory.RemoveItem (slot);
        }

        public bool Interact (Object obj, out bool attacked, out string message) {
            message = null;
            attacked = false;
            if (obj is Item) {
                return PickUpItem (obj as Item, out message);
            }
            if (obj is Wall || obj is NullSpace) {
                return true;
            }
            if (obj is Door) {
                if ((obj as Door).Locked) {
                    message = "This door is locked!";
                    return false;
                }
                return true;
            }
            if (obj is Rug) {
                return true;
            }
            attacked = true;
            message = "Attacked a " + (obj as Mob).Name + "!";
            return false;
        }
    }
}
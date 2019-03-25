using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLike {
    public class Backpack :Item {
        public int Size { get; set; }
        private Item[] Items;

        public Backpack (int size) {
            Size = size;
            Items = new Item[Size];
        }

        public Item GetItemFromSlot (int slot) {
            return Items[slot];
        }

        public Item[] GetAllItems() {
            return Items;
        }

        public int GetItemIDFromSlot (int slot) {
            return Items[slot].ID;
        }

        public int[] GetAllItemIDs() {
            int[] itemIDs = new int[Size];
            for (int i = 0; i < Size; i++) {
                itemIDs[i] = Items[i].ID;
            }
            return itemIDs;            
        }

        public bool AddItem(Item itemToAdd, out string message) {
            for (int i = 0; i < Items.Length; i++) {
                if (Items.GetValue(i) == null) {
                    Items[i] = itemToAdd;
                    message = "Picked up " + itemToAdd.ID.ToString() + "!";
                    return true;
                }
            }
            message = "Couldn't pick up the item.";
            return false;
        }

        public Item RemoveItem (int slot) {
            Item removed = Items[slot];
            Items[slot] = null;
            return removed;
        }
    }
}

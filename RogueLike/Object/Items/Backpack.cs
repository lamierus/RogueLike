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

        public bool AddItem(Item itemToAdd) {
            bool added = false;
            for (int i = 0; i < Items.Length; i++) {
                if (Items.GetValue(i) == null) {
                    Items[i] = itemToAdd;
                    added = true;
                    break;
                }
            }
            return added;
        }

        public Item RemoveItem (int slot) {
            Item removed = Items[slot];
            Items[slot] = null;
            return removed;
        }
    }
}

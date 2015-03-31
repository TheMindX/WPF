using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace layout1
{

    class Item
    {
        public string desc
        {
            get;
            set;
        }
        public bool isChecked = false;
    }

    

    class ItemList
    {
        public string title = "title";
        public bool achived = false;
        public List<Item> mItems = new List<Item>();

        public Item addItem()
        {
            var item = new Item();
            mItems.Add(item);
            return item;
        }

        public void rmItem(Item item)
        {
            mItems.Remove(item);
        }

        public void changePosBefore(Item item)
        {
            var oldIdx = mItems.IndexOf(item);
            var newIdx = oldIdx - 1;
            if(newIdx<0)
            {
                newIdx = 0;
            }
            mItems.Remove(item);
            mItems.Insert(newIdx, item);
        }

        public void changePosAfter(Item item, Item item2)
        {
            var oldIdx = mItems.IndexOf(item);
            var newIdx = oldIdx;
            if (newIdx < 0)
            {
                newIdx = 0;
            }
            mItems.Remove(item);
            mItems.Insert(newIdx, item);
        }
    }

    class ItemNote
    {
        public List<ItemList> mItemLists = new List<ItemList>();
    }
}

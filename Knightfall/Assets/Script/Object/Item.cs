using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Object
{
    public class Item
    {
        public string itemName;
        public Sprite itemSprite;
        public Action TriggerEffect;

        public Item(string name, Sprite sprite, Action effect)
        {
            itemName = name;
            itemSprite = sprite;
            TriggerEffect = effect;
        }
    }

}

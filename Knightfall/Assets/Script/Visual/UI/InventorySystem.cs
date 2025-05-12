using System;
using System.Collections.Generic;
using Assets.Script.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Visual.UI
{
    public class InventorySystem : MonoBehaviour
    {
        public Image inventorySlot;  // UI slot
        public Image itemIcon;       // UI item icon
        public Sprite emptySlotSprite; // Gray placeholder sprite

        private List<Item> items = new List<Item>(); // Item icons
        private int selectedItemIndex = 0;
        private bool isInventoryOpen = false;

        // Separate width and height for better scaling
        private float defaultWidth = 80f, defaultHeight = 80f;
        private float expandedWidth = 100f, expandedHeight = 100f;

        void Update()
        {
            HandleInventoryToggle();
            HandleItemSelection();
            HandleItemUsage();
            UpdateUI();
        }

        void HandleInventoryToggle()
        {
            // Press "F" to toggle inventory size
            if (Input.GetKeyDown(KeyCode.F) && items.Count > 0)
            {
                isInventoryOpen = !isInventoryOpen;
                ResizeInventory();
            }
        }

        void ResizeInventory()
        {
            float newWidth = isInventoryOpen ? expandedWidth : defaultWidth;
            float newHeight = isInventoryOpen ? expandedHeight : defaultHeight;

            inventorySlot.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            itemIcon.rectTransform.sizeDelta = new Vector2(newWidth - 40, newHeight - 40);
        }

        void HandleItemSelection()
        {
            if (isInventoryOpen && items.Count > 1)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");

                if (scroll > 0f)
                {
                    selectedItemIndex = (selectedItemIndex + 1) % items.Count;
                }
                else if (scroll < 0f)
                {
                    selectedItemIndex = (selectedItemIndex - 1 + items.Count) % items.Count;
                }
            }
        }

        void HandleItemUsage()
        {
            if (isInventoryOpen && Input.GetKeyDown(KeyCode.E) && items.Count > 0)
            {
                items[selectedItemIndex].TriggerEffect?.Invoke();
                isInventoryOpen = !isInventoryOpen;
                ResizeInventory();
                RemoveItem(items[selectedItemIndex]);
            }
        }


        void UpdateUI()
        {
            if (items.Count == 0)
            {
                itemIcon.sprite = emptySlotSprite;
                itemIcon.color = Color.gray; // Gray when empty
            }
            else
            {
                Item currentItem = items[selectedItemIndex];
                itemIcon.sprite = currentItem.itemSprite;
                itemIcon.color = Color.white;
            }
        }

        public void AddItem(string name,Sprite itemSprite, Action action)
        {
            Item item = new Item(name, itemSprite, action);
            items.Add(item);
            selectedItemIndex = items.Count - 1;
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
            selectedItemIndex = Mathf.Clamp(selectedItemIndex, 0, items.Count - 1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // helps us work with UI elements
using TMPro;
using Inventory;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace HeadsUpDisplay
{
    public class InventoryUI : MonoBehaviour
    {
        // TODO needs implementation https://www.youtube.com/watch?v=ZSdzzNiDvZk&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&index=5 - swap items in inventory

        public MouseItem mouseItem = new MouseItem();
        public GameObject InventoryPrefab;
        public InventoryManager InventoryManager;
        public int XStart;
        public int YStart;
        public int XSpaceBetweenItems;
        public int YSpaceBetweenItems;
        public int NumberOfColumns;
        public float ItemSize;

        private List<GameObject> ItemsDisplayed;

        void Start()
        {
            CreateSlots();
        }
        void Update()
        {
            UpdateSlots();
            HandleUsedItems();
        }
        private void CreateSlots()
        {
            ItemsDisplayed = new List<GameObject>();
            AddEvent(this.gameObject, EventTriggerType.PointerEnter, delegate { OnEnter(this.gameObject); });
            AddEvent(this.gameObject, EventTriggerType.PointerExit, delegate { OnExit(this.gameObject); });

            for (int i = 0; i < InventoryManager.GetInventorySize(); i++)
            {
                var obj = Instantiate(InventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.transform.localScale = new Vector3(ItemSize, ItemSize, ItemSize);

                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
                // AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

                ItemsDisplayed.Add(obj);
            }
        }

        private void UpdateSlots()
        {
            // set how every item in the inventory will look like.
            for (int i = 0; i < ItemsDisplayed.Count; i++)
            {
                GameObject slot = ItemsDisplayed[i];
                int ID = InventoryManager.GetItems()[i].ID;

                // normal item slot
                if (ID > 0)
                {
                    // create background Image.
                    slot.transform.GetComponentInChildren<Image>().sprite = InventoryManager.InventoryIndexList.GetItemByID(ID).GetSprite();
                    slot.transform.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1); // TODO IDK why i need this but we will test it out

                    // create Text amount
                    slot.transform.GetComponentInChildren<Text>().text = InventoryManager.GetItems()[i].count.ToString();
                }
                // empty item slot
                else
                {
                    // create background Image.
                    slot.transform.GetComponentInChildren<Image>().sprite = null;
                    slot.transform.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);

                    // create Text amount
                    slot.transform.GetComponentInChildren<Text>().text = "";
                }
            }
        }

        private void HandleUsedItems()
        {
            if (Input.GetButtonDown(Finals.USE_ITEM) &&
            mouseItem.hoverItem != null &&
            InventoryManager.InventoryIndexList.GetItemByID(mouseItem.hoverItem.ID).IsConsumable())
            {
                InventoryManager.Consume(InventoryManager.GetItems().IndexOf(mouseItem.hoverItem));
            }
        }
        private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            var eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnEnter(GameObject obj)
        {
            mouseItem.hoverObj = obj;
            int i = ItemsDisplayed.IndexOf(obj);
            if (i >= 0)
                mouseItem.hoverItem = InventoryManager.GetItems()[i];
            else
                mouseItem.hoverItem = null;
        }

        public void OnExit(GameObject obj)
        {
            mouseItem.hoverObj = null;
            mouseItem.hoverItem = null;
        }

        public void OnDragStart(GameObject obj)
        {
            int i = ItemsDisplayed.IndexOf(obj);
            int ID = InventoryManager.GetItems()[i].ID;

            if (ID > 0)
            {
                var mouseObject = new GameObject();
                var rt = mouseObject.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(50, 50);
                mouseObject.transform.SetParent(transform.parent);

                var img = mouseObject.AddComponent<Image>();
                img.sprite = InventoryManager.InventoryIndexList.GetItemByID(ID).GetSprite();
                img.raycastTarget = false;

                mouseItem.obj = mouseObject;
                mouseItem.item = InventoryManager.GetItems()[i];
            }
        }
        public void OnDragEnd(GameObject obj)
        {
            if (mouseItem.hoverObj)
            {
                if (mouseItem.hoverItem != null)
                    InventoryManager.SwapItems(mouseItem.hoverItem, mouseItem.item);
            }
            else
            {
                InventoryManager.RemoveItemStack(ItemsDisplayed.IndexOf(obj));
            }
            Destroy(mouseItem.obj);
            mouseItem.item = null;
            mouseItem.hoverItem = null;
        }
        public void OnDrag(GameObject obj)
        {
            if (mouseItem.obj != null)
                mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        public Vector3 GetPosition(int i)
        {
            return new Vector3(XStart + (XSpaceBetweenItems * (i % NumberOfColumns)), YStart + (-YSpaceBetweenItems * (i / NumberOfColumns)), 0f);
        }
    }
}
public class MouseItem
{
    public GameObject obj;
    public InventoryManager.ItemInsideInventory item;
    public InventoryManager.ItemInsideInventory hoverItem;
    public GameObject hoverObj;
}
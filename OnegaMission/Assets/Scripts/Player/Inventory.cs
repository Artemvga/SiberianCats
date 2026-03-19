using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        [SerializeField] private int _maxSlots = 10;
        private List<InteractableBase> _items = new List<InteractableBase>();

        public int MaxSlots => _maxSlots;
        public int ItemsCount => _items.Count;
        public bool IsFull => _items.Count >= _maxSlots;
        
        public event Action OnInventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool AddItem(InteractableBase item)
        {
            if (IsFull) return false;
            _items.Add(item);
            Debug.Log($"Инвентарь: {_items.Count}/{_maxSlots}");
            OnInventoryChanged?.Invoke(); // вызов события
            return true;
        }

        public List<InteractableBase> GetItems() => new List<InteractableBase>(_items);
    }
}
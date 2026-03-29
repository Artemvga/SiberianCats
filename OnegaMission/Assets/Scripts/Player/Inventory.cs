using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: Inventory.cs
// Путь: Assets/Scripts/Player/Inventory.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Player
{
    /// <summary>
    /// Реализует компонент `Inventory` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        [SerializeField] private int _maxSlots = 10;
        private List<InteractableBase> _items = new List<InteractableBase>();

        public int MaxSlots => _maxSlots;
        public int ItemsCount => _items.Count;
        public bool IsFull => _items.Count >= _maxSlots;

        public event Action OnInventoryChanged;

        /// <summary>
        /// Инициализирует объект при создании компонента Unity.
        /// </summary>
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

        /// <summary>
        /// Выполняет операцию `AddItem` в рамках обязанностей текущего компонента.
        /// </summary>
        public bool AddItem(InteractableBase item)
        {
            if (IsFull) return false;
            _items.Add(item);
            Debug.Log($"Инвентарь: {_items.Count}/{_maxSlots} — добавлен {item.ItemName}");
            OnInventoryChanged?.Invoke();
            return true;
        }

        public List<InteractableBase> GetItems() => new List<InteractableBase>(_items);

        /// <summary>
        /// Выполняет операцию `TakeAllItems` в рамках обязанностей текущего компонента.
        /// </summary>
        public List<InteractableBase> TakeAllItems()
        {
            var list = new List<InteractableBase>(_items);
            _items.Clear();
            OnInventoryChanged?.Invoke();
            return list;
        }

        /// <summary>
        /// Выполняет операцию `AddItems` в рамках обязанностей текущего компонента.
        /// </summary>
        public void AddItems(List<InteractableBase> items)
        {
            _items.AddRange(items);
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Выполняет операцию `ClearInventory` в рамках обязанностей текущего компонента.
        /// </summary>
        public void ClearInventory()
        {
            foreach (var item in _items)
            {
                if (item != null && item.gameObject != null)
                    Destroy(item.gameObject);
            }
            _items.Clear();
            OnInventoryChanged?.Invoke();
        }
    }
}
using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: PlayerTools.cs
// Путь: Assets/Scripts/Player/PlayerTools.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Player
{
    /// <summary>
    /// Реализует компонент `PlayerTools` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public class PlayerTools : MonoBehaviour
    {
        public static PlayerTools Instance { get; private set; }

        private List<ToolItem> _tools = new List<ToolItem>();

        public event Action OnToolsChanged;

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
        /// Выполняет операцию `AddTool` в рамках обязанностей текущего компонента.
        /// </summary>
        public bool AddTool(ToolItem tool)
        {
            if (tool == null) return false;
            _tools.Add(tool);
            Debug.Log($"Инструмент {tool.ItemName} ({tool.ToolType}) добавлен");
            OnToolsChanged?.Invoke();
            return true;
        }

        public bool HasTool(ToolType type) => _tools.Exists(t => t.ToolType == type);
        public List<ToolItem> GetAllTools() => new List<ToolItem>(_tools);

        /// <summary>
        /// Выполняет операцию `ClearTools` в рамках обязанностей текущего компонента.
        /// </summary>
        public void ClearTools()
        {
            foreach (var tool in _tools)
            {
                if (tool != null && tool.gameObject != null)
                    Destroy(tool.gameObject);
            }
            _tools.Clear();
            OnToolsChanged?.Invoke();
        }
    }
}
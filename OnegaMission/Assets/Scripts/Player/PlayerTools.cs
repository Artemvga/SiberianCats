using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Player
{
    public class PlayerTools : MonoBehaviour
    {
        public static PlayerTools Instance { get; private set; }

        private List<ToolItem> _tools = new List<ToolItem>();

        public event Action OnToolsChanged;

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
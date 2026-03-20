using UnityEngine;

namespace Items
{
    /// <summary>
    /// ScriptableObject для определения типа мусора (стекло, пластик и т.д.).
    /// Позволяет централизованно хранить внутреннее имя и отображаемое название.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemType", menuName = "Items/Item Type")]
    public class ItemTypeSO : ScriptableObject
    {
        [SerializeField] private string _typeName;      // внутреннее имя (например, "Glass")
        [SerializeField] private string _displayName;   // отображаемое название (например, "Стекло")

        public string TypeName => _typeName;
        public string DisplayName => _displayName;
    }
}
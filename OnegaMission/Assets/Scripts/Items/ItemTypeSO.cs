using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "NewItemType", menuName = "Items/Item Type")]
    public class ItemTypeSO : ScriptableObject
    {
        [SerializeField] private string _typeName;
        [SerializeField] private string _displayName;

        public string TypeName => _typeName;
        public string DisplayName => _displayName;
    }
}
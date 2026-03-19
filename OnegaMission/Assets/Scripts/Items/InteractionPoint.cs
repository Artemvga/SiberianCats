using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Items
{
    public class InteractionPoint : InteractableBase
    {
        [Header("Interaction Requirements")]
        [SerializeField] private List<ToolType> _requiredTools;
        [SerializeField] private bool _requiresAll = true;

        [Header("Feedback")]
        [SerializeField] private string _successMessage = "Взаимодействие выполнено";
        [SerializeField] private string _failMessage = "Не хватает инструментов";

        public override bool CanInteract(PlayerTools tools)
        {
            if (tools == null) return false;
            if (_requiredTools == null || _requiredTools.Count == 0) return true;

            if (_requiresAll)
            {
                foreach (var tool in _requiredTools)
                    if (!tools.HasTool(tool)) return false;
                return true;
            }
            else
            {
                foreach (var tool in _requiredTools)
                    if (tools.HasTool(tool)) return true;
                return false;
            }
        }

        public override void Interact()
        {
            if (CanInteract(PlayerTools.Instance))
            {
                Debug.Log(_successMessage);
                // Здесь логика активации
            }
            else
            {
                Debug.Log(_failMessage);
            }
        }
    }
}
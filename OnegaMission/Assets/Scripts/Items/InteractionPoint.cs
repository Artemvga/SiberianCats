using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    /// <summary>
    /// Точка взаимодействия (дверь, рычаг, терминал).
    /// Может требовать наличия определённых инструментов.
    /// При неудаче показывает сообщение о требовании.
    /// </summary>
    public class InteractionPoint : InteractableBase
    {
        [Header("Interaction Requirements")]
        [SerializeField] private List<ToolType> _requiredTools;
        [SerializeField] private bool _requiresAll = true; // true - нужны все, false - хотя бы один

        [Header("Feedback")]
        [SerializeField] private string _successMessage = "Взаимодействие выполнено";
        [SerializeField] private string _failMessage = "Не хватает инструментов";

        public UnityEvent OnSuccess; // вызывается при успешном взаимодействии
        public UnityEvent OnFail;    // вызывается при неудаче

        // Показываем требование, если не хватает инструментов
        public override bool ShouldShowRequirement => true;

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
                OnSuccess?.Invoke();
                OnInteractEvent?.Invoke();
                // Здесь можно добавить анимацию, звук и т.д.
            }
            else
            {
                Debug.Log(_failMessage);
                OnFail?.Invoke();
            }
        }
    }
}
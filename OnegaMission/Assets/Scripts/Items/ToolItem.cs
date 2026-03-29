using Player;
using UnityEngine;

namespace Items
{
    public class ToolItem : InteractableBase
    {
        [SerializeField] private ToolType _toolType;
        public ToolType ToolType => _toolType;

        public override bool ShouldShowRequirement => false;
        public override bool CanInteract(PlayerTools tools) => true;

        public override void Interact()
        {
            if (PlayerTools.Instance != null && PlayerTools.Instance.AddTool(this))
            {
                GameObject handModel = Instantiate(gameObject);
                handModel.name = $"{ItemName}_Hand";
                foreach (var col in handModel.GetComponents<Collider>()) Destroy(col);
                var rb = handModel.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                if (ActiveTool.Instance != null)
                    ActiveTool.Instance.RegisterTool(_toolType, handModel);

                gameObject.SetActive(false);
                Debug.Log($"Инструмент {ItemName} ({_toolType}) добавлен");
                OnInteractEvent?.Invoke();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не удалось добавить инструмент");
            }
        }

        public override string GetInteractionMessage()
        {
            return "Нажмите E, чтобы взять инструмент";
        }
    }
}
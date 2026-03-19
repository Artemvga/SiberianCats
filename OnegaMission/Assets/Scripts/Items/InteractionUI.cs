using TMPro;
using UnityEngine;

namespace Items
{
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _loreText;
        [SerializeField] private TMP_Text _requirementText;

        private void Start() => Hide();

        public void Show(IInteractable interactable)
        {
            if (interactable == null)
            {
                Hide();
                return;
            }

            _nameText.text = interactable.ItemName;
            _typeText.text = interactable.ItemType;
            _descriptionText.text = interactable.Description;
            _loreText.text = interactable.Lore;
            _requirementText.text = "";

            gameObject.SetActive(true);
        }

        public void ShowRequirement(IInteractable interactable)
        {
            if (interactable == null)
            {
                Hide();
                return;
            }

            _nameText.text = interactable.ItemName;
            _typeText.text = interactable.ItemType;
            _descriptionText.text = interactable.Description;
            _loreText.text = interactable.Lore;
            _requirementText.text = "Требуется инструмент";

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
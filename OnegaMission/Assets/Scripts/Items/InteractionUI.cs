using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    /// <summary>
    /// Управляет отображением информации о текущем интерактивном объекте.
    /// Содержит текстовые поля для названия, типа, описания, легенды и сообщения о требовании.
    /// </summary>
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _loreText;
        [SerializeField] private TMP_Text _requirementText;

        public UnityEvent OnShow;
        public UnityEvent OnHide;

        private void Start() => Hide();

        /// <summary> Показывает информацию об объекте (без требования). </summary>
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
            OnShow?.Invoke();
        }

        /// <summary> Показывает информацию и сообщение о требовании. </summary>
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
            OnShow?.Invoke();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide?.Invoke();
        }
    }
}
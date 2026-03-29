using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    public class InteractionUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _contentPanel; // панель, которая включается/выключается

        [Header("Text Fields")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _typeText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _loreText;
        [SerializeField] private TMP_Text _promptText;

        public UnityEvent OnShow;
        public UnityEvent OnClear;

        private void Awake()
        {
            // При старте панель выключена
            if (_contentPanel != null)
                _contentPanel.SetActive(false);
            else
                Debug.LogWarning("InteractionUI: _contentPanel не назначен!");
        }

        public void Show(IInteractable interactable, string prompt)
        {
            if (interactable == null)
            {
                ClearContent();
                return;
            }

            // Заполняем текстовые поля
            if (_nameText != null) _nameText.text = interactable.ItemName;
            if (_typeText != null) _typeText.text = interactable.ItemType;
            if (_descriptionText != null) _descriptionText.text = interactable.Description;
            if (_loreText != null) _loreText.text = interactable.Lore;
            if (_promptText != null) _promptText.text = prompt;

            // Включаем панель
            if (_contentPanel != null) _contentPanel.SetActive(true);

            OnShow?.Invoke();
        }

        public void ClearContent()
        {
            // Очищаем текст (опционально)
            if (_nameText != null) _nameText.text = "";
            if (_typeText != null) _typeText.text = "";
            if (_descriptionText != null) _descriptionText.text = "";
            if (_loreText != null) _loreText.text = "";
            if (_promptText != null) _promptText.text = "";

            // Выключаем панель
            if (_contentPanel != null) _contentPanel.SetActive(false);

            OnClear?.Invoke();
        }
    }
}
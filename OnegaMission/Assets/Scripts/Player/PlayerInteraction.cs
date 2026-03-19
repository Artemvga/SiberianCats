using InputSystemProject;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _interactionDistance = 10f;
        [SerializeField] private InteractionUI _interactionUI;
        [SerializeField] private LayerMask _interactableLayer;

        private IInteractable _currentInteractable;

        private void Update()
        {
            CheckInteractable();
        }

        private void CheckInteractable()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (interactable.CanInteract(PlayerTools.Instance))
                        SetCurrentInteractable(interactable);
                    else
                    {
                        SetCurrentInteractable(null);
                        _interactionUI.ShowRequirement(interactable);
                    }
                    return;
                }
            }
            SetCurrentInteractable(null);
        }

        private void SetCurrentInteractable(IInteractable newInteractable)
        {
            if (_currentInteractable == newInteractable) return;

            if (_currentInteractable != null)
            {
                _currentInteractable.OnDefocus();
                _interactionUI.Hide();
            }

            _currentInteractable = newInteractable;

            if (_currentInteractable != null)
            {
                _currentInteractable.OnFocus();
                _interactionUI.Show(_currentInteractable);
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
                SetCurrentInteractable(null);
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
        }
    }
}
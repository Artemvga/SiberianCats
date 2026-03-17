using System;
using InputSystemProject;
using Items;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] public float gracePeriod = 0.2f;
        [SerializeField] Camera _camera;
        [SerializeField] float _intaxcaderactionDistance = 10f;

        [SerializeField] private TMP_Text _interactionText;
        [SerializeField] private GameObject _interactionUI;
        
        private IInteractable _currentInteractable;

        private void Update()
        {
            InteractionRay();
        }

        private void InteractionRay()
        {
            Ray ray = _camera.ScreenPointToRay(Vector3.one / 2f);
            RaycastHit hit;
            
            bool hitSomething = false;

            if (Physics.Raycast(ray, out hit, _intaxcaderactionDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Change();
                    _currentInteractable = interactable;
                    return;
                }
            }
        }
        
        private void OnInteractPerformed(InputAction.CallbackContext contex)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
            Debug.Log("OnInteractPerformed");
        }

        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
        }
    }
}
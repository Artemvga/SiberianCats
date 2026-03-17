using UnityEngine;

namespace Items
{
    public class Trash : MonoBehaviour, IInteractable
    {
        public void Change()
        {
            throw new System.NotImplementedException();
        }

        public void Interact()
        {
            Debug.Log("Trash");
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}
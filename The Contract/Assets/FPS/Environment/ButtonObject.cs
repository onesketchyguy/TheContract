using UnityEngine;

namespace FPS.Environment
{
    public class ButtonObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string tooltip = "";
        public string elementInfo { get => tooltip; }

        [SerializeField] private UnityEngine.Events.UnityEvent onActivate;

        public void OnInteract()
        {
            if (isActiveAndEnabled) onActivate?.Invoke();
        }
    }
}
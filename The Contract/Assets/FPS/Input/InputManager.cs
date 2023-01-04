using UnityEngine;

namespace FPS.Input
{
    public enum ButtonState
    {
        None = 0,
        Down = 1,
        Up = 2,
    }

    public class InputManager : MonoBehaviour
    {
        private IMove[] movables;
        private IMouse[] mice;
        private IInteractInput[] interactInputs;

        private ButtonState[] mouseButtons = new ButtonState[2];

        void Start()
        {
            movables = GetComponents<IMove>();
            mice = GetComponents<IMouse>();
            interactInputs = GetComponents<IInteractInput>();
        }

        void Update()
        {
            foreach (var movable in movables)
            {
                movable.SetMovement(new Vector3(UnityEngine.Input.GetAxisRaw("Horizontal"), 
                    UnityEngine.Input.GetKeyDown(KeyCode.Space) ? 1 : 0, UnityEngine.Input.GetAxisRaw("Vertical")), UnityEngine.Input.GetKey(KeyCode.LeftShift));
                movable.SetCamera(new Vector2(UnityEngine.Input.GetAxisRaw("Mouse X"), UnityEngine.Input.GetAxisRaw("Mouse Y")));
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl)) movable.ToggleCrouch();
            }

            for (int i = 0; i < 2; i++)
            {
                if (UnityEngine.Input.GetMouseButton(i) == false && !UnityEngine.Input.GetMouseButtonUp(i))
                {
                    mouseButtons[i] = ButtonState.None;
                }

                if (UnityEngine.Input.GetMouseButtonDown(i))
                {
                    mouseButtons[i] = ButtonState.Down;
                }

                if (UnityEngine.Input.GetMouseButtonUp(i))
                {
                    mouseButtons[i] = ButtonState.Up;
                }
            }

            foreach (var mouse in mice)
            {
                mouse.SetMouseInput(mouseButtons[0], mouseButtons[1]);
            }

            foreach (var interact in interactInputs)
            {
                interact.SetInteracting(UnityEngine.Input.GetKeyUp(KeyCode.E));
            }
        }
    }
}
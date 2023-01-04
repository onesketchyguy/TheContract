using UnityEngine;

namespace FPS.Input
{
    public interface IMove
    {
        public void SetMovement(Vector3 move, bool sprint);

        public void SetCamera(Vector2 cam);

        public void ToggleCrouch();
    }
}

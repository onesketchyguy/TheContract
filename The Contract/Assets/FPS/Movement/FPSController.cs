using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class FPSController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private CharacterController body = null;
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float strafeMultiplier = 0.8f;
        [SerializeField] private float runMultiplier = 2.1f;
        private Vector3 input;
        private float runInput;

        [Header("Camera")]
        [SerializeField] private Transform cameraPeg = null;
        [SerializeField] private float cameraSpeed = 5.0f;
        [SerializeField] private float camClamp = 60.0f;
        private float camInputY;

        [SerializeField] private float stepIntensity = 0.2f;
        [SerializeField] private float stepSpeed = 2f;
        [SerializeField] private float transitionSpeed = 2f;
        private float stepScale = 0.0f;
        private float cameraStartY = 0.0f;
        private float stepTime = 0.0f;
        private float stepDir = 1.0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            cameraStartY = cameraPeg.localPosition.y;
        }

        private void Update()
        {
            // Movement input
            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");

            runInput = Mathf.Lerp(runInput, Input.GetKey(KeyCode.LeftShift) ? 1.1f : -0.1f, 10.0f * Time.deltaTime);

            // Rotation input
            float xRot = cameraPeg.localRotation.eulerAngles.x + (Input.GetAxisRaw("Mouse Y") * -cameraSpeed);
            if (xRot < camClamp) xRot = camClamp;
            cameraPeg.localRotation = Quaternion.Euler(xRot, 0, 0);

            camInputY = body.transform.rotation.eulerAngles.y + (Input.GetAxisRaw("Mouse X") * cameraSpeed);
            body.transform.rotation = Quaternion.Euler(0, camInputY, 0);

            // Camera wobble stuff
            stepDir = Mathf.MoveTowards(stepDir, body.velocity.magnitude > 0 ? 1.5f : -0.1f, transitionSpeed * Time.deltaTime);
            stepScale = Mathf.Lerp(Mathf.Epsilon, stepIntensity * (runInput > 0.5f ? runInput : 0.5f), stepDir );
            stepTime += (stepSpeed * body.velocity.magnitude * Time.deltaTime) + Time.deltaTime;

            if (stepTime >= 2 * 3.14159) stepTime = 0.0f;

            cameraPeg.localPosition = new Vector3(0, cameraStartY + (stepScale * Mathf.Sin(stepTime)));

            // Jump input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var vel = body.velocity;
                vel.y = 100;
            }
        }

        private void FixedUpdate()
        {
            var mSpd = new Vector2(moveSpeed * strafeMultiplier, moveSpeed) * (runInput > 0.5f ? (runMultiplier * runInput) : 1);
            var dir = (transform.forward * input.z * mSpd.y) + (transform.right * input.x * mSpd.x);
            body.SimpleMove(dir);
        }
    }
}
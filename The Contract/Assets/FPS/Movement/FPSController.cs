using UnityEngine;

namespace FPS
{
    public class FPSController : MonoBehaviour, Input.IMove
    {
        [Header("Movement")]
        [SerializeField] private CharacterController body = null;
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float strafeMultiplier = 0.8f;
        [SerializeField] private float runMultiplier = 2.1f;
        [SerializeField] private float jumpHeight = 1.0f;
        private float jumpForce;
        [Range(0, 1)]
        [SerializeField] private float landSlowDown = 0.25f;

        private CapsuleCollider capsuleCol = null;
        
        [Header("Ground checking")]
        [SerializeField] private LayerMask groundMask = 0;
        [SerializeField] private Vector3 groundCheckPos = Vector3.zero;
        [SerializeField] private float groundDist = 0.4f;
        private Vector3 GetGroundCheckPosition() { return groundCheckPos + transform.position; }
        private bool grounded;
        private float groundTime = 0;
        private float airTime = 0;
        private Vector3 input;
        private float runInput;

        [Header("Camera")]
        [SerializeField] private Transform cameraPeg = null;
        [SerializeField] private float cameraSpeed = 5.0f;
        [SerializeField] private float camClamp = 90.0f;
        private float camInputY;
        private float camRotX = 0.0f;

        [Header("Animation")]
        [SerializeField] private float stepIntensity = 0.2f;
        [SerializeField] private float landIntensity = 0.5f;
        [SerializeField] private float jumpIntensity = 0.15f;
        [SerializeField] private float stepSpeed = 2f;
        [SerializeField] private float transitionSpeed = 5.0f;
        private float camSpd = 5.0f;
        private float stepScale = 0.0f;
        private float cameraStartY = 0.0f;
        private float stepTime = 0.0f;
        private float stepDir = 1.0f;
        private Vector3 camTarget;
        private float stepEval;

        [Header("Ladders")]
        [SerializeField] private float ladderSpeed = 5.0f;
        [SerializeField] private string ladderTag = "Ladder";
        private bool onLadder = false;
        private float ladderTop;
        private float ladderBottom;

        private Vector3 velocity = Vector3.zero;

        [Header("Crouching")]
        [SerializeField] private float capsuleHeight_standing;
        [SerializeField] private float capsuleHeight_crouched;
        private bool crouching = false;

        [Space]
        [SerializeField] private StepManager stepManager = null;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (capsuleCol == null)
            {
                capsuleCol = GetComponent<CapsuleCollider>();
                if (capsuleCol == null) capsuleCol = gameObject.AddComponent<CapsuleCollider>();
            }

            capsuleHeight_standing = body.height;
            capsuleCol.height = body.height;
            capsuleCol.center = body.center;
            capsuleCol.radius = body.radius + 0.1f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, GetGroundCheckPosition());
            Gizmos.DrawWireSphere(GetGroundCheckPosition(), groundDist);
        }
#endif

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            cameraStartY = cameraPeg.localPosition.y;

            jumpForce = Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y);
        }

        private void Update()
        {
            if (!onLadder)
            {
                BaseMovement();

                capsuleCol.height = body.height = crouching ? capsuleHeight_crouched : capsuleHeight_standing;
            }
            else
            {
                ClimbLadder();
            }
        }

        private void ClimbLadder()
        {
            void JumpOffLadder()
            {
                body.Move(transform.forward * -0.1f);
                onLadder = false;
            }

            if (transform.position.y > ladderTop && input.z > 0)
            {
                body.Move((transform.forward * 0.5f) + (transform.up * 0.5f));
                onLadder = false;
            }

            if (transform.position.y < ladderBottom + 1.25f && input.z < 0) JumpOffLadder();
            if (input.y > 0) JumpOffLadder();

            body.Move(transform.up * input.z * ladderSpeed * Time.deltaTime);
        }

        private void BaseMovement()
        {
            bool groundedLastFrame = grounded;
            grounded = Physics.CheckSphere(GetGroundCheckPosition(), groundDist, groundMask);
            float camStart = crouching ? cameraStartY - (capsuleHeight_standing - capsuleHeight_crouched) : cameraStartY;

            if (grounded)
            {
                if (groundTime < 1) groundTime += Time.deltaTime;
                airTime = 0;
            }
            else
            {
                if (airTime < 1) airTime += Time.deltaTime;
                groundTime = 0.0f;
                stepTime = 0.0f;
            }

            // Camera wobble stuff
            if (groundedLastFrame != grounded)
            {
                // Jump and land
                if (groundedLastFrame == false && groundTime >= Time.deltaTime)
                {
                    camTarget = new Vector3(0, camStart - landIntensity);
                    stepManager?.Land();
                }
                else camTarget = new Vector3(0, camStart + jumpIntensity);

                camSpd = transitionSpeed * 10.0f;
            }
            else if (grounded && groundTime > 0.25f)
            {
                // Steps
                stepEval = Mathf.Sin(stepTime);
                stepDir = Mathf.MoveTowards(stepDir, body.velocity.magnitude > 0 ? 1.5f : -0.1f, transitionSpeed * Time.deltaTime);
                stepScale = Mathf.Lerp(Mathf.Epsilon, stepIntensity * (runInput > 0.5f ? runInput : 0.5f), stepDir);
                stepTime += (stepSpeed * body.velocity.magnitude * Time.deltaTime) + Time.deltaTime;

                if (stepTime >= 6.28318) stepTime = 0.0f;
                camTarget = new Vector3(0, camStart + (stepScale * stepEval));

                if (stepEval <= -0.9f && camTarget.y < camStart) stepManager?.TakeStep();

                camSpd = transitionSpeed;
            }

            cameraPeg.localPosition = Vector3.MoveTowards(cameraPeg.localPosition, camTarget, camSpd * Time.deltaTime);

            // Apply physics
            var mSpd = new Vector2(moveSpeed * strafeMultiplier, moveSpeed) * (runInput > 0.5f ? (runMultiplier * runInput) : 1);

            mSpd *= grounded ? Mathf.Clamp(groundTime, landSlowDown, 1.0f) : 1.0f - Mathf.Clamp(airTime, 0.0f, 0.75f);

            var dir = (transform.forward * input.z * mSpd.y) + (transform.right * input.x * mSpd.x);

            if (airTime > 0.1f) velocity += Physics.gravity * Time.deltaTime;
            else if (input.y > 0) velocity = transform.up * jumpForce;

            velocity.x = dir.x;
            velocity.z = dir.z;

            body.Move(velocity * Time.deltaTime); // apply velocity
        }

        public void SetMovement(Vector3 move, bool sprint)
        {
            // Movement input
            input = move;
            runInput = Mathf.Lerp(runInput, sprint ? 1.1f : -0.1f, 10.0f * Time.deltaTime);

            // Jump input
            if (move.y > 0) stepManager?.Jump();
        }

        public void SetCamera(Vector2 cam)
        {
            // Camera input
            camRotX -= (cam.y * cameraSpeed);
            camRotX = Mathf.Clamp(camRotX, -camClamp, camClamp);
            cameraPeg.localRotation = Quaternion.Euler(camRotX, 0, 0);

            camInputY = body.transform.rotation.eulerAngles.y + (cam.x * cameraSpeed);
            body.transform.rotation = Quaternion.Euler(0, camInputY, 0);
        }

        public void ToggleCrouch() => crouching = !crouching;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(ladderTag))
            {
                var bounds = collision.collider.bounds;

                ladderBottom = bounds.center.y - bounds.extents.y;
                ladderTop = bounds.center.y + bounds.extents.y;

                onLadder = true;
            }
        }
    }
}
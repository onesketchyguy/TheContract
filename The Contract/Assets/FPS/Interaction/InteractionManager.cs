using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPS.UI;

namespace FPS.Interaction
{
    public class InteractionManager : MonoBehaviour, Input.IMouse, Input.IInteractInput
    {
        [SerializeField] private Transform rayparent = null;
        [SerializeField] private LayerMask interactableMask = 0;
        [SerializeField] private float interactionDistance = 3.0f;
        private Rigidbody highlighting = null;
        private IInteractable interactable;
        private bool holding = false;
        private bool throwInput = false;
        private bool pickupInput = false;
        private bool interact = false;
        [SerializeField] private TooltipManager tooltip = null;

        [SerializeField] HighlightPlus.HighlightProfile highlightProfile = null;

        private void Update()
        {
            HandleHolding();

            if (Time.frameCount % 3 != 0) return;

            if (highlighting == null && interactable == null)
            {
                RaycastHit hit;

                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rayparent.position, rayparent.TransformDirection(Vector3.forward), out hit, interactionDistance, interactableMask))
                {
                    interactable = hit.transform.gameObject.GetComponent<IInteractable>();
                    highlighting = hit.transform.GetComponent<Rigidbody>();

                    if (highlighting != null) GetTrigger().Highlight(true);
                }
            }
            else if (highlighting != null)
            {
                if (throwInput)
                {
                    throwInput = false;

                    highlighting.useGravity = true;
                    highlighting.AddForce(rayparent.forward * 10, ForceMode.Impulse);
                    DropItem();
                    return;
                }

                if (pickupInput)
                {
                    pickupInput = false;

                    if (holding == false)
                    {
                        highlighting.useGravity = false;
                        holding = true;
                    }
                    else
                    {
                        DropItem();
                        return;
                    }
                }

                if (holding == false)
                {
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(rayparent.position, rayparent.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, interactableMask))
                    {
                        if (hit.transform != highlighting.transform) DropItem();
                    } 
                    else
                    {
                        DropItem();
                    }
                }
            }
            else if (interactable != null)
            {
                tooltip.Set(interactable.elementInfo, 10);

                if (interact)
                {
                    interactable.OnInteract();
                }

                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rayparent.position, rayparent.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, interactableMask))
                {
                    var interactable = hit.transform.gameObject.GetComponent<IInteractable>();
                    if (interactable != null && interactable != this.interactable) DropItem();
                }
                else
                {
                    DropItem();
                }
            }

            throwInput = false;
            pickupInput = false;
        }

        private void HandleHolding()
        {
            if (holding == true)
            {
                float holdDist = 1.5f;

                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rayparent.position, rayparent.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, ~interactableMask))
                {
                    if (hit.distance < holdDist) holdDist = hit.distance - 0.5f;
                }

                if (holdDist < 0.75f)
                {
                    DropItem();
                }
                else
                {
                    highlighting.velocity = Vector3.zero;
                    var targetPos = Vector3.Slerp(highlighting.position, rayparent.transform.position + rayparent.transform.forward * holdDist, 8.5f * Time.deltaTime);
                    highlighting.MovePosition(targetPos);
                }
            }
        }

        private void DropItem()
        {
            if (highlighting != null)
            {
                GetTrigger().Highlight(false);

                highlighting.useGravity = true;
                holding = false;
                highlighting = null;
            }
            interactable = null;
        }

        private HighlightPlus.HighlightTrigger GetTrigger()
        {
            if (highlighting == null) return null;

            var highlightEffect = highlighting.gameObject.GetComponent<HighlightPlus.HighlightTrigger>();
            if (highlightEffect == null)
            {
                var hl = highlighting.gameObject.AddComponent<HighlightPlus.HighlightEffect>();
                hl.profile = highlightProfile;
                hl.ProfileReload();

                highlightEffect = highlighting.gameObject.AddComponent<HighlightPlus.HighlightTrigger>();
                highlightEffect.highlightOnHover = false;
                highlightEffect.triggerMode = HighlightPlus.TriggerMode.Volume;
                highlightEffect.volumeLayerMask = 0;
            }

            return highlightEffect;
        }

        public void SetMouseInput(Input.ButtonState left, Input.ButtonState right)
        {
            if (left == Input.ButtonState.Up) throwInput = true;
            if (right == Input.ButtonState.Up) pickupInput = true;
        }

        public void SetInteracting(bool v)
        {
            interact = v;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (rayparent != null)
            {
                Gizmos.color = highlighting != null ? Color.green : Color.red;
                Gizmos.DrawRay(rayparent.position, rayparent.TransformDirection(Vector3.forward) * interactionDistance);
            }
        }
#endif
    }
}
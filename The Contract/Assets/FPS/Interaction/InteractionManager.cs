using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private Transform rayparent = null;
        [SerializeField] private LayerMask interactableMask = 0;
        [SerializeField] private float interactionDistance = 3.0f;
        private Rigidbody highlighting = null;
        private bool holding = false;
        private bool throwInput = false;
        private bool pickupInput = false;

        [SerializeField] HighlightPlus.HighlightProfile highlightProfile = null;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0)) throwInput = true;
            if (Input.GetMouseButtonUp(1)) pickupInput = true;
            HandleHolding();

            if (Time.frameCount % 3 != 0) return;

            if (highlighting == null)
            {
                RaycastHit hit;

                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(rayparent.position, rayparent.TransformDirection(Vector3.forward), out hit, interactionDistance, interactableMask))
                {
                    highlighting = hit.transform.GetComponent<Rigidbody>();

                    if (highlighting != null) GetTrigger().Highlight(true);
                }
            }
            else
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
                    var targetPos = Vector3.Slerp(highlighting.position, rayparent.transform.position + rayparent.transform.forward * holdDist, 10.0f * Time.deltaTime);
                    highlighting.MovePosition(targetPos);
                }
            }
        }

        private void DropItem()
        {
            GetTrigger().Highlight(false);

            highlighting.useGravity = true;
            holding = false;
            highlighting = null;
        }

        private HighlightPlus.HighlightTrigger GetTrigger()
        {
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
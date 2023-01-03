using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Environment
{
    public class Window : MonoBehaviour, IInteractable
    {
        [SerializeField] private Vector3 enterPoint;
        [SerializeField] private Vector3 exitPoint;

        [SerializeField] private string tooltipInfo = "Move through window";

        [SerializeField] private Door windowObject;

        public string elementInfo { get => (windowObject.isOpen ? tooltipInfo : ""); }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (enterPoint == Vector3.zero && exitPoint == Vector3.zero)
            {
                enterPoint = transform.forward; exitPoint = -transform.forward; 
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + enterPoint, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + exitPoint, 0.5f);
        }
#endif

        public void OnInteract()
        {
            // FIXME: Get the player some other way, maybe a nearby collision check
            var player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(MoveThroughWindow(player));
        }
        
        private IEnumerator MoveThroughWindow(GameObject obj)
        {
            if (!windowObject.isOpen) yield break;

            yield return null;
            var pointA = transform.position + enterPoint;
            var pointB = transform.position + exitPoint;

            var distA = Vector3.Distance(obj.transform.position, pointA);
            var distB = Vector3.Distance(obj.transform.position, pointB);

            var rb = obj.GetComponent<Rigidbody>();
            var cc = obj.GetComponent<CharacterController>();
            var cols = obj.GetComponents<Collider>();

            cc.enabled = false;
            if (rb != null) rb.detectCollisions = false;
            foreach (var item in cols) item.enabled = false;

            if (distA > distB)
            {
                while (distA > 0.1f)
                {
                    obj.transform.position = Vector3.Lerp(obj.transform.position, pointA, 5.0f * Time.deltaTime);
                    distA = Vector3.Distance(obj.transform.position, pointA);
                    yield return null;
                }
            }
            else
            {
                while (distB > 0.1f)
                {
                    obj.transform.position = Vector3.Lerp(obj.transform.position, pointB, 5.0f * Time.deltaTime);
                    distB = Vector3.Distance(obj.transform.position, pointB);
                    yield return null;
                }
            }

            cc.enabled = true;
            if (rb != null) rb.detectCollisions = true;
            foreach (var item in cols) item.enabled = true;
        }
    }
}
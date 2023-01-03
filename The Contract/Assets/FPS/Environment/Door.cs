using System.Collections;
using UnityEngine;

namespace FPS.Environment
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform doorObject = null;
        [SerializeField] private string tooltipInfo = "Door";

        public bool isOpen = false;

        [SerializeField] private Vector3 closedPoint, openPoint;
        [SerializeField] private float closedRot = 0, openRot = 90;

        [SerializeField] private float moveSpd = 10.0f;
        [SerializeField] private float rotSpd = 150.0f;

        public string elementInfo { get => tooltipInfo; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (doorObject == null) doorObject = transform;
            if (isOpen && openPoint == Vector3.zero) openPoint = transform.position;
            else if (!isOpen && closedPoint == Vector3.zero) closedPoint = transform.position;
        }
#endif

        public void OnInteract()
        {
            isOpen = !isOpen;
            StartCoroutine(UpdateDoor());
        }

        private IEnumerator UpdateDoor()
        {
            var targetPoint = isOpen ? openPoint : closedPoint;
            var targetRot = isOpen ? openRot : closedRot;
            var _rotSpd = isOpen ? rotSpd : -rotSpd;

            while (true)
            {
                bool e = true;
                yield return null;

                // Update position
                if (Vector3.Distance(doorObject.position, targetPoint) > 0.1f)
                {
                    doorObject.position = Vector3.MoveTowards(doorObject.position, targetPoint, moveSpd * Time.deltaTime);
                    e = false;
                }

                // Update rotation
                if (Mathf.Abs(doorObject.localRotation.eulerAngles.y - targetRot) > 4.99f)
                {
                    doorObject.Rotate(doorObject.up, _rotSpd * Time.deltaTime);
                    e = false;
                }

                if (e == true) break;
            }

            // Set the data directly to prevent miscalculations
            doorObject.position = targetPoint;
            doorObject.localRotation = Quaternion.Euler(Vector3.up * targetRot);
        }
    }
}
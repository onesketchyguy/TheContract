using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Environment
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform doorObject = null;
        [SerializeField] private string tooltipInfo = "Door";
        const float DOOR_SPEED = 150.0f;

        private bool isOpen = false;

        public string elementInfo { get => tooltipInfo; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (doorObject == null) doorObject = transform;
        }
#endif

        public void OnInteract()
        {
            if (isOpen == false)
            {
                StartCoroutine(OpenDoor());
                isOpen = true;
            }
            else
            {
                StartCoroutine(CloseDoor());
                isOpen = false;
            }
        }

        private IEnumerator OpenDoor()
        {
            yield return null;

            while (doorObject.localRotation.eulerAngles.y < 85)
            {
                yield return null;
                doorObject.Rotate(doorObject.up, DOOR_SPEED * Time.deltaTime);
            }

            doorObject.localRotation = Quaternion.Euler(Vector3.up * 90);
        }

        private IEnumerator CloseDoor()
        {
            yield return null;

            while (doorObject.localRotation.eulerAngles.y > 5)
            {
                yield return null;
                doorObject.Rotate(doorObject.up, -DOOR_SPEED * Time.deltaTime);
            }

            doorObject.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
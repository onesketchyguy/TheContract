using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private Transform playerBody = null;
        [SerializeField] private RawImage turner = null;
        [SerializeField] private Image compassItemPrefab = null;

        private CompassItem[] compassItems = null;
        private Image[] icons = null;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (playerBody == null)
            { 
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerBody = p.transform;
            }
        }
#endif

        public void Update()
        {
            // Turn compass
            var rect = turner.uvRect;
            rect.position = Vector2.right * (playerBody.transform.rotation.eulerAngles.y / 360);
            turner.uvRect = rect;

            // Show items
            if (compassItems != null)
            {
                int r = -1;
                for (int i = 0; i < compassItems.Length; i++)
                {
                    if (compassItems[i] == null)
                    {
                        r = i;
                        continue;
                    }

                    Vector3 dir = (new Vector3(compassItems[i].transform.position.x, playerBody.position.y, compassItems[i].transform.position.z) - playerBody.position).normalized;
                    float angle = Vector3.SignedAngle(playerBody.transform.forward, dir, Vector3.up) / 180;

                    if (icons[i].gameObject.activeSelf == false) icons[i].gameObject.SetActive(true);
                    icons[i].sprite = compassItems[i].icon;
                    icons[i].rectTransform.localPosition = Vector2.right * angle * turner.rectTransform.sizeDelta.x;
                }

                if (r != -1)
                {
                    // Hide the old icon
                    icons[r].sprite = null;
                    icons[r].gameObject.SetActive(false);

                    // Remove one item from the compass list
                    var old = compassItems;
                    compassItems = new CompassItem[old.Length - 1];

                    for (int j = 0, i = 0; i < old.Length; i++, j++)
                    {
                        if (i == r)
                        {
                            j--;
                            continue;
                        }

                        compassItems[i] = old[j];
                    }
                }
            }

            // Update the icons list
            if (Time.frameCount % 30 == 0 || compassItems == null)
            {
                compassItems = FindObjectsOfType<CompassItem>();

                // Match the items length with the icons length
                if (icons == null || compassItems.Length > icons.Length)
                {
                    var oldIcons = icons;
                    icons = new Image[compassItems.Length];

                    for (int i = 0; i < compassItems.Length; i++)
                    {
                        if (oldIcons != null && i < oldIcons.Length) icons[i] = oldIcons[i];
                        else icons[i] = Instantiate(compassItemPrefab, turner.transform);
                    }
                }
            }
        }
    }
}
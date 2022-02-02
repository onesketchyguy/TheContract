using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.UI
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private Text tooltip = null;
        private int priority = 0;
        private float tipTime = 0.0f;

        public void Set(string tip, int priority = 0)
        {
            if (priority >= this.priority)
            {
                tooltip.text = tip;
                this.priority = priority;
                tipTime = 0.25f;
            }
        }

        private void Update()
        {
            if (tipTime <= 0)
            {
                tooltip.text = "";
                this.priority = 0;
            } else tipTime -= Time.deltaTime;
        }
    }
}
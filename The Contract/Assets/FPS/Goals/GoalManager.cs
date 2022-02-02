using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FPS.Goals
{
    public class GoalManager : MonoBehaviour
    {
        [SerializeField] private Text goalText;
        private int priority = 0;

        [SerializeField] private UnityEngine.Events.UnityEvent onGoalsEmpty;
        private bool goalsEmpty = false;
        [SerializeField] private UnityEngine.Events.UnityEvent onGoalsExist;
        private bool goalsExist = false;

        private Dictionary<string, int> goals = new Dictionary<string, int>();

        public void SetGoal(string goal, int priority = 0)
        {
            if (priority >= this.priority)
            {
                goalText.text = goal;
                this.priority = priority;
            }

            int p;
            if (goals.TryGetValue(goal, out p) == false) goals.Add(goal, priority);
        }

        public void CompleteGoal(string goal)
        {
            goalText.text = "";
            priority = 0;

            foreach (var item in goals)
            {
                if (item.Key != goal && item.Value > priority)
                {
                    priority = item.Value;
                    goalText.text = item.Key;
                }
            }

            int p;
            if (goals.TryGetValue(goal, out p) == true)
            {
                goals.Remove(goal);
            }
        }

        private void Update()
        {
            if (goals.Count == 0 && goalsEmpty == false)
            {
                onGoalsEmpty?.Invoke();
                goalsEmpty = true;
                goalsExist = false;
            }
            else if (goals.Count > 0 && goalsExist == false)
            {
                onGoalsExist?.Invoke();
                goalsExist = true;
                goalsEmpty = false;
            }
        }
    }
}
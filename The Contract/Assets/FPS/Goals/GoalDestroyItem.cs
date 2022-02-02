using UnityEngine;

namespace FPS.Goals
{
    [RequireComponent(typeof(Collider))]
    public class GoalDestroyItem : MonoBehaviour
    {
        [SerializeField] private GoalItem goalItem = null;

        private GoalManager gm;

        private void Start()
        {
            gm = FindObjectOfType<GoalManager>();
        }

        private void LateUpdate()
        {
            if (goalItem != null && goalItem.inPossession) gm.SetGoal($"Destroy {goalItem.itemName}", goalItem.priority - 1);

            if (goalItem == null)
            {
                gm.CompleteGoal($"Destroy {goalItem.itemName}");
                this.enabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (goalItem != null && collision.gameObject == goalItem.gameObject)
            {
                goalItem.ForceIsComplete();
                Destroy(goalItem.gameObject, 2 * Time.deltaTime);

                gm.CompleteGoal($"Destroy {goalItem.itemName}");
            }
        }
    }
}
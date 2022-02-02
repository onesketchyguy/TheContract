using UnityEngine;

namespace FPS.Goals
{
    public class GoalItem : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidBody = null;
        [SerializeField] internal string itemName = "";
        [SerializeField] internal int priority;
        [SerializeField] private float minHoldTime = 1.0f;
        private float holdTime;

        internal bool inPossession;

        private GoalManager gm;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rigidBody == null) rigidBody = GetComponent<Rigidbody>();
        }
#endif

        private void Start()
        {
            gm = FindObjectOfType<GoalManager>();
            gm.SetGoal($"Aquire {itemName}", priority);
        }

        private void Update()
        {
            // NOTE: This is how we're detecting if the player is holding this... Not great.
            if (rigidBody.useGravity == false)
            {
                // Player has this
                gm.CompleteGoal($"Aquire {itemName}");
                inPossession = true;

                holdTime = minHoldTime;
            }
        }

        public void ForceIsComplete()
        {
            gm.CompleteGoal($"Aquire {itemName}");
            this.enabled = false;
        }
    }
}
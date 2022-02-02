using UnityEngine;

namespace FPS
{
    public class StepManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource = null;
        // FIXME: Add different surfaces
        [SerializeField] private AudioClip[] stepClips = null;
        [SerializeField] private AudioClip[] landClips = null;
        [SerializeField] private AudioClip[] jumpClips = null;

        private float stepTime = 0.0f;
        private float landTime = 0.0f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (audioSource != null) audioSource = GetComponent<AudioSource>();
        }
#endif

        public void TakeStep()
        {
            if (Time.time - stepTime < 0.1f) return;
            stepTime = Time.time;
            audioSource.PlayOneShot(stepClips[Random.Range(0, stepClips.Length)]);
        }

        public void Land()
        {
            if (Time.time - landTime < 0.1f) return;
            landTime = Time.time;
            audioSource.PlayOneShot(landClips[Random.Range(0, landClips.Length)]);
        }
        public void Jump()
        {
            if (Time.time - landTime < 0.1f) return;
            landTime = Time.time;
            audioSource.PlayOneShot(jumpClips[Random.Range(0, jumpClips.Length)]);
        }
    }
}
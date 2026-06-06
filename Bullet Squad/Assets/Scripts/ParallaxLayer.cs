using UnityEngine;

namespace BulletSquad
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] private float factor = 0.35f;
        private Transform cam;
        private Vector3 startPosition;

        private void Start()
        {
            cam = Camera.main.transform;
            startPosition = transform.position;
        }

        private void LateUpdate()
        {
            transform.position = startPosition + new Vector3(cam.position.x * factor, 0f, 0f);
        }
    }
}

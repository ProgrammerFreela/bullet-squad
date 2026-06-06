using UnityEngine;

namespace BulletSquad
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float minX;
        [SerializeField] private float maxX = 80f;
        [SerializeField] private Vector3 offset = new Vector3(2f, 1.5f, -10f);
        [SerializeField] private float smooth = 5f;

        private void LateUpdate()
        {
            if (target == null)
            {
                PlayerController player = FindFirstObjectByType<PlayerController>();
                target = player != null ? player.transform : null;
            }

            if (target == null)
            {
                return;
            }

            Vector3 desired = target.position + offset;
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, -1f, 6f);
            transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
        }
    }
}

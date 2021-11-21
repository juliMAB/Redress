using UnityEngine;

namespace GuilleUtils.Animations
{
    public class Swing : MonoBehaviour
    {
        private Vector3 rotation = Vector3.zero;

        [SerializeField] private bool swingingToRight = true;
        [SerializeField] private float maxSwingValue = 16;
        [SerializeField] private float speed = 5;

        private void Awake()
        {
            rotation = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            rotation.z += swingingToRight ? -Time.deltaTime * speed : Time.deltaTime * speed;

            transform.rotation = Quaternion.Euler(rotation);

            if (Mathf.Abs(rotation.z) > maxSwingValue)
            {
                swingingToRight = !swingingToRight;
            }
        }
    }
}
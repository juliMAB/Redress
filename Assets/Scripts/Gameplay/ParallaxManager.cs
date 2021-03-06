using UnityEngine;

namespace Redress.Gameplay.Platforms
{
    public class ParallaxManager : MonoBehaviour
    {
        [System.Serializable] 
        public struct BackgroundGroup
        {
            public GameObject[] layers;
            public Transform end;
            public Vector3[] startPosition;

            public float[] localXDifference;

            private float _speed;
            private float _layerSpeedDiff;
            private Transform _startPos;

            public void UpdateGroup(float speed, float layerSpeedDiff, Transform startPos, bool doParallax)
            {
                _speed = speed;
                _layerSpeedDiff = layerSpeedDiff;
                _startPos = startPos;

                MoveLayers(doParallax);

                if (doParallax)
                {
                    layers[3].GetComponentInChildren<SpriteRenderer>().sortingOrder = -10;
                }

                if (IsGroupOutOfScreen())
                {
                    ResetGroup();
                    ChangeEnabledParallaxGroup();
                    layers[3].GetComponentInChildren<SpriteRenderer>().sortingOrder = -11;
                }
            }

            private void MoveLayers(bool doParallax)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].transform.localPosition += Vector3.left * _speed * Time.deltaTime; // muevo todas las layers lo mismo

                    if (doParallax)
                    { 
                        layers[i].transform.localPosition += Vector3.right * i * _layerSpeedDiff * Time.deltaTime;
                        // hago retroceder cada vez un poquito más a las layers de más atras que son las que mas lento deberían ir
                    }
                }
            }

            private void ResetGroup()
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    Vector3 position = _startPos.position;
                    position.z = startPosition[i].z;
                    layers[i].transform.position = position + Vector3.right * localXDifference[i];
                }
            }

            private bool IsGroupOutOfScreen()
            {
                return end.position.x < -halfScreenSize - 0.2f;
            }
        }

        [SerializeField] private BackgroundGroup group1;
        [SerializeField] private BackgroundGroup group2;
        [SerializeField] private float layerSpeedDiff = 0.5f;
        [SerializeField] private float speed = 5;
        [SerializeField] private static bool enabledGroup1 = true;
        [SerializeField] private static float halfScreenSize = 8.9f;


        private void Start()
        {
            group1.startPosition = new Vector3[group1.layers.Length];
            group2.startPosition = new Vector3[group2.layers.Length];

            for (int i = 0; i < group1.layers.Length; i++)
            {
                group1.startPosition[i] = group1.layers[i].transform.localPosition;
            }

            for (int i = 0; i < group2.layers.Length; i++)
            {
                group2.startPosition[i] = group2.layers[i].transform.localPosition;
            }
        }

        public static void ChangeEnabledParallaxGroup()
        {
            enabledGroup1 = !enabledGroup1;
        }

        public void SetSpeed(float speed, float layerSpeedDiff)
        {
            this.speed = speed;
            this.layerSpeedDiff = layerSpeedDiff;
        }

        public void UpdateBackground()
        {
            group1.UpdateGroup(speed, layerSpeedDiff, group2.end, enabledGroup1);
            group2.UpdateGroup(speed, layerSpeedDiff, group1.end, !enabledGroup1);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.PoolSystem
{
    [System.Serializable]
    public class PoolObjects
    {
        public GameObject[] objects;
        public Queue<GameObject> pool;
    }

    public class PoolObjectsManager : MonoBehaviour
    {
        #region Singleton
        private static PoolObjectsManager instance = null;
        public static PoolObjectsManager Instance => instance;
        
        #endregion

        [Header("Pools")]
        [SerializeField] private PoolObjects platforms;
        [SerializeField] private PoolObjects platformObjects;
        [SerializeField] private PoolObjects bullets;
        [SerializeField] private PoolObjects arrows;

        public PoolObjects Platforms => platforms; 
        public PoolObjects PlatformObjects => platformObjects;
        public PoolObjects Bullets => bullets;
        public PoolObjects Arrows => arrows;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            void InitializePool(ref PoolObjects poolObjects)
            {
                poolObjects.pool = new Queue<GameObject>();

                for (int i = 0; i < poolObjects.objects.Length; i++)
                {
                    poolObjects.pool.Enqueue(poolObjects.objects[i]);
                }
            }

            InitializePool(ref platforms);
            InitializePool(ref platformObjects);
            InitializePool(ref bullets);
            InitializePool(ref arrows);
        }

        public void DeactivateObject(GameObject gObject)
        {
            gObject.SetActive(false);

            void DeactivateIfPartOfGroup(ref PoolObjects poolObjects)
            {
                for (int i = 0; i < poolObjects.objects.Length; i++)
                {
                    if (poolObjects.objects[i] == gObject)
                    {
                        poolObjects.pool.Enqueue(gObject);
                        return;
                    }
                }
            }

            DeactivateIfPartOfGroup(ref platforms);
            DeactivateIfPartOfGroup(ref platformObjects);
            DeactivateIfPartOfGroup(ref bullets);
            DeactivateIfPartOfGroup(ref arrows);
        }

        public GameObject ActivatePlatform()
        {
            return ActivateObject(ref platforms.pool);
        }

        public GameObject ActivatePlatformObject()
        {
            return ActivateObject(ref platformObjects.pool);
        }

        public GameObject ActivateBullet(bool playerBullet)
        {
            if (playerBullet)
            {
                return ActivateObject(ref arrows.pool);
            }
            else
            {
                return ActivateObject(ref bullets.pool);
            }
        }

        private GameObject ActivateObject(ref Queue<GameObject> queue)
        {
            GameObject gObject = queue.Dequeue();
            int index = 0;
            bool noObjectToReturn = false;

            while (gObject.activeSelf && !noObjectToReturn)
            {
                queue.Enqueue(gObject);
                gObject = queue.Dequeue();

                index++;

                if (index > queue.Count)
                {
                    noObjectToReturn = true;
                    queue.Enqueue(gObject);
                }
            }

            if (!noObjectToReturn)
            {
                gObject.SetActive(true);
                return gObject;
            }
            else
            {
                Debug.LogError("No objects available to activate!");
                return null;
            }
        }
    }
}


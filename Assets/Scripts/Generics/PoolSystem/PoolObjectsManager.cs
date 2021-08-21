using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Weapon;

namespace Games.Generics.PoolSystem
{
    public class PoolObjectsManager : MonoBehaviour
    {
        [Header("Pool system")]
        [SerializeField] protected GameObject[] objects = null;
        [SerializeField] protected Queue<GameObject> objectsPool = null;

        public GameObject[] Objects { get => objects; }

        protected void DeactivateObject(GameObject gObject)
        {
            if(gObject.TryGetComponent(out Bullet bullet))
            {
                float asdaf = 2;
            }

            gObject.SetActive(false);
            objectsPool.Enqueue(gObject);
        }

        protected GameObject ActivateObject()
        {
            GameObject gObject = objectsPool.Dequeue();

            while (gObject.activeSelf)
            {
                objectsPool.Enqueue(gObject);
                gObject = objectsPool.Dequeue();
            }

            gObject.SetActive(true);

            return gObject;
        }
    }
}


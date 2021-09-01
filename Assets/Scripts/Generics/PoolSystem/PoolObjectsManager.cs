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

        public void DeactivateObject(GameObject gObject)
        {
            gObject.SetActive(false);
            objectsPool.Enqueue(gObject);
        }

        public GameObject ActivateObject()
        {
            GameObject gObject = objectsPool.Dequeue();
            int index = 0;
            bool noObjectToReturn = false;

            while (gObject.activeSelf && !noObjectToReturn)
            {
                objectsPool.Enqueue(gObject);
                gObject = objectsPool.Dequeue();

                index++;

                if (index > objectsPool.Count)
                {
                    noObjectToReturn = true;
                    objectsPool.Enqueue(gObject);
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


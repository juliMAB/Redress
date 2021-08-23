using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.PoolSystem;

namespace Games.Generics.Weapon
{
    public class Gun : PoolObjectsManager
    {
        [SerializeField] private Transform firePosition = null;
        [SerializeField] private float coolDownTime = 0.1f;
        [SerializeField] private bool canShoot = true;
        
        private IEnumerator setCoolDownLifetimeInstance = null;

        public float bulletSpeed = 10f;

        private void Awake()
        {
            objectsPool = new Queue<GameObject>();
            Bullet bullet = null;
            
            for (int i = 0; i < objects.Length; i++)
            {
                objectsPool.Enqueue(objects[i]);
                bullet = objects[i].GetComponent<Bullet>();

                bullet.OnCollided += DeactivateObject;
            }
        }

        private IEnumerator SetCoolDownLifetime()
        {
            canShoot = false;

            yield return new WaitForSeconds(coolDownTime);

            canShoot = true;

            yield return null;
        }

        public void Shoot()
        {
            if (!canShoot)
            {
                return;
            }

            GameObject GO = ActivateObject();
            
            Bullet bullet = GO.GetComponent<Bullet>();
            bullet.speed = bulletSpeed;
            bullet.transform.position = firePosition.position + firePosition.right * bullet.transform.lossyScale.x / 2;
            bullet.transform.rotation = transform.rotation;

            setCoolDownLifetimeInstance = SetCoolDownLifetime();
            StartCoroutine(setCoolDownLifetimeInstance);
        }

        public void ResetStats()
        {
            if (setCoolDownLifetimeInstance != null)
            {
                StopCoroutine(setCoolDownLifetimeInstance);
            }

            canShoot = true;
        }
    }
}

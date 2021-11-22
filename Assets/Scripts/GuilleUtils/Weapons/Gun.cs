using System.Collections;
using UnityEngine;

using GuilleUtils.PoolSystem;
using Redress.Gameplay.Management;

namespace GuilleUtils.Weapon
{
    public class Gun : MonoBehaviour
    {
        private IEnumerator setCoolDownLifetimeInstance = null;

        [SerializeField] AK.Wwise.Event shoot;
        [SerializeField] private Transform firePosition = null;
        [SerializeField] private float coolDownTime = 0.1f;
        [SerializeField] private float[] anglesOfShoot = null;
        [SerializeField] private bool canShoot = true;

        public float bulletSpeed = 10f;
        public float coolDownMultiplier = 1;

        private void Start()
        {
            Bullet bullet = null;

            PoolObjectsManager poolManager = PoolObjectsManager.Instance;

            for (int i = 0; i < poolManager.Bullets.objects.Length; i++)
            {
                bullet = poolManager.Bullets.objects[i].GetComponent<Bullet>();

                bullet.OnCollided += poolManager.DeactivateObject;
            }

            for (int i = 0; i < poolManager.Arrows.objects.Length; i++)
            {
                bullet = poolManager.Arrows.objects[i].GetComponent<Bullet>();

                bullet.OnCollided += poolManager.DeactivateObject;
            }
        }

        private IEnumerator SetCoolDownLifetime()
        {
            canShoot = false;

            if (tag == "PlayerWeapon")
            {
                yield return new WaitForSeconds(coolDownTime);
            }
            else
            {
                yield return new WaitForSeconds(coolDownTime * coolDownMultiplier);
            }

            canShoot = true;

            yield return null;
        }

        public void Shoot()
        {
            if (!canShoot)
            {
                return;
            }
            shoot.Post(gameObject);


            for (int i = 0; i < anglesOfShoot.Length; i++)
            {
                GameObject GO = PoolObjectsManager.Instance.ActivateBullet(gameObject.tag == "PlayerWeapon");
                Bullet bullet = GO.GetComponent<Bullet>();

                bullet.speed = bulletSpeed;
                bullet.transform.position = firePosition.position + firePosition.right * bullet.transform.lossyScale.x / 2;
                bullet.transform.rotation = transform.rotation * Quaternion.Euler(transform.forward * anglesOfShoot[i]);
            }

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

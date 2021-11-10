using System.Collections;
using UnityEngine;

using Games.Generics.PoolSystem;

namespace Games.Generics.Weapon
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] enum Type {Laser, Arrow };
        [SerializeField] Type type;
        private IEnumerator setCoolDownLifetimeInstance = null;

        [SerializeField] private Transform firePosition = null;
        [SerializeField] private float coolDownTime = 0.1f;
        [SerializeField] private float[] anglesOfShoot = null;
        [SerializeField] private bool canShoot = true;

        public float bulletSpeed = 10f;

        private void Start()
        {
            Bullet bullet = null;

            PoolObjectsManager poolManager = PoolObjectsManager.Instance;

            for (int i = 0; i < poolManager.Bullets.objects.Length; i++)
            {
                bullet = poolManager.Bullets.objects[i].GetComponent<Bullet>();

                bullet.OnCollided += poolManager.DeactivateObject;
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
            switch (type)
            {
                case Type.Laser:
                    AkSoundEngine.PostEvent(SoundsManager.Get().Laser, gameObject);
                    break;
                case Type.Arrow:
                    AkSoundEngine.PostEvent(SoundsManager.Get().Disparo, gameObject);
                    break;
                default:
                    break;
            }
            
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

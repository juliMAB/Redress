using UnityEngine;
using System;

using GuilleUtils.Weapon;
using GuilleUtils.PoolSystem;

namespace Redress.Gameplay.Controllers
{
    public class GunsController : MonoBehaviour
    {
        private PoolObjectsManager poolManager = null;

        [SerializeField] private Gun playerGun = null;

        public float speedMultiplier = 1f;

        public void Initialize(float bulletSpeed)
        {
            poolManager = PoolObjectsManager.Instance;
            SetBulletsSpeed(bulletSpeed, true);
        }

        public void SetBulletsSpeed(float speed, bool playerBulletsToo)
        {
            Gun[] allGuns = FindObjectsOfType<Gun>();
            for (int i = 0; i < allGuns.Length; i++)
            {
                if (playerBulletsToo || allGuns[i] != playerGun)
                {
                    allGuns[i].bulletSpeed = speed;
                }
            }

            for (int i = 0; i < poolManager.Bullets.objects.Length; i++)
            {
                poolManager.Bullets.objects[i].GetComponent<Bullet>().speed = speed;
            }

            if (playerBulletsToo)
            {
                for (int i = 0; i < poolManager.Arrows.objects.Length; i++)
                {
                    poolManager.Arrows.objects[i].GetComponent<Bullet>().speed = speed;
                }
            }
        }

        public void AssingCooldownToGuns()
        {
            Gun[] guns = FindObjectsOfType<Gun>();

            for (int i = 0; i < guns.Length; i++)
            {
                guns[i].coolDownMultiplier = 1 / speedMultiplier;
            }
        }
    }
}
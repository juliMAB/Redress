using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Games.Generics.Weapon;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class ThreeWayGun : PickUp
    {
        private GameObject playerGun = null;
        private Gun threeWayGun = null;
        private Vector3 position = Vector3.zero;

        private void Awake()
        {
            totalDurability = 5f;

            threeWayGun = GetComponent<Gun>();
        }

        protected override void Update()
        {
            if (leftDurability < 0)
            {
                player.ResetGun();
            }
            else if (picked)
            {
                FollowPlayer();
            }

            base.Update();
        }

        protected override void OnPicked()
        {
            base.OnPicked();
                        
            if (player.Gun != player.InitialGun)
            {
                UnactiveOldGun();
            }

            playerGun = player.InitialGun.gameObject;
            playerGun.GetComponent<Gun>().enabled = false;
            playerGun.GetComponentInChildren<SpriteRenderer>().enabled = false;
            player.Gun = threeWayGun;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            threeWayGun.ResetStats();
        }

        private void UnactiveOldGun()
        {
            OnConsumed?.Invoke(player.Gun.gameObject);
        }

        private void FollowPlayer()
        {
            position = playerGun.transform.position;
            transform.position = position;
            transform.rotation = playerGun.transform.rotation;
        }        
    }
}

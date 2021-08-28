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

        private void Start()
        {
            totalDurability = 5f;
            leftDurability = totalDurability;

            threeWayGun = GetComponent<Gun>();
        }

        protected override void Update()
        {
            if (leftDurability < 0)
            {
                player.Gun = playerGun.GetComponent<Gun>();
                player.Gun.enabled = true;
                player.Gun.GetComponentInChildren<SpriteRenderer>().enabled = true;
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
                        
            playerGun = player.Gun.gameObject;
            playerGun.GetComponent<Gun>().enabled = false;
            playerGun.GetComponentInChildren<SpriteRenderer>().enabled = false;
            player.Gun = threeWayGun;
        }

        private void FollowPlayer()
        {
            position = playerGun.transform.position;
            transform.position = position;
            transform.rotation = playerGun.transform.rotation;
        }
    }
}

using UnityEngine;
using GuilleUtils.Weapon;
using GuilleUtils.Animations;

namespace Redress.Gameplay.Objects.PickUps
{
    public class ThreeWayGun : PickUp
    {
        private GameObject playerGun = null;
        private Gun threeWayGun = null;
        private Transform originalParet;
        private Swing swing;
        private static bool actived;
        private static bool cancel;

        protected override void Awake()
        {
            base.Awake();
            threeWayGun = GetComponent<Gun>();
            originalParet = transform.parent;
            swing = GetComponent<Swing>();
        }

        protected override void OnPickedUp()
        {            
            base.OnPickedUp();
            swing.enabled = false;
            playerGun = player.InitialGun.gameObject;
            playerGun.GetComponent<Gun>().enabled = false;
            playerGun.GetComponentInChildren<SpriteRenderer>().enabled = false;
            player.Gun = threeWayGun;
            FollowPlayer();
            if (actived)
                cancel = true;
            actived = true;
        }
        protected override void OnEndPickUp()
        {
            if (cancel)
            {
                cancel = false;
                return;
            }
            actived = false;
            base.OnEndPickUp();
            player.ResetGun();
            transform.parent = originalParet;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            threeWayGun.ResetStats();
            swing.enabled = true;
        }

        private void FollowPlayer()
        {
            transform.position = playerGun.transform.position;
            transform.rotation = playerGun.transform.rotation;
            transform.parent = playerGun.transform;
        }        
    }
}

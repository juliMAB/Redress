using UnityEngine;

namespace Redress.Gameplay.Objects.PickUps
{
    public class Shield : PickUp
    {
        protected override void Awake()
        {
            base.Awake();

            totalDurability = 5f;
            leftDurability = 0f;
        }

        protected override void OnPickedUp()
        {
            player.SetInmuneForTime(totalDurability, Color.blue);
            leftDurability = 0f;
        }
    }
}

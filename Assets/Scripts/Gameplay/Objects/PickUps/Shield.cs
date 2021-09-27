using UnityEngine;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class Shield : PickUp
    {
        private void Start()
        {
            totalDurability = 5f;
            leftDurability = 0f;
        }

        protected override void OnPicked()
        {
            base.OnPicked();

            player.SetInmuneForTime(totalDurability, Color.blue);
            leftDurability = 0f;
        }
    }
}

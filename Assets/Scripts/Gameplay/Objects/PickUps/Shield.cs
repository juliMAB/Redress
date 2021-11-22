using UnityEngine;

namespace Redress.Gameplay.Objects.PickUps
{
    public class Shield : PickUp
    {
        protected override void OnPickedUp()
        {
            base.OnPickedUp();
            player.SetInmuneForTime(totalDurability, Color.blue);
            visual.SetActive(false);
        }
    }
}

using UnityEngine;

namespace Redress.Gameplay.Objects.PickUps
{
    public class Shield : PickUp
    {
        protected override void OnPickedUp()
        {
            player.SetInmuneForTime(totalDurability, Color.blue);
            visual.SetActive(false);
        }
    }
}

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class Life : PickUp
    {
        private void Start()
        {
            totalDurability = 0f;
            leftDurability = 0f;
        }

        protected override void OnPickedUp()
        {
            player.AddLife();
        }
    }
}

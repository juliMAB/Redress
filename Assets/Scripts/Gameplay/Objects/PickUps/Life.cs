namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class Life : PickUp
    {
        protected override void Start()
        {
            base.Start();

            totalDurability = 0f;
            leftDurability = 0f;
        }

        protected override void OnPickedUp()
        {
            player.AddLife();
        }
    }
}

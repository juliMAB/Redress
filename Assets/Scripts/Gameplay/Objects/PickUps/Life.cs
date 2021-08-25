using EndlessT4cos.Gameplay.User;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class Life : PickUp
    {
        private Player player;

        public Player Player { set => player = value; }

        private void Start()
        {
            totalDurability = 0f;
            leftDurability = 0f;
        }

        protected override void OnPicked()
        {
            base.OnPicked();

            player.AddLife();
        }
    }
}

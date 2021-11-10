namespace Redress.Gameplay.User
{
    public class PlayerData
    {
        public PlayerData(int _score, float _velocity, float _traveledDitance)
        {
            score = _score;
            velocity = _velocity;
            traveledDistance = _traveledDitance;
        }

        private int score = 0;
        private float velocity = 0;
        private float traveledDistance = 0f;

        public int Score => score;
        public float Velocity => velocity; 
        public float TraveledDitance => traveledDistance; 

        public void UpdateData(int _score, float _velocity, float _traveledDitance)
        {
            score = _score;
            velocity = _velocity;
            traveledDistance = _traveledDitance;
        }
    }
}

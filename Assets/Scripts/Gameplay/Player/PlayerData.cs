using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.User
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

        public int Score { get => score; }
        public float Velocity { get => velocity; }
        public float TraveledDitance { get => traveledDistance; }

        public void UpdateData(int _score, float _velocity, float _traveledDitance)
        {
            score = _score;
            velocity = _velocity;
            traveledDistance = _traveledDitance;
        }
    }
}

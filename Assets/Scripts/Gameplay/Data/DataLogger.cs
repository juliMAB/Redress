using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redress.Gameplay.User;

namespace Redress.Gameplay.Data
{
    public class DataLogger : MonoBehaviourSingleton<DataLogger>
    {
        PlayerData playerData = new PlayerData();
        public string username;

        public PlayerData LoadData()
        {
            return playerData;
        }
        public void SaveData(PlayerData data)
        {
            playerData = data; //la info del player.
        }
        public void ResetData()
        {
            playerData.ResetData();
        }
    }
}
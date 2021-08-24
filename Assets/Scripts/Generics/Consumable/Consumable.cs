using System;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace Games.Generics.Consumable
{
    public class Consumable : MovableObject
    {
        public Action OnTaken = null;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnTaken?.Invoke();
        }
    }
}

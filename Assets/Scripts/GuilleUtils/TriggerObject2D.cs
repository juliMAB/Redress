using System;
using UnityEngine;

namespace GuilleUtils.TriggerObject
{
    public class TriggerObject2D : MonoBehaviour
    {
        public Action OnActivatedTrigger;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnActivatedTrigger?.Invoke();
        }
    }
}


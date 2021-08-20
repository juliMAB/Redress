using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.TriggerObject
{
    public class TriggerObject2D : MonoBehaviour
    {
        public delegate void onActivatedTriggerDelegate();
        public onActivatedTriggerDelegate onActivatedTrigger;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onActivatedTrigger?.Invoke();
        }
    }
}


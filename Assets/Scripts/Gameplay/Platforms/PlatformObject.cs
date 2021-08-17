using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformObject : MovableObject
    {
        [SerializeField] public Row row = Row.Middle;
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class Platform : MovableObject
    {
        [SerializeField] public Row row = Row.Middle;
    }
}

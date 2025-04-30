using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules
{
    [Serializable] 
    public struct MoveStruct
    {
        public MovementType movementType;
        public int distanceMove;
    }

    [Serializable]
    public struct PathMove
    {
        public List<MoveStruct> moves;
    }
}

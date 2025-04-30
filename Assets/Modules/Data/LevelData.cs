using System.Collections.Generic;
using UnityEngine;

namespace Modules.Data
{
    [CreateAssetMenu(menuName = "DataSO/LevelData")]
    public class LevelData : ScriptableObject
    {
        public GameObject levelPrefab;
        public List<PathMove> pathMoves = new List<PathMove>();
    }
}

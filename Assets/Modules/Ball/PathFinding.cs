using System.Collections.Generic;
using Modules.Map;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Modules.Ball
{
    public class PathFinding : MonoBehaviour
    {
        [FormerlySerializedAs("noahGrid")] public MapController mapController;

        public List<Vector3> GetPath(Vector3 position,Vector2Int direction, PathMove pathMove)
        {
            var cellBall = mapController.GetCellByWorldPosition(position);
            int x = 0, y = 0;
            List<Vector3> path = new List<Vector3>();
            ;
            mapController.GetXY(out x, out y, position);
            foreach (var move in pathMove.moves)
            {
                if (move.movementType == MovementType.Run)
                    path.AddRange(GetPathRun(position, direction, move));
                else
                    path.AddRange(GetPathJump(position, direction, move));
            }
     
            
            return null;
        }

        private List<Vector3> GetPathRun(Vector3 position, Vector2Int direction, MoveStruct move)
        {
            return null;
        }

        private List<Vector3> GetPathJump(Vector3 position, Vector2Int direction, MoveStruct move)
        {
            return null;
        }
    }
}

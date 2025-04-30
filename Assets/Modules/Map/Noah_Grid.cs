using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.Map
{
    public class MapController : MonoBehaviour
    {
        public Transform posSpawnPlayer;
        [FormerlySerializedAs("height")] [SerializeField] private int col;
        [FormerlySerializedAs("width")] [SerializeField] private int row;
        [FormerlySerializedAs("columns")] [SerializeField] private List<Cell> cells = new List<Cell>();
        [SerializeField] private Cell[,] _matrix;

        private void Awake()
        {
            GenerateMatrix();
            SetupMatrix();
        }
        
        [ContextMenu("Generate Matrix")]
        private void GenerateMatrix()
        {
            int indexCell = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    cells[indexCell].gameObject.name = $"row_{i}_column_{j}";
                    cells[indexCell].SetupXY(i, j);
                    Vector2 posTarget = GetPositionCell(i, j);
                    cells[indexCell].transform.localPosition = new Vector3(posTarget.x, 0, posTarget.y);
                    indexCell += 1;
                }
            }
        }

        private Vector2 GetPositionCell(int x,int y)
        {
            Vector2 origin = new Vector2(-(row - 1) / 2f, -(col - 1) / 2f);
            return new Vector2(origin.x + x, origin.y + y);
        }

        public void ShowMap()
        {
            
        }

        public MapController(Cell[,] matrix)
        {
            _matrix = matrix;
        }

        private void SetupMatrix()
        {
            _matrix = new Cell[row, col];
            int indexCell = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    _matrix[i, j] = cells[indexCell];
                    indexCell += 1;
                }
            }
        }

        public Vector3 GetWorldPosition(Vector3 worldPosition)
        {
            return GetCellByWorldPosition(worldPosition).transform.position;
        }

        public Cell GetCellByWorldPosition(Vector3 worldPosition)
        {
            Vector3 origin = new Vector3(-(row - 1) / 2f, 0, -(col - 1) / 2f);
            int x = Mathf.RoundToInt(worldPosition.x - origin.x);
            int y = Mathf.RoundToInt(worldPosition.z - origin.z);
            return _matrix[x, y];
        }

        public Cell GetCellByXY(int x, int y)
        {
            return _matrix[x, y];
        }

        public void GetXY(out int x, out int y, Vector3 worldPosition)
        {
            Vector3 origin = new Vector3(-(row - 1) / 2f, 0, -(col - 1) / 2f);
            x = Mathf.RoundToInt(worldPosition.x - origin.x);
            y = Mathf.RoundToInt(worldPosition.z - origin.z);
        }
    }
}

using System;
using UnityEngine;

namespace Modules.Map
{
    public class Cell : MonoBehaviour
    {
        public int x;
        public int y;
        public bool isGround;

        public void SetupXY(int xVal, int yVal)
        {
            this.x = xVal;
            this.y = yVal;
        }
    }
}

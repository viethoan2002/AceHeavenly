using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerFitter : MonoBehaviour
{
    void Start()
    {
        var canvas = GetComponent<CanvasScaler>();
        
        float targetRatio = 1280f / 720;
        float screenRatio = Screen.width / (float)Screen.height;
        
        canvas.matchWidthOrHeight = (screenRatio >= targetRatio) ? 1 : 0;
    }
}
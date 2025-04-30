using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
    public class LoadingProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        private void Update()
        {
            fillImage.fillAmount = Loader.Progress;
        }
    }
}

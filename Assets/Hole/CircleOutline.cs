using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CircleOutline : MonoBehaviour
    {
        public static CircleOutline Instance;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image image;
        private float _witdh, _height;
        [SerializeField] private RectTransform circleOutline;
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void ScaleIn(Action action = null)
        {
            circleOutline.anchoredPosition = Vector2.zero;
            circleOutline.localScale = new Vector3(15, 15, 15);
            image.raycastTarget = true;
            circleOutline.DOScale(Vector3.zero, 1f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                action?.Invoke();
                image.raycastTarget = false;
            });
        }


        public void ScaleOut(Action action = null)
        {
            circleOutline.anchoredPosition = Vector2.zero;
            circleOutline.localScale = Vector3.zero;
            image.raycastTarget = true;
            DOVirtual.DelayedCall(1f, () =>
            {
                circleOutline.DOScale(new Vector3(15, 15, 15), 1f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                {
                    action?.Invoke();
                    image.raycastTarget = false;
                });
            }); 
        }
    }
}

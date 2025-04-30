using System;
using System.Collections;
using Modules.Controller.Swipe;
using UnityEngine;

namespace Modules.Camera
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private SwipeListener swipeListener;
        [SerializeField] private Transform originTransform;
        public float targetAngle = 90f; // Góc cần xoay
        public float rotationSpeed = 2.0f; // Tốc độ xoay
        private Quaternion startRotation;
        private Quaternion targetRotation;
        private float t = 0;
        private bool isRotating = false;

        private void OnEnable()
        {
            swipeListener.OnSwipeRotate.AddListener(SwipeRotate);
            swipeListener.EndSwipeRotate.AddListener(EndRotate);
            swipeListener.StartSwipeRotate.AddListener(StartRotate);
        }

        private void OnDisable()
        {
            swipeListener.OnSwipeRotate.RemoveListener(SwipeRotate);
            swipeListener.EndSwipeRotate.RemoveListener(EndRotate);
            swipeListener.StartSwipeRotate.RemoveListener(StartRotate);
        }

        private void SwipeRotate(float amount)
        {
            if (Mathf.Abs(amount) > 0.1f)
            {
                t = 0;
                isRotating = true;
                startRotation = transform.rotation;
                targetRotation = transform.rotation * Quaternion.Euler(0, amount, 0);
            } 
        }
        
        private void StartRotate()
        {
            startRotation = originTransform.rotation;
            isRotating = true;
        }

        private void EndRotate()
        {
            isRotating = false;
            StartCoroutine(RotateToOriginal());
        }

        IEnumerator RotateToOriginal()
        {
            t = 0;
            bool en = true;
            startRotation = transform.rotation;
            targetRotation = Quaternion.Euler(0, -45, 0);
            while (en)
            {
                t += rotationSpeed * Time.deltaTime * 0.1f;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                
                if (t >= 1)
                {
                    en = false;
                }

                yield return null;
            }
        }
        
        private void FixedUpdate()
        {
            if (isRotating)
            {
                t += rotationSpeed;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                
                if (t >= 1)
                {
                    isRotating = false;
                }
            }
        }
    }
}

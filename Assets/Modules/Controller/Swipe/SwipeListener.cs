using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Modules.Controller.Swipe
{
    public class SwipeListener : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public UnityEngine.Events.UnityEvent OnSwipeCancelled;

        public UnityEvent<string> OnSwipe;

        public UnityEvent<float> OnSwipeRotate;
        
        public UnityEvent StartSwipeRotate;
        public UnityEvent EndSwipeRotate;

        [SerializeField]
        private float _sensetivity = 10;

        [SerializeField]
        private bool _continuousDetection;

        [SerializeField]
        private SwipeDetectionMode _swipeDetectionMode = SwipeDetectionMode.EightSides;

        private bool _waitForSwipe = true;

        private float _minMoveDistance = 0.1f;
        private float _floatRotateDistance = 0.1f;

        private Vector3 _swipeStartPoint;
        private Vector3 _swipeRotatePoint;

        private Vector3 _offset;
        private Vector3 _offsetRotate;

        private VectorToDirection _directions;

        public bool ContinuousDetection { get => _continuousDetection; set => _continuousDetection = value; }

        public float Sensetivity
        {
            get
            {
                return _sensetivity;
            }
            set
            {
                _sensetivity = value;
                UpdateSensetivity();
            }
        }

        public SwipeDetectionMode SwipeDetectionMode { get => _swipeDetectionMode; set => _swipeDetectionMode = value; }

        public void SetDetectionMode(List<DirectionId> directions)
        {
            _directions = new VectorToDirection(directions);
        }

        private void Start()
        {
            UpdateSensetivity();

           // if (SwipeDetectionMode != SwipeDetectionMode.Custom)
            {
                SetDetectionMode(DirectionPresets.GetPresetByMode(SwipeDetectionMode));
            }
        }

        private void UpdateSensetivity()
        {
            int screenShortSide = Screen.width < Screen.height ? Screen.width : Screen.height;
            _minMoveDistance = screenShortSide / _sensetivity;
            _floatRotateDistance = screenShortSide;

        }

        //private void Update()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        InitSwipe();
        //    }

        //    if (_waitForSwipe && Input.GetMouseButton(0))
        //    {
        //        CheckSwipe();
        //    }

        //    if (_continuousDetection == false)
        //    {
        //        CheckSwipeCancellation();
        //    }
        //}

        private void CheckSwipeCancellation()
        {
            if (_waitForSwipe)
            {
                OnSwipeCancelled?.Invoke();
            }
        }

        private void InitSwipe()
        {
            SampleSwipeStart();
            StartSwipeRotate?.Invoke();
            _swipeRotatePoint = Input.mousePosition;
            _waitForSwipe = true;
        }

        private void CheckSwipe()
        {
            _offset = Input.mousePosition - _swipeStartPoint;
            if ((Input.mousePosition - _swipeRotatePoint).magnitude > 0)
            {
                OnSwipeRotate?.Invoke(Input.mousePosition.x < _swipeRotatePoint.x
                    ? -(Input.mousePosition - _swipeRotatePoint).magnitude
                    : (Input.mousePosition - _swipeRotatePoint).magnitude);
                _swipeRotatePoint = Input.mousePosition;
            }
            
            if (_offset.magnitude >= _minMoveDistance)
            {
                OnSwipe?.Invoke(_directions.GetSwipeId(_offset));
                if (!_continuousDetection)
                {
                    _waitForSwipe = false;
                }
                SampleSwipeStart();
            }
        }

        private void SampleSwipeStart()
        {
            _swipeStartPoint = Input.mousePosition;
            _offset = Vector3.zero;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            InitSwipe();
        }

        public void OnDrag(PointerEventData eventData)
        {
            CheckSwipe();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            CheckSwipeCancellation();
            EndSwipeRotate?.Invoke();
        }
    }
}
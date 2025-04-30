using System;
using Modules.Cards;
using Modules.Command;
using Modules.Controller.Swipe;
using UnityEngine;

namespace Modules.Ball
{
    public class BallController : MonoBehaviour
    {
        [SerializeField] private SwipeListener swipeListener;
        [SerializeField] private BallMovement ballMovement;
        [SerializeField] private CardController card;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private MeshRenderer meshRenderer;

        private HoleController _holeController;

        private void OnEnable()
        {
            swipeListener.OnSwipe.AddListener(OnSwipe);
            CardManager.OnSelectCard += SetupCard;
        }

        private void OnDisable()
        {
            swipeListener.OnSwipe.RemoveListener(OnSwipe);
            CardManager.OnSelectCard -= SetupCard;
        }

        public void SetupBall(Vector3 position)
        {
            transform.position = position;
            meshRenderer.enabled = true;
            ballMovement.isMoving = false;
            trailRenderer.Clear();
        }

        private void SetupCard(CardController cardController)
        {
            card = cardController;
        }

        public void CheckWin()
        {
            if (_holeController != null)
            {
                meshRenderer.enabled = false;
                _holeController.PlayEffect();
                GameManager.Instance.WinGame();
            }
        }

        private void OnSwipe(string direction)
        {
            if (ballMovement.isMoving)
                return;
            
            MoveCommand command = new MoveCommand(ballMovement, card, transform.position);
            CommandInvoke.ExecuteCommand(command);
            
            switch (direction)
            {
                case "Up":
                    ballMovement.StartMoving(DirectionType.Forward);
                    break;
                case "Right":
                    ballMovement.StartMoving(DirectionType.Right);
                    break;
                case "Down":
                    ballMovement.StartMoving(DirectionType.Backward);
                    break;
                case "Left":
                    ballMovement.StartMoving(DirectionType.Left);
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hole"))
            {
                _holeController = other.GetComponent<HoleController>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Hole"))
            {
                _holeController = null;
            }
        }
    }
}

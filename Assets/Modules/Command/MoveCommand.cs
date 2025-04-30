using Modules.Ball;
using Modules.Cards;
using Modules.Map;
using UnityEngine;

namespace Modules.Command
{
    public class MoveCommand : ICommand
    {
        public BallMovement ball;
        
        public Vector3 lastPosition;
        public CardController card;

        public MoveCommand(BallMovement ball, CardController cardController, Vector3 lastPosition)
        {
            this.ball = ball;
            this.card = cardController;
            this.lastPosition = lastPosition;
        }
        
        public void Execute()
        {
            ball.ExecuteMove(card);
        }

        public void Undo()
        {
            ball.UndoMove(lastPosition,card);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Cards;
using Modules.Map;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.Ball
{
    public class BallMovement : MonoBehaviour
    {
        [SerializeField] private BallController ballController;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Vector3 velocity;
        [SerializeField] private Vector3 direction;
        [SerializeField] private Vector2 startPoint;
        
        [SerializeField] private PathMove currentPath;
        [SerializeField] private DirectionType currentDirection;
        
        [SerializeField] private float speed;
        [SerializeField] private float currentSpeed;
        [SerializeField] private int distance;
        
        [FormerlySerializedAs("_isMoving")] public bool isMoving;
        private bool _isClearMove;
        private RaycastHit _hit;
        [SerializeField] private Vector3 directionHit;
        [SerializeField] private LayerMask groundLayer;
        
        [SerializeField] private float speedJump;
        [SerializeField] private float speedFall;
        private float _x1, _y1, _x2, _y2, _x3, _y3;
        private Vector3 _vectorA;
        private Vector3 _vectorB;
        private Vector3 _vectorC;

        private void SetPathMove(PathMove pathMove)
        {
            currentPath = pathMove;
        }

        public void StartMoving(DirectionType directionType)
        {
            if (isMoving)
                return;
            
            isMoving = true;
            _isClearMove = false;
            StartCoroutine(MovingCoroutine(directionType));
        }

        IEnumerator MovingCoroutine(DirectionType directionType)
        {
            foreach (var p in currentPath.moves)
            {
                if(_isClearMove)
                    break;
                distance = p.distanceMove;
                if (p.movementType == MovementType.Run)
                {
                    SetupMovingRun(directionType);
                    yield return MovingRunCoroutine();
                }
                else
                {
                    SetupMovingJump(directionType);
                    yield return MovingJumpCoroutine();
                }
            }
        }

        private void SetupMovingJump(DirectionType type)
        {
            Vector3 targetPos = Vector3.zero;
            currentDirection = type;
            switch (currentDirection)
            {
                case DirectionType.Left:
                    targetPos = transform.position + distance * Vector3.left;
                    break;
                case DirectionType.Right:
                    targetPos = transform.position + distance * Vector3.right;
                    break;
                case DirectionType.Backward:
                    targetPos = transform.position + distance * Vector3.back;
                    break;
                case DirectionType.Forward:
                    targetPos = transform.position + distance * Vector3.forward;
                    break;
            }

            _vectorA = transform.position;
            if (Physics.Raycast(new Vector3(targetPos.x, 5, targetPos.z), Vector3.down, out _hit, 6, groundLayer))
            {
                _vectorC = new Vector3(_hit.point.x, _hit.point.y + 0.125f, _hit.point.z);
            }
            else
                _vectorC = new Vector3(targetPos.x, -1, targetPos.z);
            
            _vectorB = Vector3.Lerp(_vectorA, _vectorC, 0.5f);
            _vectorB = new Vector3(_vectorB.x, _vectorA.y > _vectorC.y ? _vectorA.y + 1 : _vectorC.y + 1, _vectorB.z);
        }

        IEnumerator MovingJumpCoroutine()
        {

            _x1 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorA.x : _vectorA.z; _y1 = _vectorA.y;
            _x2 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorB.x : _vectorB.z; _y2 = _vectorB.y;
            _x3 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorC.x : _vectorC.z; _y3 = _vectorC.y;
            
            float amount = 0;
            
            float[] coefficients = FindParabolaEquation(new Vector2(_x1,_y1),
                                                        new Vector2(_x2,_y2),
                                                        new Vector2(_x3,_y3));
            
            while (amount < 1)
            {
                amount = amount + Time.deltaTime * speedJump > 1 ? 1 : amount + Time.deltaTime * speedJump;
                float x = Mathf.Lerp(_vectorA.x, _vectorC.x, amount);
                float z = Mathf.Lerp(_vectorA.z, _vectorC.z, amount);
                float alpha = Mathf.Lerp(_x1, _x3, amount);
                float y = coefficients[0] * alpha * alpha + coefficients[1] * alpha + coefficients[2];
                transform.position = new Vector3(x, y, z);
                yield return null;
            }

            transform.position = _vectorC;
            
            CheckBallOnInclinedPlane();
        }

        private void SetupMovingRun(DirectionType type)
        {
            currentDirection = type;
            switch (currentDirection)
            {
                case DirectionType.Left:
                    direction = -Vector3.right;
                    directionHit = -Vector3.forward;
                    break;
                case DirectionType.Right:
                    direction = Vector3.right;
                    directionHit = Vector3.forward;
                    break;
                case DirectionType.Backward:
                    direction = -Vector3.forward;
                    directionHit = Vector3.right;
                    break;
                case DirectionType.Forward:
                    direction = Vector3.forward;
                    directionHit = -Vector3.right;
                    break;
            }
            
            rb.constraints = direction.x != 0
                ? RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation
                : RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }

        IEnumerator MovingRunCoroutine()
        {
            startPoint = new Vector2(transform.position.x, transform.position.z);
            currentSpeed = speed;
            
            while (Vector2.Distance(startPoint, new Vector2(transform.position.x, transform.position.z)) < distance)
            {
                CheckWall();
                SetVector();
                if (!Physics.Raycast(transform.position, Vector3.down, out _hit, 0.25f, groundLayer))
                    yield return MovingFallingCoroutine();
                else
                    yield return null;
            }
            
            CheckBallOnInclinedPlane();
        }

        IEnumerator MovingFallingCoroutine()
        {
            _vectorB = transform.position;
            Vector3 targetPos = GameManager.Instance.mapController.GetWorldPosition(transform.position);
            if (Physics.Raycast(new Vector3(targetPos.x, 5, targetPos.z), Vector3.down, out _hit, 6, groundLayer))
            {
                _vectorC = new Vector3(_hit.point.x, _hit.point.y + 0.125f, _hit.point.z);
            }
            else
                _vectorC = new Vector3(targetPos.x, -1, targetPos.z);

            _vectorA = new Vector3(_vectorC.x + 2 * (_vectorB.x - _vectorC.x), _vectorC.y,
                _vectorC.z + 2 * (_vectorB.z - _vectorC.z));

            _x1 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorA.x : _vectorA.z; _y1 = _vectorA.y;
            _x2 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorB.x : _vectorB.z; _y2 = _vectorB.y;
            _x3 = Mathf.Abs(_vectorA.x - _vectorC.x) > 0.01f ? _vectorC.x : _vectorC.z; _y3 = _vectorC.y;
            
            float[] coefficients = FindParabolaEquation(new Vector2(_x1,_y1),
                new Vector2(_x2,_y2),
                new Vector2(_x3,_y3));

            if (float.IsNaN(coefficients[1]))
                yield break;

            float amount = 0.5f;

            while (amount < 1)
            {
                amount = amount + Time.deltaTime * speedFall > 1 ? 1 : amount + Time.deltaTime * speedFall;
                float x = Mathf.Lerp(_vectorA.x, _vectorC.x, amount);
                float z = Mathf.Lerp(_vectorA.z, _vectorC.z, amount);
                float alpha = Mathf.Lerp(_x1, _x3, amount);
                float y = coefficients[0] * alpha * alpha + coefficients[1] * alpha + coefficients[2];
                transform.position = new Vector3(x, y, z);
                yield return null;
            }

            transform.position = _vectorC;
            if (_vectorC.y < 0)
                StopMoving();
        }

        private void SetVector()
        {
            if (direction.magnitude <= 0)
                return;
            
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 2,groundLayer) )
            {
                Vector3 perpendicularX = Vector3.Cross(_hit.normal, directionHit).normalized;
                velocity = perpendicularX * currentSpeed;
            }
            else
            {
                velocity = Vector3.down * speed;
            }
            
            rb.velocity = velocity;
        }

        private void CheckWall()
        {
            if (direction.magnitude <= 0)
                return;

            if (Physics.Raycast(transform.position, direction, out _hit, 0.175f, groundLayer))
            {
                if (Vector3.Distance(_hit.normal, -direction) < 0.01f && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_hit.point.x, _hit.point.z)) <= 0.13f)
                {
                    var targetPos = GameManager.Instance.mapController.GetWorldPosition(transform.position);
                    distance -= ((int)Vector2.Distance(startPoint, new Vector2(targetPos.x, targetPos.z)) + 1);

                    startPoint = new Vector2(targetPos.x, targetPos.z);
                    Debug.Log(startPoint);

                    switch (currentDirection)
                    {
                        case DirectionType.Left:
                            SetupMovingRun(DirectionType.Right);
                            break;
                        case DirectionType.Right:
                            SetupMovingRun(DirectionType.Left);
                            break;
                        case DirectionType.Backward:
                            SetupMovingRun(DirectionType.Forward);
                            break;
                        case DirectionType.Forward:
                            SetupMovingRun(DirectionType.Backward);
                            break;
                    }
                }
                else if (Vector3.Distance(_hit.normal, -direction) > 0.01f)
                {
                    var targetPos = GameManager.Instance.mapController.GetWorldPosition(transform.position);
                    transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                    
                    distance-= ((int)Vector2.Distance(startPoint, new Vector2(targetPos.x, targetPos.z)));
                    startPoint = new Vector2(targetPos.x, targetPos.z);

                    switch (currentDirection)
                    {
                        case DirectionType.Left:
                            SetupMovingRun(_hit.normal.z > 0 ? DirectionType.Forward : DirectionType.Backward);
                            break;
                        case DirectionType.Right:
                            SetupMovingRun(_hit.normal.z > 0 ? DirectionType.Forward : DirectionType.Backward);
                            break;
                        case DirectionType.Backward:
                            SetupMovingRun(_hit.normal.x > 0 ? DirectionType.Right : DirectionType.Left);
                            break;
                        case DirectionType.Forward:
                            SetupMovingRun(_hit.normal.x > 0 ? DirectionType.Right : DirectionType.Left);
                            break;
                    }
                }
            }
        }

        private void CheckBallOnInclinedPlane()
        {
            var targetPos = GameManager.Instance.mapController.GetWorldPosition(transform.position);
            transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, 2,groundLayer) )
            {
                if (Vector3.Distance(_hit.normal, Vector3.up) > 0.01f)
                {
                    _isClearMove = true;
                    MoveBack(_hit.normal);
                }
                else
                {
                    StopMoving();
                }
            }
        }

        private void MoveBack(Vector3 hitNormal)
        {
            distance = 1;
            startPoint = new Vector2(transform.position.x, transform.position.z);

            if (Mathf.Abs(hitNormal.z) < 0.001f)
            {
                SetupMovingRun(hitNormal.x > 0 ? DirectionType.Right : DirectionType.Left);
            }
            else
            {
                SetupMovingRun(hitNormal.z > 0 ? DirectionType.Forward : DirectionType.Backward);
            }

            StartCoroutine(MovingRunCoroutine());
        }

        private void StopMoving()
        {
            velocity = Vector3.zero;
            direction = Vector3.zero;
            rb.velocity = velocity;
                
            isMoving = false;
            ballController.CheckWin();
        }
        
        float[] FindParabolaEquation(Vector2 A, Vector2 B, Vector2 C)
        {
            float x1 = A.x, y1 = A.y;
            float x2 = B.x, y2 = B.y;
            float x3 = C.x, y3 = C.y;
            
            float[,] matrix = {
                { x1 * x1, x1, 1, y1 },
                { x2 * x2, x2, 1, y2 },
                { x3 * x3, x3, 1, y3 }
            };

            return SolveLinearSystem(matrix);
        }

        float[] SolveLinearSystem(float[,] matrix)
        {
            int n = 3;
            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                    if (Mathf.Abs(matrix[k, i]) > Mathf.Abs(matrix[maxRow, i]))
                        maxRow = k;
                
                for (int k = i; k <= n; k++)
                {
                    (matrix[maxRow, k], matrix[i, k]) = (matrix[i, k], matrix[maxRow, k]);
                }
                
                for (int k = i + 1; k < n; k++)
                {
                    float factor = matrix[k, i] / matrix[i, i];
                    for (int j = i; j <= n; j++)
                        matrix[k, j] -= factor * matrix[i, j];
                }
            }
            
            float[] solution = new float[n];
            for (int i = n - 1; i >= 0; i--)
            {
                solution[i] = matrix[i, n] / matrix[i, i];
                for (int k = i - 1; k >= 0; k--)
                    matrix[k, n] -= matrix[k, i] * solution[i];
            }
            return solution;
        }

        public void ExecuteMove(CardController cardController)
        {
            CardManager.Instance.RemoveCard(cardController);
            SetPathMove(cardController.PathMove);
        }

        public void UndoMove(Vector3 position,CardController cardController)
        {
            StopMoving();
            CardManager.Instance.AddCard(cardController);
            transform.position = position;
        }

        private void OnDrawGizmos()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _hit,2,groundLayer))
            {
                Vector3 perpendicularX = Vector3.Cross(_hit.normal, transform.forward).normalized;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_hit.point, _hit.point + _hit.normal);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + perpendicularX);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, _hit.point);
            }
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + direction * 0.13F);
        }
    }
}
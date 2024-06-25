using System;
using System.Collections;
using System.Collections.Generic;
using static Utilities.Helpers.CommonFields;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using Random = UnityEngine.Random;

namespace UI.CorsiBlockTypes
{
    public class MovingCorsiBlock : CorsiBlock
    {
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite moveSprite;
        [SerializeField] private BoxCollider2D blockCollider;
        [SerializeField] private Rigidbody2D rigidBody;
        [SerializeField] private RectTransform rectTransform;

        private Direction _lastDirection;
        private Vector2 _moveVector;
        private Coroutine _movement;
        private bool _canChangeDirection = true;
        private const float DirectionChangeCooldown = 0.35f;

        private void OnValidate()
        {
            blockCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
            rectTransform = GetComponent<RectTransform>();
        }
        
        public override void MakeBlockMove()
        {
            _canChangeDirection = true;
            _lastDirection = (Direction)Random.Range(0, 4);
            _moveVector = DirectionVectors[_lastDirection];
            _movement = StartCoroutine(MoveSelf());
        }
        
        public float moveSpeed = .1f;
        public override IEnumerator MoveSelf()
        {
            while (true)
            {
                var position = rigidBody.position;
                position += _moveVector * (moveSpeed * Time.deltaTime);
                rigidBody.MovePosition(position);
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_canChangeDirection)
            {
                if (other.gameObject.CompareTag("UpBorder") || other.gameObject.CompareTag("DownBorder"))
                {
                    _moveVector = new Vector2(_moveVector.x, -_moveVector.y);
                }
            
                if (other.gameObject.CompareTag("RightBorder") || other.gameObject.CompareTag("LeftBorder"))
                {
                    _moveVector = new Vector2(-_moveVector.x, _moveVector.y);
                }

                if (other.gameObject.CompareTag("Fish"))
                {
                    ChooseNewDirection();
                }
                
                StartCoroutine(CooldownDirectionChange());
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_canChangeDirection)
            {
                if (other.gameObject.CompareTag("UpBorder") || other.gameObject.CompareTag("DownBorder"))
                {
                    _moveVector = new Vector2(_moveVector.x, -_moveVector.y);
                }
            
                if (other.gameObject.CompareTag("RightBorder") || other.gameObject.CompareTag("LeftBorder"))
                {
                    _moveVector = new Vector2(-_moveVector.x, _moveVector.y);
                }

                if (other.gameObject.CompareTag("Fish"))
                {
                    ChooseNewDirection();
                }
                
                StartCoroutine(CooldownDirectionChange());
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("UpBorder") || other.gameObject.CompareTag("DownBorder"))
            {
                _moveVector = new Vector2(_moveVector.x, -_moveVector.y);
            }
            
            if (other.gameObject.CompareTag("RightBorder") || other.gameObject.CompareTag("LeftBorder"))
            {
                _moveVector = new Vector2(-_moveVector.x, _moveVector.y);
            }
            
            /*
                RectTransform otherRectTransform = other.gameObject.GetComponent<RectTransform>();
                Vector2 otherPosition = otherRectTransform.anchoredPosition;
                Vector2 thisPosition = rectTransform.anchoredPosition;

                if (otherPosition.x < thisPosition.x)
                {
                    UpdateLastDirection(Direction.Right);
                }
                else if (otherPosition.x >= thisPosition.x)
                {
                    UpdateLastDirection(Direction.Left);
                }
                else if (otherPosition.y <= thisPosition.y)
                {
                    UpdateLastDirection(Direction.Up);
                }
                else
                {
                    UpdateLastDirection(Direction.Down);
                }*/
        }

        private void ChooseNewDirection()
        {
            if(_movement != null)
                StopCoroutine(_movement);
            var direction = (Direction)Random.Range(0, 4);
            while (direction == _lastDirection)
            {
                direction = (Direction)Random.Range(0, 4);
            }

            UpdateLastDirection(direction);
            _movement = StartCoroutine(MoveSelf());
        }

        private void UpdateLastDirection(Direction direction)
        {
            _lastDirection = direction;
            _moveVector = DirectionVectors[_lastDirection];
        }

        private IEnumerator CooldownDirectionChange()
        {
            _canChangeDirection = false;
            yield return new WaitForSeconds(DirectionChangeCooldown);
            _canChangeDirection = true;
        }
    }
}

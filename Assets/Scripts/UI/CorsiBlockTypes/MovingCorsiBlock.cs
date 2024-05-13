using System;
using System.Collections;
using System.Collections.Generic;
using static Utilities.CommonFields;
using UnityEngine;
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

        private Direction _lastDirection;
        private Vector2 _moveVector;
        private Coroutine _movement;
        private bool _canChangeDirection = true;
        private float _directionChangeCooldown = 0.3f;

        private void OnValidate()
        {
            blockCollider = GetComponent<BoxCollider2D>();
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _lastDirection = (Direction)Random.Range(0, 4);
            _moveVector = DirectionVectors[_lastDirection];
            _movement = StartCoroutine(MoveBlock());
        }
        
        public float moveSpeed = .5f;
        
        private IEnumerator MoveBlock()
        {
            while (true)
            {
                var transform1 = transform;
                var position = transform1.position;
                position += (Vector3)_moveVector * (moveSpeed * Time.deltaTime);
                position = new Vector3(position.x, position.y, 0);
                transform1.position = position;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("this is a trigger " + other.name);
            ChooseNewDirection();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_canChangeDirection)
            {
                ChooseNewDirection();
                StartCoroutine(CooldownDirectionChange());
                Debug.Log("this is a trigger stay " + other.name);
            }
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
            
            _lastDirection = direction;
            _moveVector = DirectionVectors[_lastDirection];
            _movement = StartCoroutine(MoveBlock());
        }

        private IEnumerator CooldownDirectionChange()
        {
            _canChangeDirection = false;
            yield return new WaitForSeconds(_directionChangeCooldown);
            _canChangeDirection = true;
        }
    }
}

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
        [SerializeField] private RectTransform rectTransform;

        private Direction _lastDirection;
        private Vector2 _moveVector;
        private Coroutine _movement;
        private bool _canChangeDirection = true;
        private float _directionChangeCooldown = 0.7f;

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
        
        public float moveSpeed = 20;
        public override IEnumerator MoveSelf()
        {
            while (true)
            {
                var anchoredPosition = rectTransform.anchoredPosition;
                anchoredPosition += _moveVector * (moveSpeed * Time.deltaTime);
                rectTransform.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y);
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ChooseNewDirection();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_canChangeDirection)
            {
                ChooseNewDirection();
                StartCoroutine(CooldownDirectionChange());
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("on trigger enter: " + other.gameObject.name);
            rigidBody.velocity = -rigidBody.velocity;
            if (other.gameObject.GetComponent<RectTransform>().anchoredPosition.x < rectTransform.anchoredPosition.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
            _movement = StartCoroutine(MoveSelf());
        }

        private IEnumerator CooldownDirectionChange()
        {
            _canChangeDirection = false;
            yield return new WaitForSeconds(_directionChangeCooldown);
            _canChangeDirection = true;
        }
    }
}

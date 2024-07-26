using System;
using Interactables;
using Spans.Skeleton;
using Spawners;
using UI;
using UI.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class SwipeHandler : MonoBehaviour
    {
        [SerializeField] private float swipeThreshold;
        [SerializeField] private float moveSpeed;
        [SerializeField] private SketchUIHelper sketchUIHelper;

        private Vector2 _swipeStartPosition;
        private Sketch _spawnedObject;
        private Vector3 _touchStart;
        private Vector3 _lastTouchPosition;
        private Vector3 _finalPosition;
        private bool _isSwiping;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void FocusSpawnedObject(Sketch spawned)
        {
            _spawnedObject = spawned;
            _spawnedObject.SetUIHelperReference(sketchUIHelper);
        }

        private void Update()
        {
            if (IsInteractingWithUI()) return;

            if (!ButtonsEnabled() && Input.touchCount == 0 && _spawnedObject != null)
            {
                _spawnedObject.EnableButtons(_finalPosition);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStart = touch.position;
                    _lastTouchPosition = touch.position;
                    _isSwiping = true;
                    _spawnedObject.DisableButtonsWhileMoving();
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _isSwiping = false;
                }

                if (_isSwiping)
                {
                    Vector2 swipeDelta = touch.position - (Vector2)_lastTouchPosition;
                    Vector3 movementDirection = new Vector3(swipeDelta.x, 0f, swipeDelta.y).normalized;
                    Vector3 targetPosition = _spawnedObject.transform.position + movementDirection * (moveSpeed * Time.deltaTime);
                    _spawnedObject.transform.position = targetPosition;
                    _finalPosition = targetPosition;

                    // Update _lastTouchPosition to the current touch position for more responsive swiping
                    _lastTouchPosition = touch.position;
                }
            }
        }

        public Vector3 GetFinalPosition()
        {
            return _finalPosition;
        }

        public bool ButtonsEnabled()
        {
            return _spawnedObject != null && _spawnedObject.ButtonsEnabled();
        }

        private bool IsInteractingWithUI()
        {
            // Check if any UI element is currently selected or active
            return EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null;
        }

        private void AddListeners()
        {
            InteractableSpawner.OnPositionChoiceNeeded += FocusSpawnedObject;
        }

        private void RemoveListeners()
        {
            InteractableSpawner.OnPositionChoiceNeeded -= FocusSpawnedObject;
        }
    }
}

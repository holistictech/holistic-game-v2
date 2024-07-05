using System;
using Interactables;
using Spans.Skeleton;
using Spawners;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
        private Vector3 _finalPosition;
        private bool _isSwiping;
        private bool _uiActive;

        public static event Action<Vector3> OnLocationSelected;

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
            if (_uiActive) return;
            if (IsInteractingWithUI()) return;

            if (!ButtonsEnabled() && Input.touchCount == 0)
            {
                _spawnedObject.EnableButtons(_finalPosition);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStart = touch.position;
                    _isSwiping = true;
                    _spawnedObject.DisableButtonsWhileMoving();
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _isSwiping = false;
                }

                if (_isSwiping)
                {
                    Vector2 swipeDelta = touch.position - (Vector2)_touchStart;
                    Vector3 movementDirection = new Vector3(swipeDelta.x, 0f, swipeDelta.y).normalized;
                    Vector3 targetPosition = _spawnedObject.transform.position + movementDirection * (moveSpeed * Time.deltaTime);
                    _spawnedObject.transform.position = targetPosition;
                    _finalPosition = targetPosition;

                    // Update _touchStart to the current touch position for more responsive swiping
                    _touchStart = touch.position;
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

        private void ToggleSwipe(ToggleSwipeInput input)
        {
            Debug.Log($"Swipe input toggled: {input.Toggle}");
            _uiActive = input.Toggle;
        }

        private void AddListeners()
        {
            InteractableSpawner.OnPositionChoiceNeeded += FocusSpawnedObject;
            //_eventBus.Register<ToggleSwipeInput>(ToggleSwipe);
            EventBus.Instance.Register<ToggleSwipeInput>(ToggleSwipe);
        }

        private void RemoveListeners()
        {
            InteractableSpawner.OnPositionChoiceNeeded -= FocusSpawnedObject;
            EventBus.Instance.Unregister<ToggleSwipeInput>(ToggleSwipe);
            //_eventBus.Unregister<ToggleSwipeInput>(ToggleSwipe);
        }
    }
}
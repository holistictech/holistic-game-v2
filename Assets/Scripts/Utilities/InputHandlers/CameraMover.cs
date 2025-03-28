using System;
using Spans.Skeleton;
using Spawners;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities.InputHandlers
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private RectTransform[] uiElements;
        [SerializeField] private float moveSpeed;
        private float _minX = -90f, _maxX = 20f, _minZ = -170f, _maxZ = -65f;
        
        private Vector3 _touchStart;
        private bool _isSwiping;
        private bool _isPlacing;

        private void Awake()
        {
            EventBus.Instance.Register<ToggleSwipeInput>(ToggleSwipe);
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private bool _lastEventValue;
        private void Update()
        {
            if (_isPlacing) return;
            if (IsInteractingWithUI()) return;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStart = touch.position;
                    _isSwiping = true;

                    foreach (RectTransform uiElement in uiElements)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(uiElement, touch.position))
                        {
                            _isSwiping = false;
                            break;
                        }
                    }
                }
                
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _isSwiping = false;
                }

                if (_isSwiping)
                {
                    Vector2 direction = (Input.GetTouch(Input.touchCount -1).position - (Vector2)_touchStart).normalized;

                    Vector3 targetPosition = transform.position - new Vector3(direction.x, 0, direction.y) * (moveSpeed * Time.deltaTime);
                    targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
                    targetPosition.z = Mathf.Clamp(targetPosition.z, _minZ, _maxZ);
                    transform.position = targetPosition;
                    if (_lastEventValue)
                    {
                        Debug.Log("event trigger");
                        EventBus.Instance.Trigger(new ToggleUIEventButtons(false));
                        _lastEventValue = false;
                    }
                }
            }
            else
            {
                if (!_lastEventValue)
                {
                    EventBus.Instance.Trigger(new ToggleUIEventButtons(true));
                    _lastEventValue = true;
                }
            }
        }

        private void DisableCameraMovement(Sketch config)
        {
            _isPlacing = true;
        }
        
        private bool IsInteractingWithUI()
        {
            // Check if any UI element is currently selected or active
            return EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null;
        }

        private void EnableCameraMovement()
        {
            _isPlacing = false;
        }
        
        private void EnableCameraMovement(Quaternion rotation)
        {
            _isPlacing = false;
        }
        
        private void ToggleSwipe(ToggleSwipeInput input)
        {
            Debug.Log($"Swipe input toggled: {input.Toggle}");
            enabled = input.Toggle;
        }
        
        private void AddListeners()
        {
            Sketch.OnPlacementConfirmed += EnableCameraMovement;
            Sketch.OnPlacementCancelled += EnableCameraMovement;
            InteractableSpawner.OnPositionChoiceNeeded += DisableCameraMovement;
        }

        private void RemoveListeners()
        {
            Sketch.OnPlacementConfirmed -= EnableCameraMovement;
            Sketch.OnPlacementCancelled -= EnableCameraMovement;
            InteractableSpawner.OnPositionChoiceNeeded -= DisableCameraMovement;
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unregister<ToggleSwipeInput>(ToggleSwipe);
        }
    }
}

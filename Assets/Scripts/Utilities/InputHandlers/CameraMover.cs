using System;
using Spans.Skeleton;
using Spawners;
using UI;
using UnityEngine;

namespace Utilities.InputHandlers
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private RectTransform[] uiElements;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float boundaryPaddingX;
        [SerializeField] private float boundaryPaddingZ;
        private float _minX = -25f, _maxX = 90f, _minZ = -15f, _maxZ = 40f;
        
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

        private void Start()
        {
            // The boundaries are now hardcoded, so this can be removed
            // var position = transform.position;
            // _minX = position.x - boundaryPaddingX;
            // _maxX = position.x + boundaryPaddingX;
            // _minZ = position.z - boundaryPaddingZ;
            // _maxZ = position.z + boundaryPaddingZ;
        }

        private void Update()
        {
            if (_isPlacing) return;
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
                    Vector2 direction = (Input.mousePosition - _touchStart).normalized;

                    Vector3 targetPosition = transform.position - new Vector3(direction.x, 0, direction.y) * (moveSpeed * Time.deltaTime);
                    targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
                    targetPosition.z = Mathf.Clamp(targetPosition.z, _minZ, _maxZ);
                    transform.position = targetPosition;
                }
            }
        }

        private void DisableCameraMovement(Sketch config)
        {
            _isPlacing = true;
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

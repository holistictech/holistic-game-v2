using System;
using Interactables;
using Spawners;
using UI;
using Unity.VisualScripting;
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
            if (IsInteractingWithUI()) return;
            if (!ButtonsEnabled() && Input.touchCount == 0)
            {
                _spawnedObject.EnableButtons(_finalPosition);
            }

            if (Input.touchCount > 0)
            {
                _spawnedObject.DisableButtonsWhileMoving();
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStart = touch.position;
                    _isSwiping = true;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _isSwiping = false;
                }

                if (_isSwiping)
                {
                    Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane);
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                    if (groundPlane.Raycast(ray, out float enter))
                    {
                        Vector3 worldPosition = ray.GetPoint(enter);

                        // Calculate the difference between current and start positions
                        Vector2 swipeDelta = touch.position - (Vector2)_touchStart;

                        // Convert swipe delta to world movement direction
                        Vector3 movementDirection = new Vector3(swipeDelta.x, 0f, swipeDelta.y).normalized;

                        // Calculate the target position based on the movement direction and move speed
                        var position = _spawnedObject.transform.position;
                        // Optional: Use Lerp for smoother but more responsive movement

                        Vector3 targetPosition = position + movementDirection * (moveSpeed * Time.deltaTime);
                        position = Vector3.Lerp(position, new Vector3(targetPosition.x, position.y, targetPosition.z), 0.1f);

                        // Update the position of the spawned object, keeping the y-axis constant
                        position = new Vector3(targetPosition.x, position.y, targetPosition.z);
                        _spawnedObject.transform.position = position;
                        _finalPosition = targetPosition;
                    }
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

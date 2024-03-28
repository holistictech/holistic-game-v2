using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private RectTransform[] uiElements;
        [SerializeField] private float moveSpeed; 
        [SerializeField] private float boundaryPaddingX;
        [SerializeField] private float boundaryPaddingZ;
        private float _minX, _maxX, _minZ, _maxZ;
        
        private Vector3 _touchStart;
        private bool _isSwiping;

        private void Start()
        {
            var position = transform.position;
            _minX = position.x - boundaryPaddingX;
            _maxX = position.x + boundaryPaddingX;
            _minZ = position.z - boundaryPaddingZ;
            _maxZ = position.z + boundaryPaddingZ;
        }

        private void Update()
        {
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

                    Vector3 targetPosition = transform.position - new Vector3(direction.x, 0 , direction.y) * (moveSpeed * Time.deltaTime);
                    targetPosition.x = Mathf.Clamp(targetPosition.x, _minX, _maxX);
                    targetPosition.z = Mathf.Clamp(targetPosition.z, _minZ, _maxZ);
                    transform.position = targetPosition;
                }
                
            }
        }
    }
}

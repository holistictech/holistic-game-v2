using System;
using UnityEngine;

namespace Utilities
{
    public class SwipeHandler : MonoBehaviour
    {
        [SerializeField] private float swipeThreshold;

        private Vector2 _swipeStartPosition;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _swipeStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 swipeEndPosition = Input.mousePosition;
                Vector2 swipeDirection = swipeEndPosition - _swipeStartPosition;

                if (swipeDirection.magnitude > swipeThreshold)
                {
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(swipeEndPosition.x, swipeEndPosition.y, 10f));
                    Vector3 roundedPosition = new Vector3(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y), 0f);
                    
                    Debug.Log("World position: " + worldPosition);
                    Debug.Log("Rounded position: " + roundedPosition);
                }
            }
        }
    }
}

using UnityEngine;

namespace Utilities
{
    public class ZoomController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        [SerializeField] private Vector3 cameraDefaultPosition;
        
        private float _zoomSpeed = 3f;
        private float _minZoom = 5.0f;
        private float _maxZoom = 80f;

        private Vector2 _touchStartPos;
        private float _previousZoomMagnitude;
        
        void Update()
        {
            if (Input.touchCount == 2)
            {
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);

                Vector2 firstPrevPos = first.position - first.deltaPosition;
                Vector2 secondPrevPos = second.position - second.deltaPosition;

                float prevTouchDelta = (firstPrevPos - secondPrevPos).magnitude;
                float touchDelta = (first.position - second.position).magnitude;

                float deltaMagnitude = _previousZoomMagnitude - (touchDelta - prevTouchDelta);
                Zoom(deltaMagnitude * _zoomSpeed);
                
                _previousZoomMagnitude = touchDelta - prevTouchDelta;
            }
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Zoom(scroll * _zoomSpeed);
        }
        
        private void Zoom(float delta)
        {
            if (mainCamera == null)
                return;

            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - delta, _minZoom, _maxZoom);
        }
    }
}

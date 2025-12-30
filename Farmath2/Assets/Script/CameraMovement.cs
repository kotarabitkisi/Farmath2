using DG.Tweening;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [Header("Kaydýrma Ayarlarý")]
    [Tooltip("Kamera hareketinin hýzý (duyarlýlýðý)")]
    public float panSpeed = 20f;
    public Vector2 camPosMin, camPosMax;

    private Vector3 _dragOrigin;
    public Camera _cam;

    [Header("Zoom Ayarlarý")]
    [Tooltip("Zoom hýzý")]
    public float zoomSpeed = 5f;
    [Tooltip("Maksimum yakýnlaþma (min orthographic size)")]
    public float minZoomSize = 5f;
    [Tooltip("Maksimum uzaklaþma (max orthographic size)")]
    public float maxZoomSize = 20f;



    void Update()
    {
        if (!GameManager.instance.pageopened)
        {
            if (Input.touchSupported)
            {
                HandleMobileZoomInput();
                HandleMobilePanInput();
            }
            else
            {
                HandlePanInput();
                HandleZoomInput();
            }

        }

        ClampCameraPosition();
    }

    private void HandleMobileZoomInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float targetSize = _cam.orthographicSize + deltaMagnitudeDiff * 0.01f * zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, minZoomSize, maxZoomSize);

            _cam.DOOrthoSize(targetSize, 0.1f)
                .SetEase(Ease.Linear);
        }
    }
    private void HandleMobilePanInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _dragOrigin = _cam.ScreenToWorldPoint(touch.position);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(touch.position);

                Vector3 newPosition = transform.position + difference * Time.deltaTime * panSpeed;

                transform.position = newPosition;
            }
        }
    }
    private void HandlePanInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = transform.position + difference * Time.deltaTime * panSpeed;
            transform.position = newPosition;
        }
    }

    private void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            float targetSize = _cam.orthographicSize - scroll * zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, minZoomSize, maxZoomSize);

            _cam.DOOrthoSize(targetSize, 0.2f)
                .SetEase(Ease.OutSine);
        }
    }

    private void ClampCameraPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, camPosMin.x, camPosMax.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, camPosMin.y, camPosMax.y);
        transform.position = clampedPosition;
    }
}


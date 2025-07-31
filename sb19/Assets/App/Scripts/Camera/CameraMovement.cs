using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 2)] private float speed;

  [SerializeField] private SpriteRenderer mapRenderer;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    private Vector3 panStart;
    private Camera cam;
    public bool isOutfitting = false;
    private void Awake()
    {
        
        Vector3 center = mapRenderer.transform.position;
        float halfWidth = mapRenderer.bounds.size.x / 2f;
        float halfHeight = mapRenderer.bounds.size.y / 2f;

        mapMinX = center.x - halfWidth;
        mapMaxX = center.x + halfWidth;
        mapMinY = center.y - halfHeight;
        mapMaxY = center.y + halfHeight;
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void Update()
    {
        if (isOutfitting) return;

        if (Input.GetMouseButtonDown(0))
        {
            panStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 current = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = panStart - current;

            cam.transform.position += direction * speed;
            ClampCamera();

            panStart = current; // Update panStart for continuous dragging*/

           // Vector3 direction = panStart - cam.ScreenToWorldPoint(Input.mousePosition);
           // cam.transform.position += direction * speed;
        }
    }
    
    private void ClampCamera()
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minX = mapMinX + horzExtent;
        float maxX = mapMaxX - horzExtent;
        float minY = mapMinY + vertExtent;
        float maxY = mapMaxY - vertExtent;

        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        cam.transform.position = pos;
    }

    public void LockCamera()
    {
        isOutfitting = true;
        if (cam == null) cam = GetComponent<Camera>();

        // Snap to center
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    public void UnlockCamera()
    {
        isOutfitting = false;
    }
}

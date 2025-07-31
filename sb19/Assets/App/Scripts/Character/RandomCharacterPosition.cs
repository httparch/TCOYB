using UnityEngine;

public class RandomCharacterPosition : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDurationMin = 1f;
    [SerializeField] private float moveDurationMax = 3f;
    [SerializeField] private float pauseDurationMin = 1f;
    [SerializeField] private float pauseDurationMax = 2f;

    [SerializeField] private SpriteRenderer backgroundRenderer;

    private bool isMoving = true;
    private float moveTimer = 0f;
    private float direction = 1f;

    private float minX;
    private float maxX;
    private float halfWidth;

    private void Start()
    {
        if (backgroundRenderer == null)
        {
            //Debug.LogError("Background Renderer not assigned!");
            enabled = false;
            return;
        }

        float bgWidth = backgroundRenderer.bounds.size.x;
        minX = backgroundRenderer.transform.position.x - bgWidth / 2f;
        maxX = backgroundRenderer.transform.position.x + bgWidth / 2f;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        halfWidth = sr != null ? sr.bounds.size.x / 2f : 0.5f;

        StartNewMovement();
    }

    private void Update()
    {

       // HandleMouseClick();

        if (isMoving)
        {
            Vector3 newPosition = transform.position + Vector3.right * direction * moveSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minX + halfWidth, maxX - halfWidth);
            transform.position = newPosition;
        }

        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0)
        {
            if (isMoving)
                StartPause();
            else
                StartNewMovement();
        }
    }

    private void StartNewMovement()
    {
        isMoving = true;
        moveTimer = Random.Range(moveDurationMin, moveDurationMax);

        // Pick a direction based on current position
        if (transform.position.x <= minX + halfWidth)
            direction = 1f; // force right
        else if (transform.position.x >= maxX - halfWidth)
            direction = -1f; // force left
        else
            direction = Random.value > 0.5f ? 1f : -1f;
    }

    private void StartPause()
    {
        isMoving = false;
        moveTimer = Random.Range(pauseDurationMin, pauseDurationMax);
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldMouse.z = transform.position.z;

            Collider2D col = GetComponentInChildren<Collider2D>();
            if (col != null && col.OverlapPoint(worldMouse))
            {
                isMoving = false;
                moveTimer = 10f; // Pause for 1 minute
            }
        }
    }
}

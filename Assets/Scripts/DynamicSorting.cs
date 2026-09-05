using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSorting : MonoBehaviour
{
    [Header("Sorting Thresholds")]
    [Tooltip("If player's relative Y position (0 to 1 of obstacle height) is below this, player is in front.")]
    [Range(0f, 1f)]
    [SerializeField] private float frontThresholdRatio = 1f / 3f; // 1/3 (~0.33)

    [Tooltip("If player's relative Y position (0 to 1 of obstacle height) is above this, player is behind.")]
    [Range(0f, 1f)]
    [SerializeField] private float behindThresholdRatio = 1f / 5f; // 1/5 (~0.2)

    [Header("Layer Order Offset")]
    [SerializeField] private int playerFrontOffset = 1;
    [SerializeField] private int playerBehindOffset = -1;
    [SerializeField] private int defaultSortingOrder = 0;

    [Header("Auto Detection Settings")]
    [Tooltip("Radius around the player to check for obstacles.")]
    [SerializeField] private float detectionRadius = 2f;
    [Tooltip("Layer(s) that obstacles are on.")]
    [SerializeField] private LayerMask obstacleLayer;

    private SpriteRenderer playerSpriteRenderer;

    private void Awake()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerSpriteRenderer == null) return;

        // Detect all colliders in the detection radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, obstacleLayer);
        
        SpriteRenderer closestObstacle = null;
        float minDistance = float.MaxValue;

        // Find the closest obstacle that has a SpriteRenderer
        foreach (var col in colliders)
        {
            // Skip if it's the player itself
            if (col.gameObject == gameObject) continue;

            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestObstacle = sr;
                }
            }
        }

        if (closestObstacle != null)
        {
            // Calculate relative Y position of the player's pivot relative to the obstacle's bounds
            Bounds obstacleBounds = closestObstacle.bounds;
            float obstacleMinY = obstacleBounds.min.y;
            float obstacleHeight = obstacleBounds.size.y;

            if (obstacleHeight > 0)
            {
                // Player's current Y position (using bottom of player's sprite bounds for foot accuracy)
                float playerY = playerSpriteRenderer.bounds.min.y;

                // Normalize player Y position relative to obstacle: 0 at bottom, 1 at top
                float relativeY = (playerY - obstacleMinY) / obstacleHeight;

                if (relativeY <= behindThresholdRatio)
                {
                    // Player is low -> Sorted in front of obstacle
                    playerSpriteRenderer.sortingOrder = closestObstacle.sortingOrder + playerFrontOffset;
                }
                else if (relativeY >= frontThresholdRatio)
                {
                    // Player is high -> Sorted behind obstacle
                    playerSpriteRenderer.sortingOrder = closestObstacle.sortingOrder + playerBehindOffset;
                }
                return;
            }
        }

        // If no obstacle is nearby, reset to default sorting order
        playerSpriteRenderer.sortingOrder = defaultSortingOrder;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection circle in the editor for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

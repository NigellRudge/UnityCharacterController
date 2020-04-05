using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    // This value is to of set the position of the raycast of the objects they are fired from
    const float skinWidth = 0.015f;
    
    //The amount of rays that are fired from both horizontal and vertical positions
    public int horizonralRayCount = 4;
    public int verticallRayCount = 4;

    public LayerMask collisionMask;

    //T
    float horizontalRaySpacing;
    float verticalRaySpacing;
    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizonralRayCount = Mathf.Clamp(horizonralRayCount, 2, int.MaxValue);
        verticallRayCount = Mathf.Clamp(verticallRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizonralRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticallRayCount - 1);
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        if(velocity.x != 0) { 
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0) { 
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticallRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft :raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
            }
        }
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizonralRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft :raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;
            }
        }
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;



    }
}

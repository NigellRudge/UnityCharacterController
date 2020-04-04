using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    const float skinWidth = 0.015f;
    public int horizonralRayCount = 4;
    public int verticallRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;
    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();

        for(int i = 0; i < verticallRayCount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i,Vector2.up * -2,Color.red);
        }
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
    struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;



    }
}

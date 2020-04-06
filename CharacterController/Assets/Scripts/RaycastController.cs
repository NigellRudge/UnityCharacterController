using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    // This value is to of set the position of the raycast of the objects they are fired from
    public const float skinWidth = 0.015f;

    //The amount of rays that are fired from both horizontal and vertical positions
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;


    // A reference to the collisionMask used for the obstacles
    public LayerMask collisionMask;

    //The Horizontal and vertical spacing for the Rays that will be used during detection
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    // Reference to the collider 
    [HideInInspector]
    public BoxCollider2D collider;

    // A Reference the struct that will hold position info of the Raycasts
    public RaycastOrigins raycastOrigins;

    public virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    //This method is used to calculate the spacing of the rays we wil use for collision detection
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }



    // This is a data structure to group the point
    // where the raycast that will be fired will originate from
    public struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

    }
}

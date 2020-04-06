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


    //the maximun angle that a player should be able to climb/walk on
    float maxClimbAngle = 75;

    //The maximun angle that the player can descend from before he falls
    float maxDescendAngle = 70;


    // A reference to the collisionMask used for the obstacles
    public LayerMask collisionMask;

    //The Horizontal and vertical spacing for the Rays that will be used during detection
    float horizontalRaySpacing;
    float verticalRaySpacing;
   
    // Reference to the collider 
    BoxCollider2D collider;

    // A Reference the struct that will hold position info of the Raycasts
    RaycastOrigins raycastOrigins;

    // A Reference the struct that will hold all of the collision information for our player
    public CollisionInfo collisions;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    // This method is used to move the player object
    // It takes a Vector 3 variable and then  calls the methods that handle te collisions
    public void Move(Vector3 velocity)
    {
        //We first upate the origins/points where our rays will be cast from
        UpdateRaycastOrigins();

        // we reset all of the collision info
        collisions.Reset();

        // we store the velocity in our collion info variable so we can use it later
        collisions.velocityOld = velocity;

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    // This method handles the Horizontal collisions.
    // It takes a reference to the Vector3 velocity vector passed the move method
    void HorizontalCollisions(ref Vector3 velocity)
    {
        // we first get the direction on the x-axis that are player is moving
        float directionX = Mathf.Sign(velocity.x);

        // we then define the length that each rays should have (+skinwidth)
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        // In this loop we draw the rays from a starting point
        for (int i = 0; i < horizonralRayCount; i++)
        {
            //based on the direction we are moving (left = -1, right = 1) we are setting the point where we will start
            //drawing the rays from
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            
            // we add the spacing between the rays so that they are evenly spaced from eachother
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            // We fire a raycast from the rayorigin position in de direction of X, with a length equal to
            // our raylength variable and it will detect object in the collisionmask of our collisionMask variable
            // Defined at the top
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            
            // For the porpuse of testing we actually draw the ray so we see it there are firing correctly
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            // we act only is the rays actually hit something
            if (hit)
            {   
                // we store the angle  if the oject we hit
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                // we check if  the angle is smaller or equal the to maximum climb angle
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    // we check if we are descending a slop
                    if (collisions.descendingSlope)
                    {
                        // we set descending slope ti false
                        collisions.descendingSlope = false;

                        // we set the velocity to the old velocity stored in the collision variable
                        velocity = collisions.velocityOld;
                    }

                    // we create a variable to store the distance between the player and the start of the slope
                    float distanceToSlopestart = 0;

                    // we check to see if we are climbing a new slope or not
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        // we set the distance to the new slope
                        distanceToSlopestart = hit.distance - skinWidth;

                        // we detract the distance from the current velocity
                        velocity.x -= distanceToSlopestart * directionX;
                    }

                    // we call the climb slope method and pass a reference of the velocity and the slopeAngle we are going to climb
                    ClimbSlope(ref velocity, slopeAngle);

                    // we add the distance back to the velocioty
                    velocity.x += distanceToSlopestart * directionX;
                }

                // we check to see if we arent climbing a slope and the slopeAngle is more than the max climbAngle
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {

                    // we set the velocity equal to the distance to the slope mi
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;


                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;

                }

            }
        }
    }

    // This method handles the Vertical collisions.
    // It takes a reference to the Vector3 velocity vector passed the move method
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticallRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

            }


        }
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomLeft) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    // This function handles the movement of the player when
    // when moving up a slope
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY =  Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        //collision below is true because player is still grounded

        //check if the user is jumping or not
        if (velocity.y <= climbVelocityY)
        {   
            // we set the velocity on the y-axis equal to the climbvelocity on the y-axis
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

            //set collision below true
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    // This method handles the physics of when the player is moving down a slope
    void DescendSlope(ref Vector3 velocity)
    {
        // we first get the direction on the x-axis
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance -skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    // This method updates the raycast origin for our player. It is called each frame
    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    //This method is used to calculate the spacing of the rays we wil use for collision detection
    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizonralRayCount = Mathf.Clamp(horizonralRayCount, 2, int.MaxValue);
        verticallRayCount = Mathf.Clamp(verticallRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizonralRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticallRayCount - 1);
    }

    

    // This is a data structure to group the point
    // where the raycast that will be fired will originate from
    struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

    }

    // This is a datastructure that will hold the information
    // about the colissions
    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;
        public bool climbingSlope;
        public float slopeAngle;
        public float slopeAngleOld;
        public bool descendingSlope;

        public Vector3 velocityOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            descendingSlope = false;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    float accellarationTimeAirborne = 0.2f;
    float accellarationTimeGrounded = 0.1f;

    // The heights that a player will be able to jump
    public float jumpHeight = 4;
    public float doubleJumpHeight = 3; 
    
    // The times it wil take the player to reach the peak of his jump
    public float timeToJumpApex = 0.4f;
    public float timeToDoubleJumpApex = 0.5f;
    
    // Gravity values for player
    float gravity;
    float doubleJumpGravity;

    //the jump velocities for the player
    float jumpVelocity;
    float doubleJumpVelocity;


    //velocity of movement for the player
    Vector3 velocity;

    //Move speed for the player
    float moveSpeed = 6;

    // Value that will used for smoothing when changing direction
    float velocityXSmoothing;

    // Reference to the controller script
    Controller2D controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        
        //Calculate gravities for player
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
        //calculate jump velocities for player
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
   
        print($"gravity: {gravity}, jumpvelocity: {jumpVelocity}");
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        // get the input from user to move the player
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        // Jumping algorithm
        // Check if user preses jump button
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
                velocity.y = jumpVelocity;
        }

       
        // Here we get the velocity on the X-axis and them we also
        // Account for the change in direction. Based on if the player is on the ground 
        // or airborne. Then we change smoothly between directions
        var targerVelocityX = input.x * moveSpeed;
        var accelTime = (controller.collisions.below) ? accellarationTimeGrounded : accellarationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targerVelocityX,ref velocityXSmoothing,accelTime);

        // Apply Grafity
        velocity.y += gravity * Time.deltaTime;

        //we call the controller script move method
        controller.Move(velocity * Time.deltaTime);
    }
}

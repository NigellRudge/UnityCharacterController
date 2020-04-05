using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    // Basic moveSpeed for Player
    float moveSpeed = 6;
    // Basic gravity for player
    float gravity = -20;
    //velocity of movement for the player
    Vector3 velocity;
    Controller2D controller;
    // Start is called before the first frame update
    void Start()
    {
      controller = GetComponent<Controller2D>();  
    }

    // Update is called once per frame
    void Update()
    {
        // get the input from user to move the player
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Add that input to the velocity
        velocity.x = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

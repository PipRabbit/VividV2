using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;
    public Animator anim;

    private KeyCode lastPressedKey = KeyCode.None;

    private Vector2 moveDirection;

    void Update()
    {
        // Track last key pressed
        if (Input.GetKeyDown(KeyCode.W))
            lastPressedKey = KeyCode.W;
        if (Input.GetKeyDown(KeyCode.S))
            lastPressedKey = KeyCode.S;
        if (Input.GetKeyDown(KeyCode.A))
            lastPressedKey = KeyCode.A;
        if (Input.GetKeyDown(KeyCode.D))
            lastPressedKey = KeyCode.D;

        // Determine movement based on currently held keys
        moveDirection = Vector2.zero;

        // Priority = last pressed key IF still held
        if (IsKeyHeld(lastPressedKey))
        {
            moveDirection = GetDirection(lastPressedKey);
        }
        else
        {
            // Fallback: check other keys still being held
            if (Input.GetKey(KeyCode.W)) moveDirection = Vector2.up;
            else if (Input.GetKey(KeyCode.S)) moveDirection = Vector2.down;
            else if (Input.GetKey(KeyCode.A)) moveDirection = Vector2.left;
            else if (Input.GetKey(KeyCode.D)) moveDirection = Vector2.right;
        }

        // Animator
        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    bool IsKeyHeld(KeyCode key)
    {
        return key != KeyCode.None && Input.GetKey(key);
    }

    Vector2 GetDirection(KeyCode key)
    {
        if (key == KeyCode.W) return Vector2.up;
        if (key == KeyCode.S) return Vector2.down;
        if (key == KeyCode.A) return Vector2.left;
        if (key == KeyCode.D) return Vector2.right;
        return Vector2.zero;
    }
}
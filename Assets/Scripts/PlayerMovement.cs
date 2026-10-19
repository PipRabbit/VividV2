using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Locking")]
    [Tooltip("Freeze the player while a dialogue box is open.")]
    public bool lockMovementDuringDialogue = true;

    [Header("Animator (optional)")]
    [Tooltip("Enable only if your Animator has float parameters LastMoveX and LastMoveY (used for idle facing).")]
    public bool setFacingParameters = false;

    private KeyCode lastPressedKey = KeyCode.None;

    private Vector2 moveDirection;

    public static void Teleport(Transform target, Vector3 position)
    {
        Rigidbody2D body = target.GetComponent<Rigidbody2D>();

        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.position = position;
        }

        target.position = position;
    }

    bool IsLocked()
    {
        if (lockMovementDuringDialogue &&
            DialogueSystem.instance != null &&
            DialogueSystem.instance.IsOpen)
            return true;

        if (RoomTransitionManager.instance != null &&
            RoomTransitionManager.instance.IsTransitioning)
            return true;

        if (ComputerManager.instance != null && ComputerManager.instance.IsOpen)
            return true;

        return false;
    }

    void Update()
    {
        if (IsLocked())
        {
            moveDirection = Vector2.zero;
            UpdateAnimator();
            return;
        }

        // Track last key pressed
        if (Input.GetKeyDown(KeyCode.W))
            lastPressedKey = KeyCode.W;
        if (Input.GetKeyDown(KeyCode.S))
            lastPressedKey = KeyCode.S;
        if (Input.GetKeyDown(KeyCode.A))
            lastPressedKey = KeyCode.A;
        if (Input.GetKeyDown(KeyCode.D))
            lastPressedKey = KeyCode.D;

        moveDirection = Vector2.zero;

        if (IsKeyHeld(lastPressedKey))
        {
            moveDirection = GetDirection(lastPressedKey);
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) moveDirection = Vector2.up;
            else if (Input.GetKey(KeyCode.S)) moveDirection = Vector2.down;
            else if (Input.GetKey(KeyCode.A)) moveDirection = Vector2.left;
            else if (Input.GetKey(KeyCode.D)) moveDirection = Vector2.right;
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", moveDirection.x);
        anim.SetFloat("MoveY", moveDirection.y);

        if (setFacingParameters && moveDirection != Vector2.zero)
        {
            anim.SetFloat("LastMoveX", moveDirection.x);
            anim.SetFloat("LastMoveY", moveDirection.y);
        }
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

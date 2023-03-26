#nullable enable

using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController2D : MonoBehaviour
{
    public CharacterController2D CharacterController = default!;
    public Animator Animator = default!;
    public float RunVelocity = 50f;

    private const string RunSpeed = nameof(RunSpeed);
    private const string JumpTrigger = nameof(JumpTrigger);
    private const string IsGrounded = nameof(IsGrounded);
    private const string VerticalVelocity = nameof(VerticalVelocity);

    private Rigidbody2D Rigidbody = default!;
    private float HorizontalMovement = 0f;
    private bool IsCrouching = false;
    private bool ShouldJump = false;

    private float HorizontalAxis => Input.GetAxisRaw("Horizontal");

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Handle inputs
    void Update()
    {
        HorizontalMovement = HorizontalAxis * RunVelocity;
        if (Input.GetButtonDown("Jump"))
        {
            ShouldJump = true;
            Animator.SetTrigger(JumpTrigger);
        }
        else if (Input.GetButtonDown("Crouch"))
        {
            IsCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            IsCrouching = false;
        }

        Animator.SetFloat(RunSpeed, Math.Abs(HorizontalMovement));
        Animator.SetBool(IsGrounded, CharacterController.IsGrounded);
        Animator.SetFloat(VerticalVelocity, Rigidbody.velocity.y);
        Debug.Log(Rigidbody.velocity.y);
    }

    // Performs actions
    void FixedUpdate()
    {
        var movement = HorizontalMovement * Time.fixedDeltaTime;
        CharacterController.Move(movement, IsCrouching, ShouldJump);
        ShouldJump = false;
    }
}

#nullable restore
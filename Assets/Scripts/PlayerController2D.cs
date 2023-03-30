#nullable enable

using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController2D : MonoBehaviour
{
    public CharacterController2D CharacterController = default!;
    public Animator Animator = default!;
    public float RunVelocity = 50f;

    private Rigidbody2D Rigidbody = default!;
    private float HorizontalMovement = 0f;
    private bool IsCrouching = false;
    private int RemainingJumps = 2;
    private bool ShouldJump = false;

    private bool IsGrounded => CharacterController.IsGrounded;
    private float VerticalVelocity => Rigidbody.velocity.y;

    private float HorizontalAxis => Input.GetAxisRaw("Horizontal");

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        CharacterController.OnLandEvent.AddListener(OnLand);
    }

    private void OnLand()
    {
        Debug.Log("Houston, we have landed!");
        RemainingJumps = 2;
    }

    // Handle inputs
    void Update()
    {
        HorizontalMovement = HorizontalAxis * RunVelocity;
        if (Input.GetButtonDown("Jump"))
        {
            if (--RemainingJumps >= 0)
            {
                ShouldJump = true;
                Animator.SetTrigger("JumpTrigger");
            }

        }
        else if (Input.GetButtonDown("Crouch"))
        {
            IsCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            IsCrouching = false;
        }

        var runSpeed = Math.Abs(Rigidbody.velocity.x);
        Debug.Log(runSpeed);

        Animator.SetFloat("RunSpeed", runSpeed);
        Animator.SetBool(nameof(IsGrounded), IsGrounded);
        Animator.SetFloat(nameof(VerticalVelocity), VerticalVelocity);
        Animator.SetInteger(nameof(RemainingJumps), RemainingJumps);
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
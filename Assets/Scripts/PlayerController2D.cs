#nullable enable

using System;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public CharacterController2D CharacterController = default!;
    public Animator Animator = default!;
    public float RunSpeed = 50f;

    private const string IdleTrigger = nameof(IdleTrigger);
    private const string RunTrigger = nameof(RunTrigger);
    private const string JumpTrigger = nameof(IdleTrigger);
    private const string CrouchTrigger = nameof(IdleTrigger);

    private string LastTrigger = string.Empty;
    private string CurrentTrigger = string.Empty;
    private float HorizontalMovement = 0f;
    private bool IsCrouching = false;
    private bool ShouldJump = false;

    private float HorizontalAxis => Input.GetAxisRaw("Horizontal");

    // Handle inputs
    void Update()
    {
        HorizontalMovement = HorizontalAxis * RunSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            ShouldJump = true;
            CurrentTrigger = JumpTrigger;
        }
        else if (Input.GetButtonDown("Crouch"))
        {
            IsCrouching = true;
            CurrentTrigger = CrouchTrigger;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            IsCrouching = false;
            CurrentTrigger = IdleTrigger;
        }
        else if (Math.Abs(HorizontalMovement) > 0.01)
        {
            CurrentTrigger = RunTrigger;
            Debug.Log(HorizontalMovement);
        }
        else
        {
            CurrentTrigger = IdleTrigger;
        }
    }

    void FixedUpdate()
    {
        var movement = HorizontalMovement * Time.fixedDeltaTime;
        if (CurrentTrigger != LastTrigger)
            Animator.SetTrigger(CurrentTrigger);

        CharacterController.Move(movement, IsCrouching, ShouldJump);
        LastTrigger = CurrentTrigger;
        ShouldJump = false;
    }
}

#nullable restore
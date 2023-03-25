using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D CharacterController;
    public float RunSpeed = 50f;

    private float HorizontalMovement = 0f;
    private bool IsJump = false;
    private bool IsCrouch = false;

    void Update()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal") * RunSpeed;
        if (Input.GetButtonDown("Jump"))
            IsJump = true;
        if (Input.GetButtonDown("Crouch"))
            IsCrouch = true;
        if (Input.GetButtonUp("Crouch"))
            IsCrouch = false;
    }

    void FixedUpdate()
    {
        var movement = HorizontalMovement * Time.fixedDeltaTime;
        CharacterController.Move(movement, IsCrouch, IsJump);
        IsJump = false;
    }
}

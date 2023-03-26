using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public CharacterController2D CharacterController;
    public Animator Animator;
    public float RunSpeed = 50f;

    private const string IDLE_ANIMATION = "Idle";

    private float HorizontalMovement = 0f;
    private bool IsCrouch = false;
    private bool ShouldJump = false;
    private bool AnimationShouldChange = false;

    private float HorizontalAxis => Input.GetAxisRaw("Horizontal");
    private string CurrentAnimation => HorizontalMovement == 0
        ? IDLE_ANIMATION
        : string.Empty;

    void Update()
    {
        HorizontalMovement = HorizontalAxis * RunSpeed;
        if (HorizontalMovement != 0)
        {
            AnimationShouldChange = true;
        }
        if (Input.GetButtonDown("Jump"))
        {
            ShouldJump = true;
            AnimationShouldChange = true;
        }
        if (Input.GetButtonDown("Crouch"))
        {
            IsCrouch = true;
            AnimationShouldChange = true;
        }
        if (Input.GetButtonUp("Crouch"))
        {
            IsCrouch = false;
            AnimationShouldChange = true;
        }
    }

    void FixedUpdate()
    {
        var movement = HorizontalMovement * Time.fixedDeltaTime;
        if (AnimationShouldChange)
            Animator.Play(CurrentAnimation, 0, 0);

        CharacterController.Move(movement, IsCrouch, ShouldJump);
        AnimationShouldChange = false;
        ShouldJump = false;
    }
}

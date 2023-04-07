using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] private float m_JumpForce = 2500f;                          // Amount of force added when the player jumps.
    [Range(0, 1)][SerializeField] private float m_CrouchFactor = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, 20)][SerializeField] private float m_MoveFactor = 10f;
    [Range(0, 1)][SerializeField] private float m_MovementSmoothing = .3f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;

    [Space]
    [Header("Collisions")]

    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    [Space]
    [Header("Events")]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;

    const float k_GroundedRadius = 0.7f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = 0.2f; // Radius of the overlap circle to determine if the player can stand up
    private bool m_IsGrounded;            // Whether or not the player is grounded.
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool m_wasCrouching = false;
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_Velocity = Vector2.zero;

    public bool IsGrounded => m_IsGrounded;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_IsGrounded;
        m_IsGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

        if (colliders.Any(c => c.gameObject != gameObject))
        {
            m_IsGrounded = true;
            if (!wasGrounded)
                OnLandEvent.Invoke();
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_IsGrounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchFactor;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            var targetVelocity = new Vector2(move * m_MoveFactor, m_Rigidbody2D.velocity.y);

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector2.SmoothDamp(
                m_Rigidbody2D.velocity,
                targetVelocity,
                ref m_Velocity,
                m_MovementSmoothing
            );

            if ((move > 0 && !m_FacingRight)
                || (move < 0 && m_FacingRight))
            {
                Flip();
            }
        }

        // If the player should jump...
        if (jump)
        {
            // Add a vertical force to the player.
            m_IsGrounded = false;
            var x = m_Rigidbody2D.velocity.x;
            m_Rigidbody2D.velocity = new Vector2(x, 0);
            m_Rigidbody2D.AddForce(new Vector2(0, m_JumpForce));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
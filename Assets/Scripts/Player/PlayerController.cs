using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Serializable fields
    /// <summary>
    /// The speed of the player
    /// </summary>
    [SerializeField]
    private float speed;
    /// <summary>
    /// When commencing a swing the minimum speed to use
    /// </summary>
    [SerializeField]
    private float minimumSwingSpeed;
    /// <summary>
    /// The amount of speed to gain upon commence a swing
    /// </summary>
    [SerializeField]
    private float swingSpeedBoost;
    /// <summary>
    /// The deceleration to apply during a swing
    /// </summary>
    [SerializeField]
    private float swingDeceleration;
    #endregion

    #region Properties
    /// <summary>
    /// The rigid body belonging to the player
    /// </summary>
    private Rigidbody2D rb;
    /// <summary>
    /// The playerInput component being used to control the player
    /// </summary>
    private PlayerInput playerInput;
    /// <summary>
    /// Is the player currently swinging;
    /// </summary>
    private bool isSwinging;
    /// <summary>
    /// The direction of rotation that the player is currently undergoing
    /// </summary>
    private SwingDirection swingDirection;
    /// <summary>
    /// Directional (sign) modifier based on SwingDirection
    /// </summary>
    private int swingDirectionModifier => (swingDirection == SwingDirection.Clockwise ? 1 : -1);
    /// <summary>
    /// The renderer for the grappling rope
    /// </summary>
    private LineRenderer ropeRenderer;
    /// <summary>
    /// The weapon to fire the grappling rope
    /// </summary>
    private Weapon weapon;
    private bool isShooting => weapon.isShooting;
    /// <summary>
    /// Transform of the last hook which successfully landed
    /// </summary>
    private Transform lastLandedHook;
    #endregion Properties

    #region Unity methods
    void Start()
    {
        // Get components
        ropeRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        weapon = GetComponent<Weapon>();

        isSwinging = false;
    }

    void FixedUpdate()
    {
        UpdatePlayerRotationWhileAiming();

        if (isShooting && !isSwinging && weapon.CheckMaxGrappleRangeExceeded())
        {
            // Check if the max grapple range has been exceeded
            weapon.DestroyHook();
        }

        if (isSwinging)
        {
            Swing();
        }
    }

    void LateUpdate()
    {
        if (isSwinging)
        {
            ropeRenderer.enabled = true;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPositions(new[] { transform.position, lastLandedHook.position });
        }
        else
        {
            ropeRenderer.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag(Tag.Terrain) || collider2D.gameObject.CompareTag(Tag.Death))
        {
            // If the player collides with anything, the game ends
            Destroy(gameObject);
            SceneManager.LoadScene(Scene.Menu);
        }
    }
    #endregion

    #region InputEvents
    private void OnShoot()
    {
        if (!isShooting)
        {
            weapon.Shoot(OnSuccessfulHookshot);
        }
    }

    private void OnRelease()
    {
        if (isSwinging)
        {
            isSwinging = false;
            // Swap action maps once the swing is released
            playerInput.SwitchCurrentActionMap(ActionMap.Flying);
            weapon.DestroyHook();

            // Set the velocity and let Unity handle physics until the next swing
            var swingRadius = transform.position - lastLandedHook.position;
            rb.velocity = Vector3.Cross(Vector3.forward, swingRadius.normalized) * speed * swingDirectionModifier;
        }
    }
    #endregion

    /// <summary>
    /// Rotate the player to face the current cursor position
    /// </summary>
    private void UpdatePlayerRotationWhileAiming()
    {
        var playerPosition = transform.position;
        var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var cursorDirection = cursorPosition - playerPosition;

        var angleRadians = Mathf.Atan2(cursorDirection.x, cursorDirection.y);
        var angleDegrees = Mathf.Rad2Deg * angleRadians;

        transform.rotation = Quaternion.Euler(0, 0, -angleDegrees);
    }

    /// <summary>
    /// Event to invoke on successfully landing a hook
    /// </summary>
    void OnSuccessfulHookshot(object sender, OnSuccessfulHookshotEventArgs e)
    {
        lastLandedHook = e.hookTransform;

        playerInput.SwitchCurrentActionMap(ActionMap.Swinging);

        // Unity doesn't seem to handle the swinging physics in the way I want, so I'll do it myself
        var swingRadius = transform.position - e.hookTransform.position;
        var swingTangent = Vector3.Cross(swingRadius, Vector3.forward);

        var angleBetween = Vector3.Angle(swingTangent, rb.velocity);
        swingDirection = angleBetween > 90 ? SwingDirection.Clockwise : SwingDirection.AntiClockwise;

        speed = MathF.Max(rb.velocity.magnitude + swingSpeedBoost, minimumSwingSpeed);

        rb.velocity = Vector2.zero;
        isSwinging = true;
    }

    /// <summary>
    /// Update the position of the player as they swing around the landed hook
    /// </summary>
    void Swing()
    {
        var swingRadius = transform.position - lastLandedHook.position;
        var angularVelocityRadians = speed / swingRadius.magnitude;
        var angularVelocityDegrees = Mathf.Rad2Deg * angularVelocityRadians;

        // Factor in whether to swing clockwise or anticlockwise
        var angle = swingDirectionModifier * angularVelocityDegrees;

        transform.RotateAround(lastLandedHook.position, Vector3.forward, angle * Time.deltaTime);

        //While swinging, decelarate
        speed = Mathf.Sign(speed) * (Mathf.Abs(speed) - swingDeceleration);
    }

    #region Public methods
    public float GetCurrentSpeed()
    {
        if (isSwinging)
        {
            return speed;
        }
        return rb.velocity.magnitude;
    }
    #endregion
}

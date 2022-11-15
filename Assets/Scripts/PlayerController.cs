using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Serializable fields
    /// <summary>
    /// The point from which the grappling hook is fired
    /// </summary>
    [SerializeField]
    private Transform firePoint;
    /// <summary>
    /// Prefab for the hook being fired
    /// </summary>
    [SerializeField]
    private Hook hookPrefab;
    /// <summary>
    /// The maximum range of the grappling hook being fired
    /// </summary>
    [SerializeField]
    private float maxGrappleRange;
    /// <summary>
    /// The renderer for the grappling rope
    /// </summary>
    [SerializeField]
    private LineRenderer ropeRenderer;
    /// <summary>
    /// The speed of the player (currently fixed, TODO decide how to spice it up)
    /// </summary>
    [SerializeField]
    private float speed;
    /// <summary>
    /// When commencing a swing the minimum speed to use
    /// </summary>
    [SerializeField]
    private readonly float minimumSwingSpeed = 5;
    /// <summary>
    /// The amount of speed to gain upon commence a swing
    /// </summary>
    [SerializeField]
    private float swingAcceleration = 2;
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
    /// Is the hook currently mid-flight
    /// </summary>
    private bool isShooting;
    /// <summary>
    /// The instance of the hook being fired
    /// </summary>
    private Hook hookObject;
    /// <summary>
    /// The direction of rotation that the player is currently undergoing
    /// </summary>
    private SwingDirection swingDirection;
    /// <summary>
    /// Directional (sign) modifier based on SwingDirection
    /// </summary>
    private int swingDirectionModifier => (swingDirection == SwingDirection.Clockwise ? 1 : -1);
    #endregion Properties

    #region Unity methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        isSwinging = false;
        isShooting = false;
    }

    void FixedUpdate()
    {
        UpdatePlayerRotationWhileAiming();

        if (this.isShooting && !this.isSwinging && CheckMaxGrappleRangeExceeded())
        {
            // Check if the max grapple range has been exceeded
            this.isShooting = false;
            this.hookObject.Destroy();
        }

        if (isSwinging)
        {
            Swing();
        }
    }

    void LateUpdate()
    {
        if (isShooting || isSwinging)
        {
            ropeRenderer.enabled = true;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPositions(new[] { transform.position, hookObject.transform.position });
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
            Destroy(this.gameObject);
            SceneManager.LoadScene(Scene.Menu);
        }
    }
    #endregion

    #region InputEvents
    private void OnShoot()
    {
        if (!isShooting)
        {
            this.isShooting = true;

            var firePointPosition = this.firePoint.position;
            var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            var hookDirection = cursorPosition - firePointPosition;

            this.hookObject = Instantiate<Hook>(hookPrefab, firePointPosition, this.firePoint.rotation);

            // Set the velocity of the hook which was fired
            this.hookObject.SetDirection(hookDirection);
            // Add the OnSuccessfulHookshot event listener
            this.hookObject.OnSuccessfulHookshot += OnSuccessfulHookshot;
        }
    }

    private void OnRelease()
    {
        if (this.isSwinging)
        {
            this.isSwinging = false;
            // Swap action maps once the swing is released
            playerInput.SwitchCurrentActionMap(ActionMap.Flying);
            this.hookObject.Destroy();

            // Set the velocity and let Unity handle physics until the next swing
            var swingRadius = transform.position - hookObject.transform.position;
            rb.velocity = Vector3.Cross(Vector3.forward, swingRadius.normalized) * speed * swingDirectionModifier;
        }
    }
    #endregion

    /// <summary>
    /// Check if the maximum grapple range has been exceeded by a hook which is currently mid-flight
    /// </summary>
    private bool CheckMaxGrappleRangeExceeded()
    {
        if (this.hookObject == null)
        {
            return false;
        }
        var hookDisplacement = this.hookObject.transform.position - this.firePoint.transform.position;

        return hookDisplacement.magnitude > this.maxGrappleRange;
    }

    /// <summary>
    /// Rotate the player to face the current cursor position
    /// </summary>
    private void UpdatePlayerRotationWhileAiming()
    {
        var playerPosition = this.transform.position;
        var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var cursorDirection = cursorPosition - playerPosition;

        var angleRadians = Mathf.Atan2(cursorDirection.x, cursorDirection.y);
        var angleDegrees = Mathf.Rad2Deg * angleRadians;

        this.transform.rotation = Quaternion.Euler(0, 0, -angleDegrees);
    }

    /// <summary>
    /// Event to invoke on successfully landing a hook
    /// </summary>
    void OnSuccessfulHookshot(object sender, EventArgs e)
    {
        playerInput.SwitchCurrentActionMap(ActionMap.Swinging);

        // Unity doesn't seem to handle the swinging physics in the way I want, so I'll do it myself
        var swingRadius = transform.position - hookObject.transform.position;
        var swingTangent = Vector3.Cross(swingRadius, Vector3.forward);

        var angleBetween = Vector3.Angle(swingTangent, rb.velocity);
        swingDirection = angleBetween > 90 ? SwingDirection.Clockwise : SwingDirection.AntiClockwise;

        speed = MathF.Max(rb.velocity.magnitude + swingAcceleration, minimumSwingSpeed);

        rb.velocity = Vector2.zero;
        this.isShooting = false;
        this.isSwinging = true;
    }

    /// <summary>
    /// Update the position of the player as they swing around the landed hook
    /// </summary>
    void Swing()
    {
        var swingRadius = transform.position - hookObject.transform.position;
        var angularVelocityRadians = speed / swingRadius.magnitude;
        var angularVelocityDegrees = Mathf.Rad2Deg * angularVelocityRadians;

        // Factor in whether to swing clockwise or anticlockwise
        var angle = swingDirectionModifier * angularVelocityDegrees;

        transform.RotateAround(hookObject.transform.position, Vector3.forward, angle * Time.deltaTime);
    }
    #region Public methods
    public int GetCurrentSpeed()
    {
        if (isSwinging)
        {
            return (int)speed;
        }
        return (int)rb.velocity.magnitude;
    }
    #endregion
}

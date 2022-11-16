using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Weapon))]
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
    /// The weapon to fire the grappling rope
    /// </summary>
    private Weapon weapon;
    private bool isShooting => weapon.isShooting;
    private Vector3 swingCentre => GameManager.gameManager.currentSwingCentre ?? Vector3.zero;
    #endregion Properties

    #region Unity lifecycle methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponent<Weapon>();

        isSwinging = false;
        GameManager.gameManager.OnHookLandedEvent += OnSuccessfulHookshot;
        GameManager.gameManager.OnReleaseEvent += OnRelease;
    }

    void FixedUpdate()
    {
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
    #endregion

    #region Events
    private void OnRelease(object sender, EventArgs e)
    {
        if (isSwinging)
        {
            isSwinging = false;
            // Swap action maps once the swing is released
            GameManager.gameManager.SwitchCurrentActionMap(ActionMap.Flying);

            // Set the velocity and let Unity handle physics until the next swing
            var swingRadius = transform.position - swingCentre;
            rb.velocity = Vector3.Cross(Vector3.forward, swingRadius.normalized) * speed * swingDirectionModifier;
        }
    }

    /// <summary>
    /// Event to invoke on successfully landing a hook
    /// </summary>
    private void OnSuccessfulHookshot(object sender, EventArgs e)
    {

        GameManager.gameManager.SwitchCurrentActionMap(ActionMap.Swinging);

        // Unity doesn't seem to handle the swinging physics in the way I want, so I'll do it myself
        var swingRadius = transform.position - swingCentre;
        // swingCentre will not be null here
        var swingTangent = Vector3.Cross(swingRadius, Vector3.forward);

        var angleBetween = Vector3.Angle(swingTangent, rb.velocity);
        swingDirection = angleBetween > 90 ? SwingDirection.Clockwise : SwingDirection.AntiClockwise;

        speed = MathF.Max(rb.velocity.magnitude + swingSpeedBoost, minimumSwingSpeed);

        rb.velocity = Vector2.zero;
        isSwinging = true;
    }
    #endregion

    /// <summary>
    /// Update the position of the player as they swing around the landed hook
    /// </summary>
    void Swing()
    {
        var swingRadius = transform.position - swingCentre;
        var angularVelocityRadians = speed / swingRadius.magnitude;
        var angularVelocityDegrees = Mathf.Rad2Deg * angularVelocityRadians;

        // Factor in whether to swing clockwise or anticlockwise
        var angle = swingDirectionModifier * angularVelocityDegrees;

        transform.RotateAround(swingCentre, Vector3.forward, angle * Time.deltaTime);

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

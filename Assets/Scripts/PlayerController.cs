using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (!this.isSwinging)
        {
            // While not swinging, the player should face the current cursor location
            UpdatePlayerRotationWhileAiming();
        }


        if (this.isShooting && !this.isSwinging && CheckMaxGrappleRangeExceeded())
        {
            // Check if the max grapple range has been exceeded
            this.isShooting = false;
            this.hookObject.Destroy();
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Terrain"))
        {
            // If the player collides with any terrain, the game ends
            Destroy(this.gameObject, .5f);
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
        this.isShooting = false;
        this.isSwinging = true;
        playerInput.SwitchCurrentActionMap(ActionMap.Swinging);
    }

}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour
{
    #region Serializable fields    
    /// <summary>
    /// The maximum range of the grappling hook being fired
    /// </summary>
    [SerializeField]
    private float maxGrappleRange;
    /// <summary>
    /// Prefab for the hook being fired
    /// </summary>
    [SerializeField]
    private Hook hookPrefab;
    /// <summary>
    /// The point from which the grappling hook is fired
    /// </summary>
    [SerializeField]
    private Transform firePoint;
    #endregion
    #region Properties    
    /// <summary>
    /// Is the hook currently mid-flight
    /// </summary>
    public bool isShooting { get; private set; }
    /// <summary>
    /// The instance of the hook being fired
    /// </summary>
    public Hook hookObject { get; private set; }
    /// <summary>
    /// The renderer for the grappling rope
    /// </summary>
    private LineRenderer ropeRenderer;
    #endregion

    void Start()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        GameManager.gameManager.OnShootEvent += OnShoot;
    }

    void LateUpdate()
    {
        if (hookObject != null)
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

    private void OnShoot(object sender, EventArgs e)
    {
        if (!isShooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        isShooting = true;

        var firePointPosition = firePoint.position;
        var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var hookDirection = cursorPosition - firePointPosition;

        hookObject = Instantiate<Hook>(hookPrefab, firePointPosition, firePoint.rotation);

        // Set the velocity of the hook which was fired
        hookObject.SetDirection(hookDirection);
        // Add an event handler
        GameManager.gameManager.OnHookLandedEvent += (object sender, OnSuccessfulHookshotEventArgs e) => isShooting = false;
    }

    #region Public methods
    /// <summary>
    /// Check if the maximum grapple range has been exceeded by a hook which is currently mid-flight
    /// </summary>
    public bool CheckMaxGrappleRangeExceeded()
    {
        if (hookObject == null)
        {
            return false;
        }
        var hookDisplacement = hookObject.transform.position - firePoint.transform.position;

        return hookDisplacement.magnitude > maxGrappleRange;
    }

    public void DestroyHook()
    {
        hookObject.Destroy();
        isShooting = false;
    }
    #endregion
}

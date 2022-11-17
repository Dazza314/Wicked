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
    private bool isShooting { get; set; }
    /// <summary>
    /// The instance of the hook being fired
    /// </summary>
    private Hook hookObject { get; set; }
    /// <summary>
    /// The renderer for the grappling rope
    /// </summary>
    private LineRenderer ropeRenderer;
    #endregion

    void Start()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        GameManager.gameManager.OnShootEvent += OnShoot;
        GameManager.gameManager.OnHookLandedEvent += (object sender, EventArgs e) => isShooting = false;
        GameManager.gameManager.OnHookLandedOnWallEvent += (object sender, EventArgs e) => isShooting = false;
    }

    void Update()
    {
        if (isShooting && CheckMaxGrappleRangeExceeded())
        {
            // Check if the max grapple range has been exceeded
            Destroy(hookObject.gameObject);
            isShooting = false;
        }
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
            isShooting = true;

            var firePointPosition = firePoint.position;
            var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            var hookDirection = cursorPosition - firePointPosition;

            hookObject = Instantiate<Hook>(hookPrefab, firePointPosition, firePoint.rotation);

            // Set the direction of the hook which was fired
            hookObject.SetDirection(hookDirection);
        }
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
    #endregion
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    #endregion

    #region Public methods
    public void Shoot(EventHandler<OnSuccessfulHookshotEventArgs> onSuccessfulHookshot)
    {
        isShooting = true;

        var firePointPosition = firePoint.position;
        var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var hookDirection = cursorPosition - firePointPosition;

        hookObject = Instantiate<Hook>(hookPrefab, firePointPosition, firePoint.rotation);

        // Set the velocity of the hook which was fired
        hookObject.SetDirection(hookDirection);
        // Add the OnSuccessfulHookshot event listener
        hookObject.OnSuccessfulHookshot += onSuccessfulHookshot;
        // Add an extra success function
        hookObject.OnSuccessfulHookshot += (object sender, OnSuccessfulHookshotEventArgs e) => isShooting = false;
    }

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

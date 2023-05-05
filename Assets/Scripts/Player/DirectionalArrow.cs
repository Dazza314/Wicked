using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DirectionalArrow : MonoBehaviour
{
    #region Serializable fields
    [SerializeField]
    private GameObject swingDirectionArrowPrefab;
    #endregion
    #region Properties
    /// <summary>
    /// The rigid body belonging to the player
    /// </summary>
    private Rigidbody2D rb;
    private GameObject swingDirectionArrow;
    /// <summary>
    /// Whether or not to show the arrow as set in player prefs
    /// </summary>
    private EventHandler onReleaseEventHandler, onHookLandedEventHandler;
    private bool showArrowPlayerPref => Convert.ToBoolean(PlayerPrefs.GetInt(Options.ShowDirectionArrow, 0));
    #endregion

    #region Unity lifecycle methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (showArrowPlayerPref)
        {
            swingDirectionArrow = Instantiate(swingDirectionArrowPrefab);
            swingDirectionArrow.transform.parent = transform;
        }

        onHookLandedEventHandler = (object sender, EventArgs e) => HideArrow();
        GameManager.gameManager.OnHookLandedEvent += onHookLandedEventHandler;
        onReleaseEventHandler = (object sender, EventArgs e) => ShowArrow();
        GameManager.gameManager.OnReleaseEvent += onReleaseEventHandler;
    }

    void FixedUpdate()
    {
        UpdateArrow();
    }

    void OnDestroy()
    {
        GameManager.gameManager.OnHookLandedEvent -= onHookLandedEventHandler;
        GameManager.gameManager.OnReleaseEvent -= onReleaseEventHandler;
    }
    #endregion

    private void UpdateArrow()
    {
        if (showArrowPlayerPref)
        {
            swingDirectionArrow.transform.position = transform.position;

            var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            cursorPosition.z = 0;
            var swingRadius = transform.position - cursorPosition;
            var swingTangent = Vector3.Cross(swingRadius, Vector3.forward);
            var angleBetween = Vector3.Angle(swingTangent, rb.velocity);

            var directionMultiplier = angleBetween < 90 ? 1 : -1;

            var nextSwingDirection = swingTangent * directionMultiplier;

            swingDirectionArrow.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(nextSwingDirection.x, nextSwingDirection.y) * Mathf.Rad2Deg);
        }
    }

    private void HideArrow()
    {
        if (showArrowPlayerPref)
        {
            swingDirectionArrow.SetActive(false);
        }
    }

    private void ShowArrow()
    {
        if (showArrowPlayerPref)
        {
            swingDirectionArrow.SetActive(true);
        }
    }
}

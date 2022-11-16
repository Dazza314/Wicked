using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class GameManager : MonoBehaviour
{
    #region Properties
    public static GameManager gameManager { get; private set; }
    private PlayerInput playerInput;
    /// <summary>
    /// The current position of the landed hook. If no hook has been landed, this will be null
    /// </summary>
    public Vector3? currentSwingCentre { get; private set; }
    #endregion

    #region Unity life cycle methods
    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManager = this;
        }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    #endregion

    #region Events
    public void OnDeath()
    {
        SceneManager.LoadScene(Scene.Menu);
    }

    #region Shoot
    public event EventHandler OnShootEvent;
    public void OnShoot()
    {
        OnShootEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Hook landed
    public event EventHandler OnHookLandedEvent;
    public void OnHookLanded(Transform hookTransform)
    {
        currentSwingCentre = hookTransform.position;
        OnHookLandedEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Release
    public event EventHandler OnReleaseEvent;
    public void OnRelease()
    {
        OnReleaseEvent?.Invoke(this, EventArgs.Empty);
        currentSwingCentre = null;
    }
    #endregion
    #endregion

    public void SwitchCurrentActionMap(string actionMap)
    {
        playerInput.SwitchCurrentActionMap(actionMap);
    }
}

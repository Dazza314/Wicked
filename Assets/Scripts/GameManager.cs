using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class GameManager : MonoBehaviour
{
    public static GameManager gameManager { get; private set; }
    /// <summary>
    /// The playerInput component being used to control the player
    /// </summary>
    private PlayerInput playerInput;

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

    #region Events
    /// <summary>
    /// On player death event
    /// </summary>
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
    public event EventHandler<OnSuccessfulHookshotEventArgs> OnHookLandedEvent;
    public void OnHookLanded(Transform hookTransform)
    {
        var args = new OnSuccessfulHookshotEventArgs()
        {
            hookTransform = hookTransform
        };
        OnHookLandedEvent?.Invoke(this, args);
    }
    #endregion

    #region Release
    public event EventHandler OnReleaseEvent;
    public void OnRelease()
    {
        OnReleaseEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion
    #endregion

    public void SwitchCurrentActionMap(string actionMap)
    {
        playerInput.SwitchCurrentActionMap(actionMap);
    }
}

public class OnSuccessfulHookshotEventArgs : EventArgs
{
    public Transform hookTransform;
}
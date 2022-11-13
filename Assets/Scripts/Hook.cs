using System;
using UnityEngine;

public class Hook : MonoBehaviour
{
    #region Serializable fields
    /// <summary>
    /// The speed of the hook projectile
    /// </summary>
    [SerializeField]
    private float speed;
    /// <summary>
    /// The rigid body belonging to the hook
    /// </summary>
    [SerializeField]
    private Rigidbody2D rb;
    #endregion
    #region Properties
    /// <summary>
    /// The velocity of the hook projectile
    /// </summary>
    private Vector2 velocity;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            OnSuccessfulHookshot.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Set the direction of the hook projectile. Note that this does not affect the speed
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        direction.Normalize();
        velocity = direction * speed;
        rb.velocity = velocity;
    }

    /// <summary>
    /// Destroy the hook game object and do any associated cleanup
    /// </summary>
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    #region Events
    /// <summary>
    /// Event invoked on successfully landing a hook on terrain
    /// </summary>
    public event EventHandler OnSuccessfulHookshot;
    #endregion
}

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
        EventHandler releaseEventHandler = null;
        releaseEventHandler = (object sender, EventArgs e) =>
        {
            Destroy();
            // Remove event handler after being invoked once as this instance of Hook will no longer exist
            GameManager.gameManager.OnReleaseEvent -= releaseEventHandler;
        };
        GameManager.gameManager.OnReleaseEvent += releaseEventHandler;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            GameManager.gameManager.OnHookLanded(transform);
            // Create a joint to hold the hook fixed against the terrain
            var terrainJoint = gameObject.AddComponent<FixedJoint2D>();
            terrainJoint.enableCollision = false;

            terrainJoint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            terrainJoint.enabled = true;
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
        Destroy(gameObject);
    }
}


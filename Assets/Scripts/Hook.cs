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
    private EventHandler destroyEventHandler;
    #endregion

    #region Unity lifecycle methods
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        destroyEventHandler = (object sender, EventArgs e) =>
        {
            // Remove event handler after being invoked once as this instance of Hook will no longer exist
            // GameManager.gameManager.OnReleaseEvent -= destroyEventHandler;
            Destroy(gameObject);
        };
        GameManager.gameManager.OnReleaseEvent += destroyEventHandler;
    }

    void OnDestroy()
    {
        // When destroying the hook, remove the eventHandler which handles swing release
        GameManager.gameManager.OnReleaseEvent -= destroyEventHandler;
    }
    #endregion

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
        else if (collision.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            GameManager.gameManager.OnHookLandedOnWall();
        }
    }

    /// <summary>
    /// Set the direction of the hook projectile. Note that this does not affect the speed
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        direction.Normalize();
        rb.velocity = direction * speed;
    }
}

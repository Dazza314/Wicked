using UnityEngine;

public class DeathCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag(Tag.Terrain) || collider2D.gameObject.CompareTag(Tag.Death))
        {
            // If the player collides with anything, the game ends
            Destroy(gameObject);
            GameManager.gameManager.OnDeath();
        }
    }
}

using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    #region Serializable fields
    [SerializeField] private ParticleSystem particleSystemPrefab;
    #endregion

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag(Tag.Terrain) || collider2D.gameObject.CompareTag(Tag.Death))
        {
            // If the player collides with anything, the game ends
            GameManager.gameManager.OnDeath();
        }
    }

    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag(Tag.Terrain) || collision2D.gameObject.CompareTag(Tag.Death))
        {
            var contacts = new ContactPoint2D[1];
            collision2D.GetContacts(contacts);
            Explode(contacts[0]);
            Destroy(this.gameObject);
            GameManager.gameManager.OnDeath();
        }
    }

    private void Explode(ContactPoint2D contactPoint)
    {
        var position = contactPoint.point;
        Instantiate(particleSystemPrefab, position, Quaternion.identity);
    }
}

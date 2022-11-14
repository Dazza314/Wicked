using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    #region Serializable fields
    [SerializeField]
    private GameObject pillarPrefab;
    #endregion

    void Start()
    {
        var pillarCount = 100;

        var pillarPosition = new Vector3();

        for (int i = 0; i < pillarCount; i++)
        {
            pillarPosition.y += Random.Range(1.5f, 4f);
            pillarPosition.x = Random.Range(-8f, 8f);
            Instantiate(pillarPrefab, pillarPosition, Quaternion.identity);
        }

    }

}

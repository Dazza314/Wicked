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
            var minYGap = yGap(pillarPosition.y);
            pillarPosition.y += Random.Range(minYGap, 9f - minYGap);
            pillarPosition.x = Random.Range(-6f, 6f);
            Instantiate(pillarPrefab, pillarPosition, Quaternion.identity);
        }

    }

    private float yGap(float currentY)
    {
        return (float)(5 - System.Math.Tanh(currentY / 100));
    }

}

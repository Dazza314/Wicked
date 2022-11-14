using UnityEngine;

public class Walls : MonoBehaviour
{
    [SerializeField]
    private int numberOfWalls = 3;
    private float height => transform.localScale.y;

    void Update()
    {
        var topOfWall = transform.position.y + height / 2;
        var bottomOfWall = transform.position.y - height / 2;
        if (cameraBottomY > topOfWall)
        {
            transform.position += Vector3.up * height * numberOfWalls;
        }
        else if (cameraTopY < bottomOfWall)
        {
            transform.position += Vector3.down * height * numberOfWalls;
        }
    }

    #region Camera calculations
    private float cameraTopY => Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane)).y;
    private float cameraBottomY => Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y;
    #endregion
}

using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Serializable fields
    /// <summary>
    /// The TextMeshPro to display the distance travelled
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI distanceText;
    /// <summary>
    /// The TextMeshPro to display the current speed
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI currentSpeedText;
    [SerializeField]
    private PlayerController player;
    #endregion

    void Update()
    {
        distanceText.text = GetDistance();
        currentSpeedText.text = GetCurrentSpeed();
    }

    private string GetDistance()
    {
        string distance = string.Empty;
        if (player != null)
        {
            int playerY = ((int)player.transform.position.y);
            if (playerY < 0)
            {
                playerY = 0;
            }
            distance = playerY.ToString();
        }
        return distance;
    }

    private string GetCurrentSpeed()
    {
        string currentSpeed = string.Empty;
        if (player != null)
        {
            currentSpeed = player.GetCurrentSpeed().ToString();
        }
        return currentSpeed;
    }
}

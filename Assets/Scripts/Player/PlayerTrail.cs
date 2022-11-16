using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    #region Serializable fields
    [SerializeField]
    private TrailRenderer playerTrail;
    [SerializeField]
    private PlayerController playerController;
    #endregion

    void Update()
    {
        var currentSpeed = playerController.GetCurrentSpeed();
        playerTrail.time = currentSpeed / 7.5f;
    }
}

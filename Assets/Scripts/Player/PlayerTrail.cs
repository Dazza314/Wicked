using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerTrail : MonoBehaviour
{
    #region Serializable fields
    [SerializeField]
    private TrailRenderer playerTrail;
    #endregion
    #region Properties
    private PlayerController playerController;
    #endregion

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        var currentSpeed = playerController.GetCurrentSpeed();
        playerTrail.time = currentSpeed / 7.5f;
    }
}

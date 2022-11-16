using UnityEngine;
using UnityEngine.InputSystem;

public class Aiming : MonoBehaviour
{
    void FixedUpdate()
    {
        UpdatePlayerRotationWhileAiming();
    }

    /// <summary>
    /// Rotate the player to face the current cursor position
    /// </summary>
    private void UpdatePlayerRotationWhileAiming()
    {
        var playerPosition = transform.position;
        var cursorPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var cursorDirection = cursorPosition - playerPosition;

        var angleRadians = Mathf.Atan2(cursorDirection.x, cursorDirection.y);
        var angleDegrees = Mathf.Rad2Deg * angleRadians;

        transform.rotation = Quaternion.Euler(0, 0, -angleDegrees);
    }
}

using System;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    void Start()
    {
        GameManager.gameManager.OnPlayAgainEvent += (object sender, EventArgs e) => PlayAgain();
    }

    #region Play again
    public void OnPlayAgainClicked()
    {
        PlayAgain();
        GameManager.gameManager.OnPlayAgain();
    }
    private void PlayAgain()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}

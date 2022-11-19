using System;
using TMPro;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    #region Serializable fields
    [SerializeField] private TextMeshProUGUI toggleDirectionArrowText;
    #endregion


    #region Unity lifecycle methods
    void Start()
    {
        GameManager.gameManager.OnPlayAgainEvent += (object sender, EventArgs e) => PlayAgain();
    }

    void Update()
    {
        SetToggleDirectionArrowText();
    }
    #endregion

    #region Play again
    public void OnPlayAgainClicked()
    {
        GameManager.gameManager.OnPlayAgain();
    }
    private void PlayAgain()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
    #region Toggle direction arrow
    public void OnToggleDirectionArrowClicked()
    {
        var oldValue = PlayerPrefs.GetInt(Options.ShowDirectionArrow, 0);
        var newValue = oldValue ^ 1;
        PlayerPrefs.SetInt(Options.ShowDirectionArrow, newValue);
    }
    private void SetToggleDirectionArrowText()
    {
        var currentValue = PlayerPrefs.GetInt(Options.ShowDirectionArrow, 0);
        toggleDirectionArrowText.text = String.Format("{0} ARROW", currentValue == 1 ? "HIDE" : "SHOW");
    }
    #endregion
}

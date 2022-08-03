using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChessMenu : MonoBehaviour
{
    public void playerVsAIWhitePiece()
    {
        Debug.Log("PlayerVsAIWhiteButton has been clicked");
        GameController.gameMode = 1;
        GameController.selectedPlayerColor = 'w';
        SceneManager.LoadScene("GameScene");
    }
    public void playerVsAIBlackPiece()
    {
        Debug.Log("PlayerVsAIBlackButton has been clicked");
        GameController.gameMode = 1;
        GameController.selectedPlayerColor = 'b';
        SceneManager.LoadScene("GameScene");
    }
    public void aIVsAi()
    {
        Debug.Log("AIvsAIButton has been clicked");
        GameController.gameMode = 2;

        SceneManager.LoadScene("GameScene");
    }
    public void backButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void exitButton()
    {
        Application.Quit();
    }
}

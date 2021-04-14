using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameStateText = null;
    [SerializeField] TextMeshProUGUI playerStateText = null;

    // Start is called before the first frame update
    void Start()
    {
        GameController.instance.onGameStateChangedCallback += UpdateGameStateUI;
        GameController.instance.onPlayerStateChangedCallback += UpdatePlayerStateUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateGameStateUI(GameController.GameState newState)
    {
        string result = "GameState: ";
        switch (newState)
        {
            case GameController.GameState.GAME_START:
                result += "GAME_START";
                break;
            case GameController.GameState.BETWEEN_ROUNDS:
                result += "BETWEEN_ROUNDS";
                break;
            case GameController.GameState.SPAWNING_ENEMIES:
                result += "SPAWNING_ENEMIES";
                break;
            case GameController.GameState.ENEMIES_ALIVE:
                result += "ENEMIES_ALIVE";
                break;
            case GameController.GameState.GAME_OVER:
                result += "GAME_OVER";
                break;
            default:
                result += "N/A";
                break;
        }
        gameStateText.text = result;
    }

    void UpdatePlayerStateUI(PlayerController.PlayerState newState)
    {
        string result = "PlayerState: ";
        switch(newState) {
            case PlayerController.PlayerState.NORMAL:
                result += "NORMAL";
                break;
            case PlayerController.PlayerState.JUMPING:
                result += "JUMPING";
                break;
            case PlayerController.PlayerState.RUNNING:
                result += "RUNNING";
                break;
            case PlayerController.PlayerState.LOCKINPUT:
                result += "LOCKINPUT";
                break;
            case PlayerController.PlayerState.FREEZE:
                result += "FREEZE";
                break;
            default:
                result += "N/A";
                break;
        }
        playerStateText.text = result;
    }
}

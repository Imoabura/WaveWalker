using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    [SerializeField] CanvasGroup GameUI;
    [SerializeField] CanvasGroup StartMenuUI;
    [SerializeField] CanvasGroup GameOverUI;

    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameUI.interactable = false;
        GameUI.blocksRaycasts = false;
        GameUI.alpha = 0f;

        GameOverUI.interactable = false;
        GameOverUI.blocksRaycasts = false;
        GameOverUI.alpha = 0f;

        StartMenuUI.interactable = true;
        StartMenuUI.blocksRaycasts = true;
        StartMenuUI.alpha = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        FadeOutUI(StartMenuUI);
        FadeInUI(GameUI);
        GameController.instance.StartGame();
    }

    public void GameOver()
    {
        FadeOutUI(GameUI);
        FadeInUI(GameOverUI);
    }

    public void ReturnToMenu()
    {
        FadeOutUI(GameOverUI);
        FadeInUI(StartMenuUI);
    }

    void FadeInUI(CanvasGroup uiElement)
    {
        StartCoroutine(FadeInTransition(uiElement));
    }

    void FadeOutUI(CanvasGroup uiElement)
    {
        StartCoroutine(FadeOutTransition(uiElement));
    }

    IEnumerator FadeInTransition(CanvasGroup uiElement)
    {
        timer = 0f;
        uiElement.interactable = true;
        uiElement.blocksRaycasts = true;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            uiElement.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;  // wait until next frame
        }

        uiElement.alpha = 1f;
    }

    IEnumerator FadeOutTransition(CanvasGroup uiElement)
    {
        timer = 0f;
        uiElement.interactable = false;
        uiElement.blocksRaycasts = false;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            uiElement.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;  // wait until next frame
        }

        uiElement.alpha = 0f;
    }
}

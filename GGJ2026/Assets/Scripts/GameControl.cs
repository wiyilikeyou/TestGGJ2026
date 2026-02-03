using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class GameControl : Singleton<GameControl>
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI countDown;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gameStop;
    [SerializeField] private GameObject scroll;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MMFeedbacks scoreFeedback;
    [SerializeField] private Image background; 
    [SerializeField] private VideoPlayer videoPlayer;
    
    private int score = 0;

    private void Start()
    {
        StartCoroutine(CountdownCoroutine());
        StartCoroutine(DelayGameOver(audioSource.clip.length + 4f));
    }

    void Update()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            float progress = Math.Min(audioSource.time  / audioSource.clip.length,1f);
            progress = Mathf.Clamp01(progress);

            if (scroll != null)
            {
                scroll.GetComponent<Image>().fillAmount = progress;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameStop();
        }
    }

    public void UpdateScore(int points)
    {
        score = Mathf.Max(0,score + points);
        if(points>0&&scoreFeedback) scoreFeedback.PlayFeedbacks();
        if(scoreText)scoreText.text = score.ToString(); 
    }
    
    private IEnumerator CountdownCoroutine()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            countDown.text = countdown.ToString(); 
            yield return new WaitForSeconds(1f); 
            countdown--; 
        }

        countDown.text = "Start!!";
        yield return new WaitForSeconds(1f); 
        countDown.text = "";
    }
    
    private IEnumerator DelayGameOver(float Delay)
    {
        yield return new WaitForSeconds(Delay); 
        GameOver();
    }
    
    public void GameOver()
    {
        gameOver.SetActive(true);
        gameOver.transform.Find("Score").GetComponent<Text>().text = score.ToString();
        AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[6],0,1);

        // Time.timeScale = 0f;
    }
    
    public void GameStop()
    {
        if (gameStop.activeSelf)
        {
            gameStop.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            gameStop.SetActive(true);
            Time.timeScale = 0f;
        }
    }


    public void GameRestart()
    {
        SceneManager.LoadScene(1);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        background.raycastTarget = true;
        StartCoroutine(FadeBackgroundAndPlayVideo());
    }
    
    public void LeaveGame()
    {
        SceneController.Instance.TransitionTo(0);
    }
    
    private IEnumerator FadeBackgroundAndPlayVideo()
    {
        background.DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        videoPlayer.gameObject.SetActive(true);
        background.DOFade(0, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(5f);
        background.DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        SceneController.Instance.TransitionTo(0);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

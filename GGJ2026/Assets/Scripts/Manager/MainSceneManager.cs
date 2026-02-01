using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class MainSceneManager : MonoBehaviour
{
    public Image background; 
    public VideoPlayer videoPlayer;
    public GameObject image1;
    public GameObject image2;
    public GameObject button1;
    public AudioSource ass;

    private void Start()
    {
        StartCoroutine(ShowImagesAndButton());
    }

    public void GameStart()
    {
        background.raycastTarget = true;
        AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[5],0,1);
        StartCoroutine(FadeBackgroundAndPlayVideo());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameExit();
        }
    }
    
    private IEnumerator ShowImagesAndButton()
    {
        yield return new WaitForSeconds(5f);
        // 将 image1 的透明度从 0 变到 1
        image1.GetComponent<Image>().DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);

        // 将 image2 的透明度从 0 变到 1
        image2.GetComponent<Image>().DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);

        // 将 button 的透明度从 0 变到 1，并设置为可交互
        Button buttonComponent = button1.GetComponent<Button>();
        buttonComponent.interactable = false; // 先禁用交互
        button1.GetComponent<Image>().DOFade(1, 1f).SetEase(Ease.InOutQuad);
        buttonComponent.interactable = true; // 启用交互
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FadeBackgroundAndPlayVideo()
    {
        
        background.DOFade(1, 1f).SetEase(Ease.InOutQuad);
        ass.DOFade(0, 1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1f);
        videoPlayer.gameObject.SetActive(true);
        background.DOFade(0, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(14f);
        background.DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        SceneController.Instance.TransitionTo(1);
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
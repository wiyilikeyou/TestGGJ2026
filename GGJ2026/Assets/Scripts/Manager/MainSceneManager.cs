using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class MainSceneManager : MonoBehaviour
{
    public Image background; 
    public VideoPlayer videoPlayer;

    public void GameStart()
    {
        background.raycastTarget = true;
        StartCoroutine(FadeBackgroundAndPlayVideo());
    }

    private IEnumerator FadeBackgroundAndPlayVideo()
    {
        background.DOFade(1, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(1f);
        videoPlayer.gameObject.SetActive(true);
        background.DOFade(0, 1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(13f);
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
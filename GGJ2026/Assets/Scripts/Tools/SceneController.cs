using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public bool showVideo = false;
    public Animator Interval;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    Coroutine co = null;
    public void TransitionTo(int index)
    {
        if (index >= SceneManager.sceneCountInBuildSettings) index = 1;
        if(co != null)return;
        // Interval.gameObject.SetActive(false);
        co = StartCoroutine(Transition(index));
    }

    IEnumerator Transition(int index)
    {
        var operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;
        // Interval.gameObject.SetActive(index > 1);
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) break;
            yield return new  WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(0.5f);
        operation.allowSceneActivation = true;
        co = null;
    }   
    
}

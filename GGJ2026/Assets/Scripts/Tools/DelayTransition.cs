using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTransition : MonoBehaviour
{
    public float delay = 10;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayTrans());
    }

    IEnumerator DelayTrans()
    {
        yield return new WaitForSeconds(delay);
        SceneController.Instance.TransitionTo(0);
        yield return null;
    }
}

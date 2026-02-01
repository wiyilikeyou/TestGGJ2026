using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPanel : MonoBehaviour
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;

    void Start()
    {
        if (restartBtn && GameControl.Instance)
        {
            restartBtn.onClick.AddListener(GameControl.Instance.GameRestart);
            AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[5],0,1);

        }

        if (menuBtn && GameControl.Instance)
        {
            menuBtn.onClick.AddListener(GameControl.Instance.Menu);
            AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[5],0,1);
        }
    }
}

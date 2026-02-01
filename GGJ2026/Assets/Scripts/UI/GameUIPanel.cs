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
        if(restartBtn)restartBtn.onClick.AddListener(GameControl.Instance.GameRestart);
        if(menuBtn)menuBtn.onClick.AddListener(GameControl.Instance.Menu);
    }
}

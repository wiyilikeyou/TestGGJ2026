using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MainSceneManager : MonoBehaviour
{
    public Sprite GameContinue;
    public Button GameStartButton;
    [Button]
    public void SetLevel9()
    {
        PlayerPrefs.SetInt("Level", 11111);
    }
    private void Start() {
        AudioManager.Instance.PlayLoop(AudioManager.Instance.audioClips[0]);
        if(PlayerPrefs.HasKey("Level") == false||PlayerPrefs.GetInt("Level") == 1){
            PlayerPrefs.SetInt("Level", 1);
        }
        else{
            GameStartButton.GetComponent<Image>().sprite = GameContinue;
            //GameStartText.text = "继续游戏";
        }
    }

    public void LevelChoose()
    {
        SceneController.Instance.TransitionTo(1);
    }

    public void GameStart()
    {
        int level = PlayerPrefs.GetInt("Level");
        if (level == 1)
        {
            SceneController.Instance.TransitionTo(1);
            return;
        }
        SceneController.Instance.TransitionTo(level + 1);
    }

    public void LevelReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("Level", 1);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MusicTest : MonoBehaviour
{
    [SerializeField]
    private string trackName = "YourTrackName";

    public Text beatCount;
    public GameObject EnemyPrefab;
    public SimpleMusicPlayer musicPlayer;
    private int beat = 0;
    private GameObject player;
    public GameObject road;

    void Start()
    {
        // 确保 Koreographer 实例存在后再注册回调
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.RegisterForEvents(trackName, OnTrackEvent);
        }
        else
        {
            Debug.LogError("Koreographer instance is not available. Check if the plugin is correctly imported.");
        }
        
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(DelayPlay(3f));
    }

    private void OnTrackEvent(KoreographyEvent koreoEvent)
    {
    
        if (player != null)
        {
            // 获取 Player 的位置
            Vector3 spawnPosition = player.transform.position;
        
            // 设置 x 坐标为 -3.5 到 3.5 之间的随机值
            spawnPosition.x = Random.Range(-3.5f, 3.5f);
        
            // 在指定位置生成 EnemyPrefab
            GameObject enemy = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);
            
            enemy.transform.SetParent(road.transform);
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found.");
        }

        // 更新 beatCount 显示
        beatCount.text = beat.ToString();
        beat++;
    }

    public IEnumerator DelayPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicPlayer.Play();
    }


    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForEvents(trackName, OnTrackEvent);
        }
    }
}
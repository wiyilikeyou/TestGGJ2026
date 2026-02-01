using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ib_Core;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public enum ETrackEventType
{
    UFO = 0,
    LaserLine = 1,
    LaserCircle = 2,
}
[System.Serializable]
public struct TrackEventBox 
{
    public string trackName;
    public ETrackEventType eventType;
}
public class MusicManager : MonoBehaviour
{
    [SerializeField]private List<TrackEventBox> trackEventBoxes = new List<TrackEventBox>();
    // public Text beatCount;
    public SimpleMusicPlayer musicPlayer;
    private int beat = 0;
    private GameObject player;
    public Transform summonPos;
    void Start()
    {
        // 确保 Koreographer 实例存在后再注册回调
        if (Koreographer.Instance != null)
        {
            for (int i = 0; i < trackEventBoxes.Count; i++)
            {
                switch (trackEventBoxes[i].eventType)
                {
                    case ETrackEventType.UFO:
                        Koreographer.Instance.RegisterForEvents(trackEventBoxes[i].trackName, OnTrackEvent_UFO);
                        break;
                    case ETrackEventType.LaserLine:
                        Koreographer.Instance.RegisterForEvents(trackEventBoxes[i].trackName, OnTrackEvent_LaserLine);
                        break;
                    case ETrackEventType.LaserCircle:
                        Koreographer.Instance.RegisterForEvents(trackEventBoxes[i].trackName, OnTrackEvent_LaserCircle);
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("Koreographer instance is not available. Check if the plugin is correctly imported.");
        }
        
        player = GameObject.FindGameObjectWithTag("Player");

        Ib_Async.DelayDoSomething(3, musicPlayer.Play,this.GetCancellationTokenOnDestroy());
    }

    private void OnTrackEvent(KoreographyEvent koreoEvent)
    {
    
        if (player != null)
        {
            // 获取 Player 的位置
            Vector3 spawnPosition = player.transform.position;
        
            // 设置 x 坐标为 -3.5 到 3.5 之间的随机值
            spawnPosition.x = Random.Range(-3.2f, 3.2f);
            spawnPosition.y = 5.75f;
            spawnPosition.z = player.transform.position.z + 30f;
        
            // 在指定位置生成 EnemyPrefab
            UFOCreator.Instance?.SummonUFO(EUFOType.NormalUFO, Dir.Forward, summonPos.position + Vector3.right * Random.Range(-2f, 2f) + Vector3.up * 5.75f);
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found.");
        }
    }

    private void OnTrackEvent_UFO(KoreographyEvent koreoEvent)
    {
        UFOCreator.Instance?.SummonUFO(EUFOType.NormalUFO, Dir.Forward, summonPos.position + Vector3.right * Random.Range(-1.5f, 1.5f) + Vector3.up * 4.5f);
    }

    private void OnTrackEvent_LaserLine(KoreographyEvent koreoEvent)
    {
        UFOCreator.Instance?.SummonUFO(EUFOType.LaserUFO, Dir.Forward, player?.transform.position ?? Vector3.zero + MathUtils.GetRandomPosOffset(2.5f));
    }
    private void OnTrackEvent_LaserCircle(KoreographyEvent koreoEvent)
    {
        UFOCreator.Instance?.SummonUFO(EUFOType.LaserCircle, Dir.Forward, player?.transform.position ?? Vector3.zero + MathUtils.GetRandomPosOffset(4f));
    }
    // public IEnumerator DelayPlay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     musicPlayer.Play();
    // }


    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            foreach (var box in trackEventBoxes)
            {
                switch (box.eventType)
                {
                    case ETrackEventType.UFO:
                        Koreographer.Instance.UnregisterForEvents(box.trackName, OnTrackEvent_UFO);
                        break;
                    case ETrackEventType.LaserLine:
                        Koreographer.Instance.UnregisterForEvents(box.trackName, OnTrackEvent_LaserLine);
                        break;
                    case ETrackEventType.LaserCircle:
                        Koreographer.Instance.UnregisterForEvents(box.trackName, OnTrackEvent_LaserCircle);
                        break;
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ENextDir
{
    Forward = 0,
    Right = 1,
    Left = 2,
}
[System.Serializable]
public struct RoadConfig
{
    public GameObject roadPrefab;
    public float roadLength;
    public float roadCreateWeight;
    public string newRoadPointName;
    public ENextDir nextDir;
}
[System.Serializable]
public class RoadBox
{
    public GameObject roadPrefab;
    public float roadLength;
    public Transform nextRoadTransform;
    public Dir dir;
    public float travelDistance;
}
public enum Dir
{
    Forward = 0,
    Right = 1,
    Backward = 2,
    Left = 3,
}
public class RoadCreator : MonoBehaviour
{
    [BoxGroup("基本参数")][Title("移动速度")][SerializeField] public float speed = 10f;
    
    [SerializeField] private List<RoadConfig> roadConfigs = new List<RoadConfig>();
    private List<RoadBox> roadQueue = new List<RoadBox>();

    private List<Vector3> roadDir = new List<Vector3>()
    {
        Vector3.back, Vector3.left, Vector3.forward,Vector3.right,
    };

    private ENextDir? changeDirSignal = null;
    void OnEnable()
    {
        OnChangeWorldDir.Register(ChangeWorldDirHandler);
    }

    private void OnDisable()
    {
        OnChangeWorldDir.Deregister(ChangeWorldDirHandler);
    }

    private void SetChangeDirSignal(ENextDir nextDir) => changeDirSignal = nextDir;
    private void ChangeWorldDirHandler(ENextDir nextDir,Vector3 pos)
    {
        switch (nextDir)
        {
            case ENextDir.Forward:
                break;
            case ENextDir.Right:
                ToRight();
                OnRotatePlayerHub.Invoke(currentDir,pos);
                break;
            case ENextDir.Left:
                ToLeft();
                OnRotatePlayerHub.Invoke(currentDir,pos);
                break;
        }
    }
    public Vector3 GetRoadDir(Dir dir)
    {
        return roadDir[(int)dir];
    }
    
    private Dir currentDir = Dir.Forward;

    
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        foreach(var item in roadQueue)  
            item.roadPrefab.transform.position += GetRoadDir(currentDir) * moveDistance;
        FillRoad(2);
        var roadBox = roadQueue[0];
        roadBox.travelDistance += moveDistance;
        if(roadBox.travelDistance >= roadBox.roadLength)
        {
            var obj = roadQueue[0];
            roadQueue.RemoveAt(0);
            Destroy(obj.roadPrefab);
        }   
    }
    
    public void ToRight()
    {
        currentDir =(Dir)(((int)currentDir + 1) % 4);
    }
    public void ToLeft()
    {
        currentDir =(Dir)(((int)currentDir + 3) % 4);
    }

    public RoadBox CreateRoadBox(RoadConfig roadConfig,Vector3 pos, Quaternion rot,Dir lastDir)
    {
        var roadBox = new RoadBox();
        roadBox.roadPrefab = Instantiate(roadConfig.roadPrefab, pos, rot);
        roadBox.nextRoadTransform = roadBox.roadPrefab.transform.Find(roadConfig.newRoadPointName);
        switch (roadConfig.nextDir)
        {
            case ENextDir.Forward:
                roadBox.dir = lastDir;
                break;
            case ENextDir.Right:
                roadBox.dir = (Dir)(((int)lastDir + 1) % 4);
                break;
            case ENextDir.Left:
                roadBox.dir = (Dir)(((int)lastDir + 3) % 4);
                break;
        }
        roadBox.travelDistance = 0;
        roadBox.roadLength = roadConfig.roadLength;
        switch (lastDir)
        {
            case Dir.Forward:
                break;
            case Dir.Right:
                roadBox.roadPrefab.transform.rotation *= Quaternion.Euler(0, 90, 0);
                break;
            case Dir.Backward:
                roadBox.roadPrefab.transform.rotation *= Quaternion.Euler(0, 180, 0);
                break;
            case Dir.Left:
                roadBox.roadPrefab.transform.rotation *= Quaternion.Euler(0, 270, 0);
                break;
        }
        roadBox.roadPrefab.transform.position = pos;
        return roadBox;
    }

    private RoadConfig GetRandomRoadConfig()
    {
        return roadConfigs[Random.Range(0, roadConfigs.Count)];
    }
    private void FillRoad(int maxCnt)
    {
        if (roadQueue.Count == 0)
        {
            roadQueue.Add(CreateRoadBox(roadConfigs[0],transform.position,Quaternion.identity, Dir.Forward));
        }
        else if (roadQueue.Count < maxCnt)
        {
            roadQueue.Add(CreateRoadBox(GetRandomRoadConfig(),roadQueue[^1].nextRoadTransform.position,Quaternion.identity,roadQueue[^1].dir ));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UFOBox
{
    public GameObject obj;
    public float summonTime;
}

public enum EUFOType
{
    NormalUFO = 0,
    EnemyUFO = 1,
    LaserUFO = 2,
}
public class UFOCreator : Singleton<UFOCreator>
{
    [SerializeField] private GameObject ufoPrefab;
    [SerializeField] private GameObject ememyUFOPrefab;
    [SerializeField] private GameObject laserUFOPrefab;
    private Dir summonDir;
    
    public UFOBox SummonUFO(EUFOType ufoType,Dir dir,Vector3 pos)
    {
        summonDir = dir;
        UFOBox box = new UFOBox();
        switch (ufoType)
        {
            case EUFOType.NormalUFO:
                box.obj = Instantiate(ufoPrefab, pos, Quaternion.identity);
                if(box.obj.TryGetComponent<UFOController>(out UFOController ufoController))
                {
                    ufoController.speed = Vector3.back * 10;
                    ufoController.ChangeColor();
                }
                break;
            case EUFOType.EnemyUFO:
                box.obj = Instantiate(ememyUFOPrefab, pos, Quaternion.identity);
                if(box.obj.TryGetComponent<EnemyUFOController>(out EnemyUFOController controller))
                {
                    controller.speed = Vector3.back * 10;
                }
                break;
            case EUFOType.LaserUFO:
                box.obj = Instantiate(laserUFOPrefab, pos , Quaternion.identity);
                box.obj.transform.position = FindObjectOfType<PlayerController>()?.transform.position??Vector3.zero;
                break;
            default:
                box.obj = Instantiate(ufoPrefab, pos, Quaternion.identity);
                break;
        }
        box.summonTime = Time.time;
        return box;
    }
}

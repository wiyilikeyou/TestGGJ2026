using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum EAlianSummonType
{
    Test = 0,
    
}
public class AlianCreator : Singleton<AlianCreator>
{
    [BoxGroup("外星人")] [SerializeField] private GameObject testAlianPrefab;
    [BoxGroup("撞击特效")] [SerializeField] public List<GameObject> effectPrefabs;

    public GameObject SummonAlian(EAlianSummonType alianSummonType,int cnt,Vector3 position,Quaternion rotation,float maxRadius,Transform parent)
    {
        switch (alianSummonType)
        {
            case EAlianSummonType.Test:
                if(testAlianPrefab == null)break;
                for (int i = 0; i < cnt; i++)
                {
                    var targetPos = position + MathUtils.GetRandomPosOffset(maxRadius);
                    return Instantiate(testAlianPrefab, targetPos, rotation,parent);
                }
                break;
        }
        return null;
    }
}
public class MathUtils : MonoBehaviour
{
    public static Vector3 GetRandomPosOffset( float radius)
    {
        Vector2 random2D = Random.insideUnitCircle * radius;
        return new Vector3(random2D.x, 0, random2D.y);
    }
}
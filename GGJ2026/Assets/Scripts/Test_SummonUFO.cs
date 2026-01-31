using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SummonUFO : MonoBehaviour
{
    public Transform summonPos;

    private float timer = 0;

    [SerializeField]private float interval = 2;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            // interval = UnityEngine.Random.Range(2, 3f);
            int enmeyType = UnityEngine.Random.Range(0, 3);
            UFOCreator.Instance?.SummonUFO((EUFOType)enmeyType, Dir.Forward, summonPos.position + Vector3.right * Random.Range(-2f, 2f) + Vector3.up * 5.75f);
        }
    }
}

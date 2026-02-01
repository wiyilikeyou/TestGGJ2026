using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public float forceMagnitude = 10f; // 力度可调参数

    private Rigidbody rb;
    private int effectIndex = 0;

    void Start()
    {
        // 获取刚体组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("未找到 Rigidbody 组件！");
        }

        if (AlianCreator.Instance)
        {
            effectIndex = AlianCreator.Instance.effectPrefabs.Count;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 0, -1f).normalized;
            Vector3 backwardForce = -randomDirection * forceMagnitude;
            rb.AddForce(backwardForce, ForceMode.Impulse);
            
            if (AlianCreator.Instance)
            {
                int randomValue = Random.Range(0, effectIndex);
                GameObject effect = Instantiate(AlianCreator.Instance.effectPrefabs[randomValue], transform.Find("EffectInstantiatePos").position, Quaternion.identity);
            }
        }
    }
}
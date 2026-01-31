using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


public class EnemyUFOController : MonoBehaviour
{
    private bool isInteracted = false;

    [SerializeField] public Vector3 speed;
    private float summonTime = -1;
    [FormerlySerializedAs("checkHitBox")] [SerializeField] private CheckHitRadius checkHitRadius;

    private void Start()
    {
        summonTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (!isInteracted && checkHitRadius.Check(out var hitBox))
        {
            foreach (var item in hitBox)
            {
                if (item.transform.CompareTag("Player"))
                {
                    isInteracted = true;
                    ShowTestUIInfo.Invoke("Hurt!");
                }
            }
        }
        transform.position += speed * Time.deltaTime;
        if (Time.time - summonTime >= 10)
        {
            Destroy(gameObject);
        }
    }
}

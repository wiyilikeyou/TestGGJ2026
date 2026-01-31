using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

public class UFOController : MonoBehaviour
{
    [SerializeField] private List<Light> lights = new List<Light>();
    [SerializeField] private Color initialColor;
    [SerializeField] private float initialIntensity = 1f;
    [SerializeField] private Color targetColor;
    [SerializeField] private float targetIntensity = 1f;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float activeDuration = 1f;

    [SerializeField] private Transform alianSummonPos;
    [SerializeField] private int alianSummonCnt = 3;
    [SerializeField] private float alianSummonRadius = 2f;
    private bool isActive = false;
    private bool isInteracted = false;
    private float activeTime = -1f;
    public bool IsActive => isActive;
    private bool changedColor = false;
    private float summonTime = -1f;
    public Vector3 speed;
    
    private List<GameObject> alians = new List<GameObject>();
    private void Start()
    {
        foreach (var light in lights)
        {
            if (light)
            {
                light.color = initialColor;
                light.intensity = initialIntensity;
            }
        }
        isActive = false;
        summonTime = Time.time;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime;
        if(!isActive)return;
        if (Time.time - activeTime >= activeDuration)
        {
            isActive = false;
            RestoreColor();
        }
        if (Time.time - summonTime >= 10)
        {
            Destroy(gameObject);
        }
    }
    [Button]
    public void ChangeColor()
    {
        if(changedColor)return;
        changedColor = true;
        bool init = false;
        foreach (var light in lights)
        {
            light.DOColor(targetColor, fadeDuration);
            DOTween.To(() => light.intensity, x => light.intensity = x, targetIntensity, fadeDuration)
                .SetEase(Ease.Linear) // 【关键】设置为线性匀速
                .OnUpdate(() =>
                {
             
                })
                .OnComplete(() => 
                {
                    if (!init)
                    {
                        isActive = true; 
                        activeTime= Time.time;
                        SummonAlian();
                        init = true;
                    }
                    
                });
        }
        
    }
    
    private void RestoreColor()
    {
        bool init = false;
        foreach (var light in lights)
        {
            light.DOColor(initialColor, 1);
            DOTween.To(() => light.intensity, x => light.intensity = x, initialIntensity, 1)
                .SetEase(Ease.Linear) // 【关键】设置为线性匀速
                .OnUpdate(() =>
                {
             
                })
                .OnComplete(() => 
                {
                    if(!init)
                    {
                        Destroy(gameObject,2);
                        init = true;
                    }
                });
        }
       
    }
    private void SummonAlian()
    {
        var obj = AlianCreator.Instance?.SummonAlian(EAlianSummonType.Test,UnityEngine.Random.Range(1,3),alianSummonPos.position,Quaternion.identity,alianSummonRadius,alianSummonPos);
        if(obj != null)alians.Add(obj);
    }

    public bool Interact()
    {
        if(!isActive || isInteracted)return false;
        isInteracted = true;
        for (int i = 0; i < alians.Count; i++)
        {
            alians[i].transform.SetParent(null);
            var rb= alians[i].AddComponent<Rigidbody>();
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-5f,5f),UnityEngine.Random.Range(3,5f),UnityEngine.Random.Range(-5f,5f)) * 5,ForceMode.Impulse);
            Destroy(alians[i].gameObject,5);
        }
        RestoreColor();
        return true;
    }
}

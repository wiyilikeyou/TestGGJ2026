using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    [SerializeField] private Light light;
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
    private void Start()
    {
        if(light)
        {
            light.color = initialColor;
            light.intensity = initialIntensity;
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
        if(!light)return;
        if(changedColor)return;
        changedColor = true;
        light.DOColor(targetColor, fadeDuration);
        DOTween.To(() => light.intensity, x => light.intensity = x, targetIntensity, fadeDuration)
            .SetEase(Ease.Linear) // 【关键】设置为线性匀速
            .OnUpdate(() =>
            {
             
            })
            .OnComplete(() => 
            {
                isActive = true; 
                activeTime= Time.time;
                SummonAlian();
            });
    }
    
    private void RestoreColor()
    {
        if(!light)return;
        light.DOColor(initialColor, 1);
        DOTween.To(() => light.intensity, x => light.intensity = x, initialIntensity, 1)
            .SetEase(Ease.Linear) // 【关键】设置为线性匀速
            .OnUpdate(() =>
            {
             
            })
            .OnComplete(() => 
            {
                Destroy(gameObject,2);
            });
    }
    private void SummonAlian()
    {
        AlianCreator.Instance?.SummonAlian(EAlianSummonType.Test,3,alianSummonPos.position,Quaternion.identity,alianSummonRadius,alianSummonPos);
    }

    public bool Interact()
    {
        if(!isActive || isInteracted)return false;
        isInteracted = true;
        RestoreColor();
        return true;
    }
}

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

    [Header("出场效果")]
    [SerializeField] private float spawnHeight = 20f; // 生成高度
    [SerializeField] private float descendDuration = 2f; // 下降时间
    [SerializeField] private float spawnRadius = 10f; // 随机生成半径

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

        // 实现从天上飞下来的效果
        SpawnFromSky();
    }

    private void SpawnFromSky()
    {
        Vector3 targetLocalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(targetLocalScale, 1);
        // 保存目标位置
        Vector3 targetPosition = transform.position;

        // 随机一个水平方向的偏移
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spawnRadius;

        // 设置初始位置：在目标位置上方 + 随机水平偏移
        Vector3 startPosition = targetPosition + new Vector3(randomOffset.x, spawnHeight, randomOffset.y) + Vector3.forward * 30;
        transform.position = startPosition;

        AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[1],0,1);
        // 使用 DOTween 平滑下降到目标位置
        transform.DOMove(targetPosition, descendDuration)
            .SetEase(Ease.OutQuad) // 使用缓出效果，更自然
            .OnComplete(() =>
            {
                // 下降完成后的回调（可选）
                ChangeColor();
            });
    }

    private void Update()
    {
        if(leaving)speed = new Vector3(speed.x*0.2f,3,speed.y*0.2f);
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
    List<Tween> tweens = new List<Tween>();
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
            var color = new Color(initialColor.r, initialColor.g, initialColor.b,0);
            light.DOColor(color, 0.5f);
            DOTween.To(() => light.intensity, x => light.intensity = x, 0, 1)
                .SetEase(Ease.Linear) // 【关键】设置为线性匀速
                .OnUpdate(() =>
                {
             
                })
                .OnComplete(() => 
                {
                    if(!init)
                    {
                        for (int i = 0; i < alians.Count; i++)
                        {
                            Destroy(alians[i].gameObject);
                        }
                        alians.Clear();
                        Destroy(gameObject,2);
                        init = true;
                    }
                });
        }
       
    }
    private void SummonAlian()
    {
        var obj = AlianCreator.Instance?.SummonAlian(EAlianSummonType.Test,UnityEngine.Random.Range(1,3),alianSummonPos.position,Quaternion.identity,alianSummonRadius,alianSummonPos);
        if (obj != null)
        {
            obj.transform.localPosition += Vector3.up * 4;
            obj.transform.localScale = new Vector3(0,1,0);
            obj.transform.DOLocalMoveY(obj.transform.localPosition.y -4, 0.25f).SetEase(Ease.OutQuad);
            obj.transform.DOScale(Vector3.one , 0.25f).SetEase(Ease.OutQuad);
            alians.Add(obj);
        }
    }
    
    bool leaving = false;
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
        transform.DOMove(transform.position + Vector3.up * 10, 1.5f);
        leaving = true;
        alians.Clear();
        RestoreColor();
        return true;
    }
}

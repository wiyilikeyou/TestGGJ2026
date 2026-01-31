using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Swap : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f; // 移动速度
    public GameObject childGameObject;    // 子图物体（背景）
    private Transform childWorldTransform;
    private Vector3 childWorldPosition;
    private Vector3 childWorldScale;
    private int layerCount = -1;

    private void Start()
    {
        childWorldTransform = childGameObject.GetComponent<Transform>();
        childWorldPosition = childWorldTransform.position;
        childWorldScale = childWorldTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    { 
        // 获取 WASD 输入
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D 或 左/右箭头
        float verticalInput = Input.GetAxis("Vertical");     // W/S 或 上/下箭头

        // 计算移动向量
        Vector3 movement = new Vector3(horizontalInput, verticalInput,0) * moveSpeed * Time.deltaTime;

        // 更新物体位置
        transform.Translate(movement);

        // 恢复子物体的世界坐标
        childWorldTransform.position = childWorldPosition;
    }
    
    [Button("Start Expand")] // 添加 Odin 按钮
    public void StartExpand()
    {
        StartCoroutine(ExpandCoroutine());
    }

    IEnumerator ExpandCoroutine()
    {
        Vector3 targetScale = new Vector3(4,4,4);
        float elapsedTime = 0f;
        float duration = 2f; // 持续时间2秒

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 插值因子
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
            // 计算父物体当前的缩放比例
            Vector3 parentScale = transform.localScale;

            // 动态调整子物体的 localScale，使其世界缩放保持不变
            childWorldTransform.localScale = new Vector3(
                childWorldScale.x / parentScale.x,
                childWorldScale.y / parentScale.y,
                childWorldScale.z / parentScale.z
            );
            yield return null; // 等待下一帧
        }
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = layerCount; // 设置为最低层级
            layerCount--;
        }
    }
}
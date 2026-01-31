using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ib_Core;
using UnityEngine;
public class OnRotatePlayerHub : Ib_Event<OnRotatePlayerHub, Dir,Vector3> { }
public class RotatePlayerHub : MonoBehaviour
{
    [SerializeField] private Transform playerHub;
    private void OnEnable()
    {
        OnRotatePlayerHub.Register(ChangeDirHandler);
    }

    private void OnDisable()
    {
        OnRotatePlayerHub.Deregister(ChangeDirHandler);
    }
    Tween tween;
    private void ChangeDirHandler(Dir dir,Vector3 pos)
    {
        if (playerHub)
        {
            tween.Kill();

            // 计算旋转角度
            float targetAngle = (int)dir * 90f;
            float currentAngle = playerHub.transform.eulerAngles.y;
            float angleDiff = targetAngle - currentAngle;

            // 标准化角度差到 -180 到 180 范围
            while (angleDiff > 180f) angleDiff -= 360f;
            while (angleDiff < -180f) angleDiff += 360f;

            // 计算绕 pos 旋转
            Vector3 offset = playerHub.transform.position - pos;
            Vector3 rotatedOffset = Quaternion.Euler(0, angleDiff, 0) * offset;
            Vector3 targetPosition = pos + rotatedOffset;

            // 同时旋转和移动
            tween = DOTween.Sequence()
                .Join(playerHub.transform.DORotateQuaternion(Quaternion.Euler(0, targetAngle, 0), 0.5f))
                .Join(playerHub.transform.DOMove(targetPosition, 0.5f));
        }
    }
}

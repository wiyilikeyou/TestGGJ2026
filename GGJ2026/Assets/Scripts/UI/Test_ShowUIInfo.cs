using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Test_ShowUIInfo : MonoBehaviour
{
   [SerializeField]private TextMeshProUGUI textMeshProUGUI;

   private void OnEnable()
   {
      ShowTestUIInfo.Register(OnShowTestUIInfo);
   }

   private void OnDisable()
   {
      ShowTestUIInfo.Deregister(OnShowTestUIInfo);
   }

   Tween tween;
   private void OnShowTestUIInfo(string info)
   {
      if(!textMeshProUGUI)return;
      tween?.Kill();
      textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g,textMeshProUGUI.color.b,0);
      textMeshProUGUI.text = info;
      tween = textMeshProUGUI.DOFade(1f, 0.66f);
   }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ib_Core;
using UnityEngine;
using UnityEngine.Serialization;

public class Laser : MonoBehaviour
{
   public GameObject laserTips;
   public GameObject laserBox;
  public ICheckHit checkHitRadius;

   private float summonTimer;
   private bool isAttacked = false;
   private Material material;
   public Renderer rend;
   
   public Vector3 scale = new Vector3(15,1,1);
   public bool isCircle = false;
   private void Start()
   {
      summonTimer = Time.time;
      TryGetComponent(out checkHitRadius);
      if (rend)
      {
         material = rend.material;
      }
      if(laserTips) laserTips.transform.localScale = new Vector3(
         isCircle ? 0 : laserTips.transform.localScale.x,
         laserTips.transform.localScale.y,
         0
      );
   }
   private float? attackTime = -1;
   private void Update()
   {
      if(Time.time - summonTimer >= 5)Destroy(gameObject);
      if (attackTime != null && Time.time - attackTime.Value <= 1f)
      {
         if (checkHitRadius.Check(out var list))
         {
            for (int i = 0; i < list.Count; i++)
            {
               if(list[i].transform.CompareTag("Player"))
               {
                  ShowTestUIInfo.Invoke("Hurt!");
                  if (list[i].TryGetComponent(out Rigidbody rb))
                  {
                     rb.AddForce(Vector3.up * 250, ForceMode.Impulse);
                  }
                  GameControl.Instance?.UpdateScore(-1000);
                  attackTime = null;
                  break;
               }
            }
         }
      }
      if(Time.time - summonTimer > 2.33f && !isAttacked)
      {
         isAttacked = true;
         laserBox.SetActive(true);
         laserBox.transform.localScale = Vector3.zero;
         laserBox.transform.DOScale(scale, 0.25f).OnComplete(() =>
         {
            attackTime = Time.time;
           
            Ib_Async.DelayDoSomething(1, () => laserBox.transform.DOScale(Vector3.zero, 0.1f),this.GetCancellationTokenOnDestroy());
         });
         laserTips.SetActive(false);
      }
      else if(Time.time - summonTimer < 2)
      {
         float scaleValue = (Time.time - summonTimer) / 2f;
         laserTips.transform.localScale = new Vector3(
            isCircle ? scaleValue : laserTips.transform.localScale.x,
            laserTips.transform.localScale.y,
            scaleValue
         );
         material.SetColor("_MainColor", new Color(1f, 0.2f, 0.2f, scaleValue));
      }else if (Time.time - summonTimer < 2.33f)
      {
         material.SetColor("_MainColor", new Color(1f, 0.2f + ((Time.time - summonTimer) -2)*2.2f, 0.2f + ((Time.time - summonTimer) -2)*2.2f, 1));
      }
   }
}

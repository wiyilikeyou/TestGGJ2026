using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
   
   private void Update()
   {
      if(Time.time - summonTimer >= 5)Destroy(gameObject);
      if(Time.time - summonTimer > 2.33f && !isAttacked)
      {
         isAttacked = true;
         laserBox.SetActive(true);
         laserBox.transform.localScale = Vector3.zero;
         laserBox.transform.DOScale(scale, 0.25f).OnComplete(() =>
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
                     break;
                  }
               }
            }
            laserBox.transform.DOScale(Vector3.zero, 0.1f);
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

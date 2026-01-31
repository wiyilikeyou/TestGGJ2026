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
         
         laserTips.transform.localScale = new Vector3( isCircle ? (Time.time - summonTimer)/2f : laserTips.transform.localPosition.x, laserTips.transform.localPosition.y,  (Time.time - summonTimer)/2f);
         material.SetColor("_MainColor", new Color(1, 0, 0, (Time.time - summonTimer) / 2f));
      }else if (Time.time - summonTimer < 2.33f)
      {
         material.SetColor("_MainColor", new Color(1, ((Time.time - summonTimer) -2)*2.2f, ((Time.time - summonTimer) -2)*2.2f, 1));
      }
   }
}

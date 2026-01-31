using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckHit
{ 
      public bool Check(out List<Collider> hitBox);
}
public class CheckHitRadius : MonoBehaviour,ICheckHit
{
      public Vector3 hitBoxOffset;
      public float radius = 1f;
      public LayerMask layerMask;

      Collider[] buffer = new Collider[10];

      public bool Check(out List<Collider> hitBox)
      {
            hitBox = null;
            var cnt = Physics.OverlapSphereNonAlloc(transform.position + hitBoxOffset, radius, buffer, layerMask);
            if (cnt > 0)
            {
                  hitBox = new List<Collider>();
                  for (int i = 0; i < cnt; i++)
                  {
                        hitBox.Add(buffer[i]);
                  }
            }
            return cnt > 0;
      }
      public void OnDrawGizmos()
      {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + hitBoxOffset, radius);
      }
}

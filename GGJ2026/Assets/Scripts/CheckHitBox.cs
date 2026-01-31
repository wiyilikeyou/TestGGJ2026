using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox : MonoBehaviour,ICheckHit
{
    public Vector3 hitBoxOffset;
    public Vector3 size = Vector3.one;
    public LayerMask layerMask;

    Collider[] buffer = new Collider[10];

    public bool Check(out List<Collider> hitBox)
    {
        hitBox = null;
        var cnt = Physics.OverlapBoxNonAlloc(transform.position + hitBoxOffset, size, buffer, Quaternion.identity, layerMask);
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
        Gizmos.DrawWireCube(transform.position + hitBoxOffset, size);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Ib_Core;
using UnityEngine;

public class OnChangeWorldDir : Ib_Event<OnChangeWorldDir, ENextDir,Vector3> { }

public class ChangeWorldDir : MonoBehaviour
{
    public ENextDir nextDir = ENextDir.Forward;
    private bool isActive = false;
    private void OnTriggerEnter(Collider other)
    {
        if(isActive)return;
        if (other.CompareTag("Player"))
        {
            isActive = true;
            OnChangeWorldDir.Invoke(nextDir,transform.position);
        }
    }
}

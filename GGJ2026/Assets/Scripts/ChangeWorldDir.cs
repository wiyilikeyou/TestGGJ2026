using System;
using System.Collections;
using System.Collections.Generic;
using Ib_Core;
using UnityEngine;

public class OnChangeWorldDir : Ib_Event<OnChangeWorldDir, ENextDir> { }

public class ChangeWorldDir : MonoBehaviour
{
    public ENextDir nextDir = ENextDir.Forward;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnChangeWorldDir.Invoke(nextDir);
        }
    }
}

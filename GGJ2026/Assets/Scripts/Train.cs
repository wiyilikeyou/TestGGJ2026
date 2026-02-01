using System;
using System.Collections;
using System.Collections.Generic;
using Ib_Core;
using UnityEngine;

public class Train : MonoBehaviour
{
    bool isActive = false;
    public  Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive && other.gameObject.CompareTag("Player"))
        {
            Ib_Log.Info("Player Enter");
            isActive = true;
            animator?.SetTrigger("Active");
        }
    }
}

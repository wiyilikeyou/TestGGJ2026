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
            AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[2],0,1);

            Ib_Log.Info("Player Enter");
            isActive = true;
            animator?.SetTrigger("Active");
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveUFO : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out UFOController ufoController))
            ufoController.ChangeColor();
    }
}

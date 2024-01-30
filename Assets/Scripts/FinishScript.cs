using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    public StartScript timerOn;
    private void OnTriggerEnter(Collider other)
    {
        timerOn.timerOn = false;
    }
}

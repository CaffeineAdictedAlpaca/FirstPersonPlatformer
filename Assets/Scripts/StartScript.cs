using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartScript : MonoBehaviour
{
    public float timer;
    public bool timerOn;
    public TextMeshPro timeText;
    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            timer += Time.deltaTime;
        }
        timeText.text = timer.ToString("F2");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timerOn = true;
        }
    }
}

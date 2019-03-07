using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    GameObject finishedPrep = null;

    [SerializeField]
    float timeToFinishPrep;

    [SerializeField]
    Text timer = null;
    float startTime;

    bool finished;

    Image green;

    public void Awake()
    {
        green = finishedPrep.GetComponent<Image>();
        finished = false;
        startTime = Time.time;
    }

    public void FixedUpdate()
    {
        if (!finished)
        {
            float guiTime = Time.time - startTime;
            timer.text = "Time: " + guiTime;
        }
        else
            green.enabled = true;
    }
    public void setFinish()
    {
        finished = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{

    float currentAmount = 0f;
    float maxAmount = 5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("z"))
        {

            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;

            else

                Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

    }
}

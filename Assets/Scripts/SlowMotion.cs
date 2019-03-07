using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("x"))
        {
            Time.timeScale = 2 * Time.timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        if (Input.GetKeyDown("z"))
        {
            Time.timeScale = 0.5f * Time.timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}

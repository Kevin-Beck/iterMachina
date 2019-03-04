using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField]
    GameObject br;
    Brain sc;

    // Use this for initialization
    void Start()
    {
        sc = br.GetComponent<Brain>();
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

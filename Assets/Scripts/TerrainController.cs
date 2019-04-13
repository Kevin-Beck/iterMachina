using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    GameData gd;
    // Start is called before the first frame update
    void Start()
    {
        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        gd.tc = gameObject.GetComponent<TerrainController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

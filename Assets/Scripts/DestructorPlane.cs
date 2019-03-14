using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructorPlane : MonoBehaviour
{
    PopulationController pc;
    // Start is called before the first frame update
    public void Awake()
    {
        
    }
    public void Start()
    {
        pc = GameObject.FindGameObjectWithTag("PopulationController").GetComponent<PopulationController>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        pc.DeconstructPopulation();
        CancelInvoke();
    }

}

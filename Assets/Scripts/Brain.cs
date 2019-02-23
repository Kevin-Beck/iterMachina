using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    Vector3 origin;

    [SerializeField]
    float instantiationRadius;

    [SerializeField]
    int numberOfDesiredNodes;

    [SerializeField]
    List<GameObject> nodes = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Construct()
    {
        for(int i = 0; i < numberOfDesiredNodes; i++)
        {

        }
    }
    private Vector3 calculatePositionForNode() { return origin; }
}

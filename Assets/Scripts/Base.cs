using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Rigidbody rb;
    FixedJoint fj;
    Renderer myRender;
    bool render;

    void Awake()
    {
        render = false;
        myRender = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        fj = GetComponent<FixedJoint>();
    }
    public void ConnectToNode(GameObject baseN)
    {
        if(baseN != null)
        {
            fj.connectedBody = baseN.GetComponent<Rigidbody>();
            rb.useGravity = true;
        }
        else
        {
            fj.connectedBody = null;
        }
    }
    public void ToggleRenderer()
    {
        render = !render;
        myRender.enabled = render;
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    FixedJoint fj;
    public Rigidbody rb;
    Renderer myRender;
    bool render;

    void Awake()
    {
        render = false;
        myRender = GetComponent<Renderer>();
        fj = GetComponent<FixedJoint>();
        rb = GetComponent<Rigidbody>();
    }
    public void ConnectToNode(GameObject edgeN)
    {
        if(edgeN != null)
        {
            fj.connectedBody = edgeN.GetComponent<Rigidbody>();
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

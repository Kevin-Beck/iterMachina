﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    FixedJoint fj;
    public Rigidbody rb;
    Transform t;
    Vector3 localPosition;
    Quaternion localRotation;
    // Start is called before the first frame update
    void Awake()
    {
        fj = GetComponent<FixedJoint>();
        t = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        localPosition = t.localPosition;
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
    public void ResetEdge()
    {

     //   fj.connectedBody = null;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localPosition = localPosition;

    }
}
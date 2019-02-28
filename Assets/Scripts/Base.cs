using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{

    float stringStrength;

    Rigidbody rb;
    HingeJoint hj;

    [SerializeField]
    SpringJoint sj1;

    [SerializeField]
    SpringJoint sj2;

    [SerializeField]
    SpringJoint sj3;

    [SerializeField]
    SpringJoint sj4;

    [SerializeField]
    FixedJoint fj;

    float spring;
    bool frozen;

    // Start is called before the first frame update
    void Awake()
    {
        spring = sj1.spring;
        rb = GetComponent<Rigidbody>();
        hj = GetComponent<HingeJoint>();
        fj = GetComponent<FixedJoint>();
    }
    public void ConnectToNode(GameObject baseN)
    {
        fj.connectedBody = baseN.GetComponent<Rigidbody>();
    }
    public void ResetBase()
    {
        sj1.spring = spring;
        sj2.spring = spring;
        sj3.spring = spring;
        sj4.spring = spring;

        fj.connectedBody = null;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    private Transform t;

    float stringStrength;

    public Rigidbody rb;
    HingeJoint hj;

    SpringJoint[] springsList;

    [SerializeField]
    FixedJoint fj;


    Vector3 savedLocalPosition;
    float spring;

    // Start is called before the first frame update
    void Awake()
    {
        t = GetComponent<Transform>();
        springsList = GetComponents<SpringJoint>();
        spring = springsList[0].spring;
        rb = GetComponent<Rigidbody>();
        hj = GetComponent<HingeJoint>();
        fj = GetComponent<FixedJoint>();
        savedLocalPosition = t.localPosition;
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
    public void ResetBase()
    {
     //   fj.connectedBody = null;
        foreach (SpringJoint sj in springsList)
        {
            sj.spring = spring;
        }
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        t.localPosition = savedLocalPosition;
        t.rotation = Quaternion.identity;

    }
}

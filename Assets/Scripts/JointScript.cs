using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointScript : MonoBehaviour
{
    [SerializeField]
    SpringJoint posMuscle;

    [SerializeField]
    SpringJoint negMuscle;

    [SerializeField]
    Renderer jointRender;

    [SerializeField]
    Base baseScript;

    [SerializeField]
    Edge edgeScript;

    [SerializeField]
    Bone boneScript;

    Vector3 homePosition;
    Quaternion homeRotation;

    float initSpring;
    // Start is called before the first frame update
    void Start()
    {
        initSpring = posMuscle.spring;
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    public void UpdateMuscles(float val)
    {
        posMuscle.spring = initSpring + val*initSpring;
        negMuscle.spring = initSpring - val*initSpring;
        UpdateJointRender(val);
        //rb.WakeUp();
    }

    private void UpdateJointRender(float col)
    {
        float x = (col + 1) / 2;
        jointRender.material.color = new Color(x,x,x);
    }
    public void SetBoneSize(float length)
    {
        boneScript.SetBoneLength(length);
    }
    public void ConnectBaseToNode(GameObject baseNode)
    {
        baseScript.ConnectToNode(baseNode);
    }
    public void ConnectEdgeToNode(GameObject edgeNode)
    {
        edgeScript.ConnectToNode(edgeNode);
    }
    public void Reset()
    {

        boneScript.ResetBone();
        baseScript.ResetBase();
        edgeScript.ResetEdge();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointScript : MonoBehaviour
{
    [SerializeField]
    private SpringJoint posMuscle = null;

    [SerializeField]
    private SpringJoint negMuscle = null;

    [SerializeField]
    private Renderer jointRender = null;

    [SerializeField]
    private Base baseScript = null;

    [SerializeField]
    private Edge edgeScript = null;

    [SerializeField]
    private Bone boneScript = null;

    private float A;
    private float B;
    private float C;

    private bool musclesOn;

    private float initSpring;
    // Start is called before the first frame update
    void Awake()
    {
        musclesOn = false;
        A = Random.Range(0f, 5f);
        B = Random.Range(0f, 3.2f);
        C = Random.Range(-1.8f, 1.8f);

        initSpring = posMuscle.spring;
    }
    public void FixedUpdate()
    {
        if(musclesOn)
        {
            UpdateMuscles(Mathf.Sin(A * Mathf.Cos(B * Time.fixedTime) + C));
        }
    }
    public void ToggleMuscle()
    {
        musclesOn = !musclesOn;
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
        Physics.SyncTransforms();
    }
}

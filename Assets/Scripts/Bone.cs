using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bone : MonoBehaviour
{
    private Transform myTransform;
    private Vector3 offSet;
    private Vector3 scale;
    public void Start()
    {
        myTransform = GetComponent<Transform>();
        offSet = myTransform.localPosition;
        scale = myTransform.localScale;
    }


    public void SetBoneLength(float length)
    {
        myTransform.localPosition = new Vector3(length, 0, 0) + offSet;
        myTransform.localScale = new Vector3(length * 2, 1, 1);
    }
    public void ResetBone()
    {
        myTransform.localScale = scale;
        myTransform.localPosition = offSet;
    }
}

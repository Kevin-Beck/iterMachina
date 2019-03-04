using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bone : MonoBehaviour
{
    private Transform myTransform;
    private Vector3 offSet;
    private Renderer myRender;

    bool render = false;

    void Awake()
    {
        myTransform = GetComponent<Transform>();
        offSet = myTransform.localPosition;
        myRender = GetComponent<Renderer>();
    }
    public void SetBoneLength(float length)
    {
        myTransform.localPosition = new Vector3(length, 0, 0)+offSet;
        myTransform.localScale = new Vector3(length * 2, 1, 1);
    }
    public void ToggleRenderer()
    {
        render = !render;
        myRender.enabled = render;
    }
}

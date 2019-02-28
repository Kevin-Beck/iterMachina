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

    float initSpring;
    // Start is called before the first frame update
    void Start()
    {
        initSpring = posMuscle.spring;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      
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
}

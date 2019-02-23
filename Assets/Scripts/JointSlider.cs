using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JointSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    [SerializeField]
    float A;
    [SerializeField]
    float B;
    [SerializeField]
    float C;
    
    [SerializeField]
    private GameObject GM;
    [SerializeField]
    private JointScript js;

    private int counter;
    // Start is called before the first frame update
    public void Start()
    {
        counter = 0;
        A = Random.Range(0f, 5f);
        B = Random.Range(0f, 3.2f);
        C = Random.Range(-1.8f, 1.8f);
        
        js = GM.GetComponent<JointScript>();
    }
    
    

    public void FixedUpdate()
    {
        float val = Mathf.Sin(A * Mathf.Cos(B * Time.fixedTime) + C);
        js.UpdateMuscles(val);
      
        //testJoint.GetComponent<Joint>().UpdateMuscles(slider.value);

        if(counter%5 == 0)
        {
            counter = 0;
            slider.value = val;
        }
    }
}

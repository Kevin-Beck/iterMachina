using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : MonoBehaviour
{
    bool selected;
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        selected = false;
    }
    private void Update()
    {
        if (selected)
            rend.material.color = Color.cyan;
        else
            rend.material.color = Color.blue;
    }
    public void Deselect()
    {
        selected = false;
    }
    public void Select()
    {
        selected = true;
    }
}

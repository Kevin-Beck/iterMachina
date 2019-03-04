using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    Renderer myRender;
    bool render;

    private void Awake()
    {
        render = false;
        myRender = GetComponent<Renderer>();
    }

    public void ToggleRenderer()
    {
        render = !render;
        myRender.enabled = render;
    }
}

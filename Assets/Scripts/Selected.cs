using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : MonoBehaviour
{
    Renderer rend;
    // TODO pretty sure this can be optimized a tad easier
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = Color.blue;
    }
    private void Update()
    {

    }
    public void Deselect()
    {
        rend.material.color = Color.blue;
    }
    public void Select()
    {
        rend.material.color = Color.cyan;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    Renderer myRender;
    bool render;

    Vector3 startingPosition;
    Vector3 endingPosition;

    private void Awake()
    {
        render = false;
        myRender = GetComponent<Renderer>();
    }

    public void SetStartPosition()
    {
        startingPosition = transform.position;
    }
    public void SetEndPosition()
    {
        endingPosition = transform.position;
    }
    public float GetCurrentScore()
    {
        float start = startingPosition.x;
        float end = endingPosition.x;
        float score = end - start;
        if (score < 0 || score > 100)
            score = 0;
        return score;
    }

    public void ToggleRenderer()
    {
        render = !render;
        myRender.enabled = render;
    }
}

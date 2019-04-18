using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCamera : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] RenderTexture render;
    [SerializeField] RectTransform rectTrans;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    public void MakeBigger()
    {
        rectTrans.anchoredPosition *= 2;
        RenderTexture bigRender = new RenderTexture(1200,1200, 24);
        cam.targetTexture = bigRender;
        rectTrans.sizeDelta = new Vector2(1200, 1200);
    }
    public void MakeSmaller()
    {
        cam.targetTexture = render;
        rectTrans.sizeDelta = new Vector2(600, 600);
        rectTrans.anchoredPosition /= 2;
    }
}

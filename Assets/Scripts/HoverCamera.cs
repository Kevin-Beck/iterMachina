using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCamera : MonoBehaviour
{
    [SerializeField] RectTransform rectTrans;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    public void MakeBigger()
    {
        rectTrans.anchoredPosition *= 2;
        rectTrans.sizeDelta = new Vector2(1200, 1200);
    }
    public void MakeSmaller()
    {
        rectTrans.sizeDelta = new Vector2(600, 600);
        rectTrans.anchoredPosition /= 2;
    }
}

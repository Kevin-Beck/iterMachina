using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoScript : MonoBehaviour
{
    BotEditorCameraTest bect;

    Vector3 lastFramePosition;
    bool firstFrame = true;
    private Camera cam;
    [SerializeField] bool xpos = false;
    [SerializeField] bool xneg = false;
    [SerializeField] bool ypos = false;
    [SerializeField] bool yneg = false;
    [SerializeField] bool zpos = false;
    [SerializeField] bool zneg = false;

    void Start()
    {
        cam = Camera.main;
        bect = GameObject.FindGameObjectWithTag("EditorController").GetComponent<BotEditorCameraTest>();      
    }

    private void OnMouseDrag()
    {
        if (firstFrame)
        {
            lastFramePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
            firstFrame = false;
        }
        else
        {
            Vector2 mousePos = new Vector2();
            Vector3 curFramePosition;
            mousePos.x = Input.mousePosition.x;
            mousePos.y = Input.mousePosition.y;
            curFramePosition = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

            Vector3 screenDifferenceVector = curFramePosition - lastFramePosition;

            Vector3 moveVector = new Vector3();
            if (xpos)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(1, 0, 0));
            else if (xneg)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(-1, 0, 0));
            else if (ypos)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(0, 1, 0));
            else if (yneg)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(0, -1, 0));
            else if (zpos)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(0, 0, 1));
            else if (zneg)
                moveVector = Vector3.Project(screenDifferenceVector, new Vector3(0, 0, -1));
            
            bect.MoveSelected(moveVector*30);
            lastFramePosition = curFramePosition;
        }
    }
}

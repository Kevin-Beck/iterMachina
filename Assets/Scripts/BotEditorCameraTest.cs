using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class BotEditorCameraTest : MonoBehaviour
{
    public FreeCameraLook cameraRig;
    public Camera cam;
    [SerializeField] GameObject GizmoPrefab = null;
    [SerializeField] GameObject NodeBuilderPrefab = null;
    [SerializeField] GameObject JointBuilderPrefab = null;

    GameData gd = null;
    List<GameObject> myGizmos = null;

    public List<GameObject> curSelected = null;

    [SerializeField] List<GameObject> nodes = null;

    List<BuilderTempInstruction> tempJoints = null;


    void Start()
    {
        cam = Camera.main;

        if (gd == null)
        {
            gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
            tempJoints = new List<BuilderTempInstruction>();
            nodes = new List<GameObject>();
            myGizmos = new List<GameObject>();
            curSelected = new List<GameObject>();
        }
        if(gd.editorDNA != null)
        {
            ConstructBotFromEditorDNA();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("e"))
            CreateEditorNodeAtCursorLocation();

        if (Input.GetKeyDown("c"))
            CopySelectedNodes();

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteCurSelectedNodes();
            curSelected.Clear();
        }
        if(Input.GetKeyDown("r"))
            if(curSelected.Count == 2)
                CheckAndCreateJoint(curSelected.ElementAt(0), curSelected.ElementAt(1));

        if (Input.GetMouseButtonDown(1))
            RightMouseButtonFunctions();

        if (Input.GetMouseButtonUp(1))
            cameraRig.enabled = false;

        if(Input.GetMouseButtonDown(0))
            LeftMouseButtonFunctions();

        if (Input.GetMouseButtonDown(2))
            CreateEditorNodeAtCursorLocation();

        if(Input.GetMouseButtonUp(0))
            ResetGizmos();
    }
    public void ConstructBotFromEditorDNA()
    {
        tempJoints = new List<BuilderTempInstruction>();
        nodes = new List<GameObject>();
        foreach (Vector3 pos in gd.editorDNA.nodePositions)
            nodes.Add(Instantiate(NodeBuilderPrefab, pos, Quaternion.identity));
        foreach (Instruction inst in gd.editorDNA.designInstructions)
        {
            GameObject newNode = nodes.ElementAt(inst.targetNode);
            GameObject oldNode = nodes.ElementAt(inst.baseNode);

            CheckAndCreateJoint(oldNode, newNode);
        }
    }
    private void DeleteCurSelectedNodes() // Deletes nodes (and connected joints)
    {
        foreach (GameObject go in curSelected)
        {
            DeleteJointsConnectedToNode(go);
            nodes.Remove(go);
            Destroy(go);
        }
    }
    private void DeleteJointsConnectedToNode(GameObject node)
    {
        int count = tempJoints.Count;
        for (int i = 0; i < count; i++)
        {
            BuilderTempInstruction bti = tempJoints.ElementAt(i);
            if (node == bti.baseNode || node == bti.tailNode)
            {
                Destroy(bti.joint);
                tempJoints.Remove(bti);
                i--;
                count = tempJoints.Count;
            }
            AdjustNodeReferenceInInstruction(bti, node);
        }
    }
    public void DeleteAllNodesAndJoints()
    {
        foreach (GameObject node in nodes)
            DeleteJointsConnectedToNode(node);

        foreach (GameObject node in nodes)
            Destroy(node);
        nodes.Clear();
        tempJoints.Clear();
    }
    private void AdjustNodeReferenceInInstruction(BuilderTempInstruction tempInst, GameObject nodeGO)
    {
        int indexOfNodeToRemove = nodes.IndexOf(nodeGO);
        int baseNodePosition = nodes.IndexOf(tempInst.baseNode);
        int tailNodePosition = nodes.IndexOf(tempInst.tailNode);

        if (baseNodePosition > indexOfNodeToRemove)
            tempInst.baseNode = nodes[baseNodePosition--];
        if (tailNodePosition > indexOfNodeToRemove)
            tempInst.tailNode = nodes[tailNodePosition--];
    }
    private bool CheckAndCreateJoint(GameObject oldNode, GameObject newNode)
    {
        bool good = true;

        Vector3 vector = (newNode.transform.position - oldNode.transform.position);
        if (vector.magnitude < 2.5f)
            good = false;

        Physics.SyncTransforms();
        Ray ray = new Ray(oldNode.transform.position + vector.normalized * 1.5f, vector.normalized);
        RaycastHit[] rch = Physics.SphereCastAll(ray, .5f, vector.magnitude - 3f, ~(1 << 9));

        if (rch.Length > 0)
            good = false;

        if (good)
        {
            GameObject newlyCreatedJoint = Instantiate(JointBuilderPrefab, oldNode.transform.position + vector.normalized * .9f, Quaternion.identity);
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);

            newlyCreatedJoint.GetComponentInChildren<Bone>().SetBoneLength(vector.magnitude - 2.5f);

            tempJoints.Add(new BuilderTempInstruction(oldNode, newNode, newlyCreatedJoint));
        }
        return good;
    }
    public void MoveSelected(Vector3 position)
    {
        foreach(GameObject go in curSelected)
        {
            Transform t = go.GetComponent<Transform>();
            Vector3 newPosition = position + t.transform.position;
            t.transform.position = ReturnValidPositionForNodeNearestPoint(newPosition);
        }
        foreach (GameObject go in curSelected)
        {
            DeleteJointsConnectedToNode(go);
        }
    }
    private void CopySelectedNodes()
    {
        int selectedCount = curSelected.Count();
        for(int i = 0; i < selectedCount; i++)
        {
            AddToSelected(CreateEditorNode(curSelected.ElementAt(0).transform.position));
            RemoveFromSelected(curSelected.ElementAt(0));
        }
    }
    public void RemoveFromSelected(GameObject toBeRemovedObject)
    {
        toBeRemovedObject.GetComponent<Selected>().Deselect();
        curSelected.Remove(toBeRemovedObject);
        ResetGizmos();
    }
    public void ResetGizmos()
    {
        foreach (GameObject go in myGizmos)
            Destroy(go);
        foreach (GameObject go in curSelected)
        {
            GameObject giz = Instantiate(GizmoPrefab, go.transform.position, Quaternion.identity);
            giz.transform.SetParent(go.transform, true);
            myGizmos.Add(giz);
        }
    }

    public Vector3 ReturnValidPositionForNodeNearestPoint(Vector3 newPosition)
    {
        // Ensures the piece stays in the boundaries
        newPosition.x = Mathf.Max(0, Mathf.Min(newPosition.x, gd.areaForInstantiation.x));
        newPosition.y = Mathf.Max(0.75f, Mathf.Min(newPosition.y, gd.areaForInstantiation.y));
        newPosition.z = Mathf.Max(0, Mathf.Min(newPosition.z, gd.areaForInstantiation.z));
        return newPosition;
    }
    public GameObject CreateEditorNode(Vector3 position)
    {
        GameObject myNode = Instantiate(NodeBuilderPrefab, position, Quaternion.identity);
        myNode.GetComponent<Renderer>().enabled = true;
        nodes.Add(myNode);
        return myNode;
    }
    public void CreateEditorNodeAtCursorLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        
        Vector3 position = ReturnValidPositionForNodeNearestPoint(hit.point);

        CreateEditorNode(position);
    }
    public void SaveBot()
    {
        RemoveExtraNodes();
        DNA savedDNA = new DNA();
        foreach(GameObject node in nodes)
            savedDNA.AddToPositions(node.transform.position);

        foreach(BuilderTempInstruction bti in tempJoints)
            savedDNA.AddToInstructions(new Instruction(nodes.IndexOf(bti.baseNode), nodes.IndexOf(bti.tailNode), Vector3.zero, 0));

        gd.editorDNA = savedDNA;

        /*
        File.Delete("C:\\iterBot\\DNA.txt");
        File.Create("C:\\iterBot\\");
        File.Create("C:\\iterBot\\DNA.txt");
        StreamWriter writer = new StreamWriter("C:\\iterBot\\DNA.txt", true);
        writer.WriteLine(savedDNA.toData());
        writer.Close();
        */
    }
    public void RemoveExtraNodes()
    {
        int numberOfNodes = nodes.Count;
        for(int i = 0; i < numberOfNodes; i++)
        {
            bool delete = true;
            GameObject nodeGO = nodes.ElementAt(i);
            foreach (BuilderTempInstruction bti in tempJoints)
            {
                if (bti.baseNode == nodeGO || bti.tailNode == nodeGO)
                {
                    delete = false;
                }
            }

            if (delete)
            {
                Debug.Log("Deleting node #" + i);
                /* TODO POP UP FOR ARE YOU SURE above this if statement
                delete = Warning("This will delete nodes not connected to other nodes!\n" +
                    "Are you sure you want to delete nodes not connected?\n");
                    */
                foreach (BuilderTempInstruction bti in tempJoints)
                    AdjustNodeReferenceInInstruction(bti, nodeGO);
                nodes.Remove(nodeGO);
                Destroy(nodeGO);
                numberOfNodes--;
                i--;
            }
        }           
    }
    private void DeselectAllSelectedItems()
    {
        foreach (GameObject go in curSelected)
            go.GetComponent<Selected>().Deselect();

        curSelected.Clear();

        RemoveAllGizmoGameObjects();
        myGizmos.Clear();
    }
    private void RemoveAllGizmoGameObjects()
    {
        foreach (GameObject go in myGizmos)
            Destroy(go);
    }
    private void RightMouseButtonFunctions()
    {
        cameraRig.enabled = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void AddToSelected(GameObject newSelected)
    {
        newSelected.GetComponent<Selected>().Select();
        curSelected.Add(newSelected);
        AddNewGizmo(newSelected.transform);
    }
    private void AddNewGizmo(Transform attached)
    {
        if (myGizmos.Count != curSelected.Count)
        {
            GameObject giz = Instantiate(GizmoPrefab, attached.position, Quaternion.identity);
            giz.transform.SetParent(attached, true);
            myGizmos.Add(giz);
        }
    }
    private void LeftMouseButtonFunctions()
    { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.gameObject.tag == "NodeBuilder")
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    DeselectAllSelectedItems();

                AddToSelected(hit.transform.gameObject);

                if (Input.GetKey(KeyCode.LeftShift))
                    if (curSelected.Count > 1)
                        CheckAndCreateJoint(curSelected.ElementAt(curSelected.Count-2), curSelected.ElementAt(curSelected.Count-1));
            }
            else if(hit.transform.gameObject.tag == "JointBuilder")
            {
                int count = tempJoints.Count;
                for(int i = 0; i < count; i++)
                {
                    if(tempJoints[i].joint == hit.transform.gameObject)
                    {
                        tempJoints.Remove(tempJoints[i--]);
                        count--;
                    }
                }

                Destroy(hit.transform.gameObject);
            }else if(hit.transform.gameObject.tag == "Arena")
                DeselectAllSelectedItems();
        }
        else
            DeselectAllSelectedItems();
    }
}

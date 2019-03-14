using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class BotEditorCameraTest : MonoBehaviour
{
    public FreeCameraLook cameraRig;

    [SerializeField] GameObject GizmoPrefab = null;
    [SerializeField] GameObject NodeBuilderPrefab = null;
    [SerializeField] GameObject JointBuilderPrefab = null;

    List<GameObject> myGizmos = null;

    [SerializeField] List<GameObject> curSelected = null;

    List<GameObject> nodes = null;

    List<BuilderTempInstruction> tempJoints = null;

    [SerializeField] GameObject InstructionCanvas = null;
    bool instructions = false;

    void Start()
    {
        tempJoints = new List<BuilderTempInstruction>();
        nodes = new List<GameObject>();
        myGizmos = new List<GameObject>();
        curSelected = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            GameObject newNode = Instantiate(NodeBuilderPrefab, new Vector3(7.5f, 2f, 7.5f), Quaternion.identity);
            newNode.GetComponent<Renderer>().enabled = true;
            nodes.Add(newNode);
        }
        if(Input.GetKeyDown("d"))
        {
            foreach (GameObject go in curSelected)
            {
                nodes.Remove(go);
                int count = tempJoints.Count;
                for(int i = 0; i < count; i++)
                {
                    BuilderTempInstruction bti = tempJoints.ElementAt(i);
                    if (go == bti.baseNode || go == bti.tailNode)
                    {
                        Destroy(bti.joint);
                        tempJoints.Remove(bti);
                        i--;
                        count = tempJoints.Count;
                    }
                }
                GameObject.Destroy(go);
            }
            curSelected.Clear();
        }
        if(Input.GetKeyDown("j"))
        {
            if(curSelected.Count == 2)
            {
                CheckAndCreateJoint(curSelected.ElementAt(0), curSelected.ElementAt(1));
            }
        }

        if (Input.GetMouseButtonDown(1))
            RightMouseButtonFunctions();

        if (Input.GetMouseButtonUp(1))
            cameraRig.enabled = false;

        if(Input.GetMouseButtonDown(0))
            LeftMouseButtonFunctions();
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
    private void MoveSelected(Vector3 position)
    {
        foreach(GameObject go in curSelected)
            go.GetComponent<Transform>().transform.position = position + go.GetComponent<Transform>().transform.position;

        foreach(GameObject go in myGizmos)
            go.GetComponent<Transform>().transform.position = position + go.GetComponent<Transform>().transform.position;

        foreach (GameObject go in curSelected)
        {
            int count = tempJoints.Count;
            for (int i = 0; i < count; i++)
            {
                BuilderTempInstruction bti = tempJoints.ElementAt(i);
                if (go == bti.baseNode || go == bti.tailNode)
                {
                    Destroy(bti.joint);
                    tempJoints.Remove(bti);
                    i--;
                    count = tempJoints.Count;
                }
            }
        }
    }
    public void ToggleInstructionPanel()
    {
        InstructionCanvas.SetActive(!instructions);
        instructions = !instructions;
    }
    public void ContinueToSim()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitProgram()
    {
        Application.Quit();
    }
    public void SaveBotToFile()
    {
        DNA savedDNA = new DNA();
        foreach(GameObject node in nodes)
            savedDNA.AddToPositions(node.transform.position);

        foreach(BuilderTempInstruction bti in tempJoints)
            savedDNA.AddToInstructions(new Instruction(nodes.IndexOf(bti.baseNode), nodes.IndexOf(bti.tailNode), Vector3.zero));
        Debug.Log(savedDNA.toData());

        File.Delete("Assets/Resources/test.txt");
        StreamWriter writer = new StreamWriter("Assets/Resources/test.txt", true);
        writer.WriteLine(savedDNA.toData());
        writer.Close();
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
    private void LeftMouseButtonFunctions()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.gameObject.tag == "NodeBuilder")
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    DeselectAllSelectedItems();
                hit.transform.gameObject.GetComponent<Selected>().Select();
                curSelected.Add(hit.transform.gameObject);

                if (myGizmos.Count < curSelected.Count)
                    myGizmos.Add(Instantiate(GizmoPrefab, hit.transform.position, Quaternion.identity));
            }
            else if (hit.transform.gameObject.tag == "GizmoX")
                MoveSelected(new Vector3(1, 0, 0));
            else if (hit.transform.gameObject.tag == "GizmoY")
                MoveSelected(new Vector3(0, 1, 0));
            else if (hit.transform.gameObject.tag == "GizmoZ")
                MoveSelected(new Vector3(0, 0, 1));
            else if (hit.transform.gameObject.tag == "GizmoXneg")
                MoveSelected(new Vector3(-1, 0, 0));
            else if (hit.transform.gameObject.tag == "GizmoYneg")
                MoveSelected(new Vector3(0, -1, 0));
            else if (hit.transform.gameObject.tag == "GizmoZneg")
                MoveSelected(new Vector3(0, 0, -1));
            else if(hit.transform.gameObject.tag == "JointBuilder")
            {
                int count = tempJoints.Count;
                for(int i = 0; i < count; i++)
                {
                    if(tempJoints[i].joint == hit.transform.gameObject)
                    {
                        tempJoints.Remove(tempJoints[i]);
                        break;
                    }
                }
                foreach(BuilderTempInstruction bti in tempJoints)
                {
                    if (bti.joint = hit.transform.gameObject)
                    {
                        tempJoints.Remove(bti);
                        break;
                    }                        
                }
                Destroy(hit.transform.gameObject);
            }else
                DeselectAllSelectedItems();
        }
        else
            DeselectAllSelectedItems();
    }
}

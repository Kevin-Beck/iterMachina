using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Brain : MonoBehaviour
{
    Vector3 origin;

    [SerializeField]
    LegoBox parts = null;

    [SerializeField]
    Vector3 instantiationDimension;

    [SerializeField]
    float spaceBetween = 0;

    [SerializeField]
    int numberOfDesiredNodes = 0;

    [SerializeField]
    GameObject nodePrefab;

    [SerializeField]
    GameObject jointPrefab;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> joints = new List<GameObject>();

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node
    public void Awake()
    {
        if(instantiationDimension == null)
        {
            instantiationDimension = new Vector3(10, 5, 10);
        }
    }
    public void TestBuilds()
    {
        
        int good = 0;
        int bad = 0;
        origin = transform.position;
        for (int i = 0; i < 10; i++)
        {
            if (ConstructNewRandomBody())
                good++;
            else
                bad++;

            DeconstructBody();
        }
        Debug.Log("Good: " + good + "  Bad: " + bad);
    }
    public void buildSingleBot()
    {
        ConstructNewRandomBody();
    }
    public void DeconstructBody()
    {
        int num = joints.Count;
        for(int i = 0; i < num; i++)
        {
            parts.ReturnJoint(joints[0]);
            joints.Remove(joints[0]);
        }

        int val = nodes.Count;
        for (int j = 0; j < val; j++)
        {
            parts.returnNode(nodes[0]);
            nodes.Remove(nodes[0]);
        }
    }
    public bool ConstructNewRandomBody() // returns false if body fails to be constructed
    {
        for(int i = 0; i < numberOfDesiredNodes; i++)
        {
            Vector3 position = GetValidSpaceForNode();
            if (position == ILLEGALVECTOR)
                return false;
            GameObject newlyCreatedNode = parts.getNode(position);
            newlyCreatedNode.GetComponent<Rigidbody>().useGravity = true;
            nodes.Add(newlyCreatedNode);

            if (i != 0)
            {
                if (!GetValidConnectionToNode(newlyCreatedNode, nodes[Random.Range(0, i)]))
                {                    
                    parts.returnNode(newlyCreatedNode);
                    nodes.Remove(newlyCreatedNode);                    
                    i--;
                }
            }
        }
        return true;
    }
    public void ToggleAllMuscles ()
    {
        foreach(GameObject gm in joints)
        {
            gm.GetComponent<JointScript>().ToggleMuscle();
        }
    }
    public void ConstructBodyFromGene()
    {

    }
    private bool GetValidConnectionToNode(GameObject newNode, GameObject oldNode)
    {
        // returns true if a valid connection is constructed
        // adds the connected object to both nodes

        // get direction vector between the two nodes

        Vector3 vector = (newNode.transform.position - oldNode.transform.position);
        if (vector.magnitude < 3.1f) 
            return false;

        Physics.SyncTransforms();
        Ray ray = new Ray(oldNode.transform.position + vector.normalized*1.5f, vector.normalized);
        RaycastHit[] rch = Physics.SphereCastAll(ray, .5f, vector.magnitude-3f);

        if (rch.Length > 0)
            return false;

        GameObject newlyCreatedJoint = parts.GetJoint(oldNode.transform.position + vector.normalized * .9f);
        joints.Add(newlyCreatedJoint);
        newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);

        newlyCreatedJoint.GetComponent<JointScript>().SetBoneSize(vector.magnitude - 2.5f);
        newlyCreatedJoint.GetComponent<JointScript>().ConnectBaseToNode(oldNode);
        newlyCreatedJoint.GetComponent<JointScript>().ConnectEdgeToNode(newNode);
        return true;

    }
    private Vector3 GetValidSpaceForNode() {
        bool validFound = false;
        int counter = 0; // safety brake on the while loop

        Vector3 target = Vector3.zero; 

        while (!validFound)
        {
            if (counter++ > 5)
                return ILLEGALVECTOR;

            target = new Vector3(Random.Range(0f, instantiationDimension.x), Random.Range(0f, instantiationDimension.y), Random.Range(0f, instantiationDimension.z));

            if (!Physics.CheckSphere(origin+target, spaceBetween))
                validFound = true;
        }
        return target;
    }
}

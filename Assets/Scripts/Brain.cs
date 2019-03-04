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
    int numberOfExtraConnections = 0;

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

    public void DeconstructBody()
    {
        ToggleAllMuscles();
        ToggleAllRenderers();

        int num = joints.Count;
        for(int i = 0; i < num; i++)
        {
            GameObject.Destroy(joints[0]);
            joints.Remove(joints[0]);
            Physics.SyncTransforms();
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
        // This loop attempts to create a body by creating a node in a valid location, then connecting it to an existing node
        for(int i = 0; i < numberOfDesiredNodes; i++)
        {
            Vector3 position = GetValidSpaceForNode();
            if (position == ILLEGALVECTOR)
                return false;
            GameObject newlyCreatedNode = parts.getNode(position);
            newlyCreatedNode.GetComponent<Rigidbody>().useGravity = true; // The new node must use gravity
            nodes.Add(newlyCreatedNode);

            if (i != 0) // If this is the first node, we just let it exist, otherwise we need to connect it to something
            {
                if (!GetValidConnectionToNode(newlyCreatedNode, nodes[Random.Range(0, i)]))
                {                    
                    parts.returnNode(newlyCreatedNode);
                    nodes.Remove(newlyCreatedNode);                    
                    i--;
                }
            }
        }
        // At this point we've created a bot that is an acyclic graph. in order to introduce the possibility of
        // cyclic structures we throw on an additional attempt to build a few connections

        // This next part splits the nodes into groups, then attempts to pair up two of those nodes in either group
        // once we successfully do this numberOfExtraConnections times (or 50 attempts) we jump out of the loop
        int count = 0;
        while(count < numberOfExtraConnections || count > 50)
        {
            int divider = Random.Range(1, nodes.Count - 1);
            if (GetValidConnectionToNode(nodes[Random.Range(0, divider)], nodes[Random.Range(divider, nodes.Count)]))
                count++;
        }
        return true;
    }

    public void MakeBody()
    {
        ConstructNewRandomBody();
        ToggleAllMuscles();
        ToggleAllRenderers();
    }

    public void ToggleAllMuscles ()
    {
        foreach (GameObject gm in joints)
            gm.GetComponent<JointScript>().ToggleMuscle();
    }
    public void ToggleAllRenderers()
    {
        foreach (GameObject joint in joints)
            joint.GetComponent<JointScript>().ToggleRenderer();
        foreach (GameObject node in nodes)
            node.GetComponent<NodeScript>().ToggleRenderer();
    }
    public void ConstructBodyFromGene()
    {

    }
    private bool GetValidConnectionToNode(GameObject newNode, GameObject oldNode)
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    DNA myDNA;

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node

    public void Awake()
    {
        myDNA = new DNA();
        if (instantiationDimension == null)
            instantiationDimension = new Vector3(10, 5, 10);
        
        origin = transform.position;
    }

    public void Start()
    {
        parts = GameObject.FindGameObjectWithTag("LegoBox").GetComponent<LegoBox>();
        
    }

    public void DeconstructBody()
    {
        ToggleAllMuscles();
        ToggleAllRenderers();

        int num = joints.Count;
        for(int i = 0; i < num; i++)
        {
            Destroy(joints[i]);
            Physics.SyncTransforms();
        }
        joints.Clear();

        int val = nodes.Count;
        for (int j = 0; j < val; j++)
        {
            parts.returnNode(nodes[j]);
        }
        nodes.Clear();
    }
    public bool ConstructNewRandomBody() // returns false if body fails to be constructed
    {
        myDNA.ClearDNA(); // making a new random body, gotta clear out old data in DNA
        nodes.Clear();
        joints.Clear();
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
                if (!GetValidConnectionToNode(nodes.Count - 1, Random.Range(0, i)))
                {                    
                    parts.returnNode(newlyCreatedNode);
                    nodes.Remove(newlyCreatedNode);                    
                    i--;
                }else
                    myDNA.AddToPositions(newlyCreatedNode.transform.position - origin); // This position is added in
            }else
                myDNA.AddToPositions(newlyCreatedNode.transform.position - origin);
        }
        // At this point we've created a bot that is an acyclic graph. in order to introduce the possibility of
        // cyclic structures we throw on an additional attempt to build a few connections

        // This next part splits the nodes into groups, then attempts to pair up two of those nodes in either group
        // once we successfully do this numberOfExtraConnections times (or 50 attempts) we jump out of the loop
        int count = 0;
        int loop = 0;
        while(count < numberOfExtraConnections && loop < 10)
        {
            loop++;
            int divider = Random.Range(1, nodes.Count-1);
            if (GetValidConnectionToNode(Random.Range(0, divider), Random.Range(divider, nodes.Count)))
                count++;
        }
        return true;
    }

    public void SetDNA(DNA newDNA)
    {
        myDNA = newDNA;
    }
    public DNA GetDNA()
    {
        return myDNA;
    }
    /**********************************************/
    public void MakeRandomBody()
    {
        ConstructNewRandomBody();
        ToggleAllMuscles();
        ToggleAllRenderers();
    }
    public void MakeDNABody()
    {
        ConstructBodyFromDNA();
        ToggleAllMuscles();
        ToggleAllRenderers();
    }
    /**************************************************/
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
    public void ConstructBodyFromDNA()
    {
        nodes.Clear();
        joints.Clear();
        for(int i = 0; i < myDNA.nodePositions.Count; i++)
            nodes.Add(Instantiate(nodePrefab, myDNA.nodePositions[i] + origin, Quaternion.identity));

        for(int i = 0; i < myDNA.designInstructions.Count; i++)
        {
            GameObject newNode = nodes.ElementAt(myDNA.designInstructions[i].targetNode);
            GameObject oldNode = nodes.ElementAt(myDNA.designInstructions[i].baseNode);

            Physics.SyncTransforms();

            Vector3 vector = (newNode.transform.position - oldNode.transform.position);
            GameObject newlyCreatedJoint = parts.GetJoint(oldNode.transform.position + vector.normalized * .9f);
            joints.Add(newlyCreatedJoint);
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);
            JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
            js.SetBoneSize(vector.magnitude - 2.5f);
            js.ConnectBaseToNode(oldNode);
            js.ConnectEdgeToNode(newNode);
            js.SetSineFactors(myDNA.designInstructions[i].GetSineFactors());
        }
    }
    private bool GetValidConnectionToNode(int a, int b)
    {
        GameObject newNode = nodes[a];
        GameObject oldNode = nodes[b];

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

        JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
        js.SetBoneSize(vector.magnitude - 2.5f);
        js.ConnectBaseToNode(oldNode);
        js.ConnectEdgeToNode(newNode);
        js.SetSineFactors(new Vector3(Random.Range(0f, 5f), Random.Range(0f, 3.2f), Random.Range(-1.8f, 1.8f)));

        myDNA.AddToInstructions(new Instruction(b, a, js.GetSineFactors()));
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

            target = origin + new Vector3(Random.Range(0f, instantiationDimension.x), Random.Range(0f, instantiationDimension.y), Random.Range(0f, instantiationDimension.z));

            if (!Physics.CheckSphere(origin+target, spaceBetween))
                validFound = true;
        }
        return target;
    }
}

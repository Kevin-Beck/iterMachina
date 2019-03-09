using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain : MonoBehaviour
{
    LegoBox legoBox = null; // This is the parts store that will deliver nodes and joints. Found in Start
    UIController userInterface = null; // This is the interface with the GUI. Found in Start

    GameData gameData = null; // A psuedo database for intial data consolidation
    Vector3 origin; // the original position of this brain
    
    Vector3 instantiationDimension; // This is the allowable room for the brain to build inside when randomly generated
     
    const float NODE_INSTANTIATION_RADIAL_CLEARANCE = 1.5f; // This is how much space is scanned when looking for nodal collisions
                                                         // basically the nodes have a scale of 1.5 currently, meaning that this isscanning a sapce of 1.5 radially from the 
                                                         // node to see if it collides. If a collision occurs the node is not validly placed.
    
    int numberOfDesiredNodes = 0;
    const int MAX_NODES = 15;
    int numberOfExtraConnections = 0;

    float chanceToMutateJoint = 0;

    List<GameObject> nodes;
    List<GameObject> joints;

    DNA myDNA;

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node

    // Awake is used to initialize the data for anything uninitialized
    public void Awake()
    {
        nodes = new List<GameObject>();
        joints = new List<GameObject>();
        myDNA = new DNA();
        if (instantiationDimension == null)
            instantiationDimension = new Vector3(10, 5, 10);
        
        origin = transform.position;
    }

    // Start is used to find all needed gameobjects by tag before starting any processing
    public void Start()
    {
        legoBox = GameObject.FindGameObjectWithTag("LegoBox").GetComponent<LegoBox>();
        userInterface = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        gameData = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();

        instantiationDimension = gameData.areaForInstantiation;
        numberOfDesiredNodes = gameData.numberOfNodes;
        numberOfExtraConnections = gameData.additionalConnections;
        chanceToMutateJoint = gameData.chanceToMutateJoint;

        
    }
    public void SetInstantiationDimension(Vector3 newDimension)
    {
        instantiationDimension = newDimension;
    }
    public void SetNumberOfDesiredNodes(int num)
    {
        if (num <= MAX_NODES)
            numberOfDesiredNodes = num;
        else
            userInterface.LogDataToScreen("Brain: NumberOfDesiredNodes > MAX_NODES");
    }
    public void SetNumberOfAdditionalConnections(int num)
    {
        numberOfExtraConnections = num;
    }

    public void SetDesiredNumberOfNodes(int newNum)
    {
        if(newNum > 0 && newNum < MAX_NODES)
            numberOfDesiredNodes = newNum;
    }

    public float CalculateCurrentScore()
    {
        FinalizeNodesForScoring();
        float Xmagnitude = 0;
        for(int i = 0; i < nodes.Count; i++)
            Xmagnitude += (nodes[0].GetComponent<NodeScript>().GetCurrentScore());        
        return Xmagnitude;
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
            legoBox.returnNode(nodes[j]);
        }
        nodes.Clear();
    }
    public bool ConstructNewRandomBody() // returns false if body fails to be constructed
    {
        myDNA.ClearDNA(); // making a new random body, gotta clear out old data in DNA
        nodes.Clear();
        joints.Clear();
        // This loop attempts to create a body by creating a node in a valid location, then connecting it to an existing node
        for (int i = 0; i < numberOfDesiredNodes; i++)
        {
            Vector3 position = GetValidSpaceForNode();
            if (position == ILLEGALVECTOR)
                return false;
            GameObject newlyCreatedNode = legoBox.getNode(position);
            newlyCreatedNode.GetComponent<Rigidbody>().useGravity = true; // The new node must use gravity
            nodes.Add(newlyCreatedNode);           

            if (i != 0) // If this is the first node, we just let it exist, otherwise we need to connect it to something
            {
                if (!GetValidConnectionToNode(nodes.Count - 1, Random.Range(0, i)))
                {                    
                    legoBox.returnNode(newlyCreatedNode);
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
        myDNA = new DNA(newDNA);
    }
    public DNA GetDNA()
    {
        return myDNA;
    }
    /**********************************************/
    public void MakeRandomBody()
    {
        ConstructNewRandomBody();
        InitializeNodesForScoring();
        ToggleAllMuscles();
        ToggleAllRenderers();
    }
    public void MakeDNABody()
    {
        ConstructBodyFromDNA();
        InitializeNodesForScoring();
        ToggleAllMuscles();
        ToggleAllRenderers();
    }
    public void MakeMutatedDNABody()
    {
        ConstructMutatedBodyFromDNA();
        InitializeNodesForScoring();
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
    public Instruction MutateDNAInstruction(Instruction old)
    {
        float roll = Random.Range(0f, 1f);
        if (gameData.chanceToRerollJointEntirely > roll)
            return new Instruction(old.baseNode, old.targetNode, GetRandomSineFactors());
        else if (chanceToMutateJoint > roll)
            return new Instruction(old.baseNode, old.targetNode, GetTweakSineFactors(old.GetSineFactors()));
        else
            return old;
    }
    public void ConstructBodyFromDNA()
    {
        for (int i = 0; i < myDNA.nodePositions.Count; i++)
            nodes.Add(legoBox.getNode(myDNA.nodePositions[i] + origin));

        for (int i = 0; i < myDNA.designInstructions.Count; i++)
        {
            GameObject newNode = nodes.ElementAt(myDNA.designInstructions[i].targetNode);
            GameObject oldNode = nodes.ElementAt(myDNA.designInstructions[i].baseNode);

            Physics.SyncTransforms();

            Vector3 vector = (newNode.transform.position - oldNode.transform.position);
            GameObject newlyCreatedJoint = legoBox.GetJoint(oldNode.transform.position + vector.normalized * .9f);
            joints.Add(newlyCreatedJoint);
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);
            JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
            js.SetBoneSize(vector.magnitude - 2.5f);
            js.ConnectBaseToNode(oldNode);
            js.ConnectEdgeToNode(newNode);
            js.SetSineFactors(myDNA.designInstructions[i].GetSineFactors());
        }
    }
    public void ConstructMutatedBodyFromDNA()
    {
        for(int i = 0; i < myDNA.nodePositions.Count; i++)
            nodes.Add(legoBox.getNode(myDNA.nodePositions[i]+origin));

        for(int i = 0; i < myDNA.designInstructions.Count; i++)
        {
            GameObject newNode = nodes.ElementAt(myDNA.designInstructions[i].targetNode);
            GameObject oldNode = nodes.ElementAt(myDNA.designInstructions[i].baseNode);

            Physics.SyncTransforms();

            Vector3 vector = (newNode.transform.position - oldNode.transform.position);
            GameObject newlyCreatedJoint = legoBox.GetJoint(oldNode.transform.position + vector.normalized * .9f);
            joints.Add(newlyCreatedJoint);
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);

            JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
            js.SetBoneSize(vector.magnitude - 2.5f);
            js.ConnectBaseToNode(oldNode);
            js.ConnectEdgeToNode(newNode);

            myDNA.designInstructions[i] = MutateDNAInstruction(myDNA.designInstructions[i]);

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

        GameObject newlyCreatedJoint = legoBox.GetJoint(oldNode.transform.position + vector.normalized * .9f);
        joints.Add(newlyCreatedJoint);
        newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);

        JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
        js.SetBoneSize(vector.magnitude - 2.5f);
        js.ConnectBaseToNode(oldNode);
        js.ConnectEdgeToNode(newNode);
        js.SetSineFactors(GetRandomSineFactors());

        myDNA.AddToInstructions(new Instruction(b, a, js.GetSineFactors()));
        return true;
    }
    private Vector3 GetTweakSineFactors(Vector3 curSine)
    {
        return curSine + new Vector3(Random.Range(-.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
    }
    public Vector3 GetRandomSineFactors()
    {
        return new Vector3(Random.Range(0f, 5f), Random.Range(0f, 3.2f), Random.Range(-1.8f, 1.8f));
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

            if (!Physics.CheckSphere(origin+target, NODE_INSTANTIATION_RADIAL_CLEARANCE))
                validFound = true;
        }
        return target;
    }
    private void InitializeNodesForScoring()
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            nodes[i].GetComponent<NodeScript>().SetStartPosition();
        }
    }
    private void FinalizeNodesForScoring()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].GetComponent<NodeScript>().SetEndPosition();
        }
    }
}

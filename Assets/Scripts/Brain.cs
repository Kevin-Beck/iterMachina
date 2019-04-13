using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain : MonoBehaviour
{
    GameData gd; // A psuedo database for intial data consolidation
    Vector3 origin; // the original position of this brain

    const float NODE_INSTANTIATION_RADIAL_CLEARANCE = 1.5f; // This is how much space is scanned when looking for nodal collisions
                                                         // basically the nodes have a scale of 1.5 currently, meaning that this isscanning a sapce of 1.5 radially from the 
                                                         // node to see if it collides. If a collision occurs the node is not validly placed.
    List<GameObject> nodes;
    List<GameObject> joints;

    DNA myDNA;

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node

    // Awake is used to initialize the data for anything uninitialized
    public void Awake()
    {
        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        nodes = new List<GameObject>();
        joints = new List<GameObject>();
        myDNA = new DNA();
        
        origin = transform.position;
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
            gd.lb.ReturnNode(nodes[j]);
        }
        nodes.Clear();
    }
    public bool ConstructNewRandomBody() // returns false if body fails to be constructed
    {
        myDNA.ClearDNA(); // making a new random body, gotta clear out old data in DNA
        nodes.Clear();
        joints.Clear();
        // This loop attempts to create a body by creating a node in a valid location, then connecting it to an existing node
        for (int i = 0; i < gd.numberOfNodes; i++)
        {
            Vector3 position = GetValidSpaceForNode();
            if (position == ILLEGALVECTOR)
                return false;
            GameObject newlyCreatedNode = gd.lb.GetNode(position);
            newlyCreatedNode.GetComponent<Rigidbody>().useGravity = true; // The new node must use gravity
            nodes.Add(newlyCreatedNode);           

            if (i != 0) // If this is the first node, we just let it exist, otherwise we need to connect it to something
            {
                if (!GetValidConnectionToNode(nodes.Count - 1, Random.Range(0, i)))
                {                    
                    gd.lb.ReturnNode(newlyCreatedNode);
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
        while(count < gd.additionalConnections && loop < 10)
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
        if (gd.chanceToRerollJointEntirely > roll)
            return new Instruction(old.baseNode, old.targetNode, GetRandomSineFactors(), GetRandomRotFactor());
        else if (gd.chanceToMutateJoint> roll)
            return new Instruction(old.baseNode, old.targetNode, GetTweakSineFactors(old.GetSineFactors()), GetTweakRotFactor(old.GetRotation()));
        else
            return old;
    }
    public void ConstructBodyFromDNA()
    {
        for (int i = 0; i < myDNA.nodePositions.Count; i++)
            nodes.Add(gd.lb.GetNode(myDNA.nodePositions[i] + origin));

        for (int i = 0; i < myDNA.designInstructions.Count; i++)
        {
            GameObject newNode = nodes.ElementAt(myDNA.designInstructions[i].targetNode);
            GameObject oldNode = nodes.ElementAt(myDNA.designInstructions[i].baseNode);

            Physics.SyncTransforms();

            Vector3 vector = (newNode.transform.position - oldNode.transform.position);
            GameObject newlyCreatedJoint = gd.lb.GetJoint(oldNode.transform.position + vector.normalized * .9f);
            joints.Add(newlyCreatedJoint);
            
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);
            newlyCreatedJoint.transform.rotation *= Quaternion.Euler(myDNA.designInstructions[i].GetRotation(), 0, 0);

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
            nodes.Add(gd.lb.GetNode(myDNA.nodePositions[i]+origin));

        for(int i = 0; i < myDNA.designInstructions.Count; i++)
        {
            GameObject newNode = nodes.ElementAt(myDNA.designInstructions[i].targetNode);
            GameObject oldNode = nodes.ElementAt(myDNA.designInstructions[i].baseNode);

            Physics.SyncTransforms();

            myDNA.designInstructions[i] = MutateDNAInstruction(myDNA.designInstructions[i]);

            Vector3 vector = (newNode.transform.position - oldNode.transform.position);
            GameObject newlyCreatedJoint = gd.lb.GetJoint(oldNode.transform.position + vector.normalized * .9f);
            joints.Add(newlyCreatedJoint);
            newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);
            newlyCreatedJoint.transform.rotation *= Quaternion.Euler(myDNA.designInstructions[i].GetRotation(), 0, 0);

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

        GameObject newlyCreatedJoint = gd.lb.GetJoint(oldNode.transform.position + vector.normalized * .9f);
        joints.Add(newlyCreatedJoint);
        newlyCreatedJoint.transform.rotation = Quaternion.FromToRotation(Vector3.right, vector);

        JointScript js = newlyCreatedJoint.GetComponent<JointScript>();
        js.SetBoneSize(vector.magnitude - 2.5f);
        js.ConnectBaseToNode(oldNode);
        js.ConnectEdgeToNode(newNode);
        js.SetSineFactors(GetRandomSineFactors());

        myDNA.AddToInstructions(new Instruction(b, a, js.GetSineFactors(), GetRandomRotFactor()));
        return true;
    }
    private Vector3 GetTweakSineFactors(Vector3 curSine)
    {
        Vector3 newSine = curSine + new Vector3(Random.Range(-.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        if (newSine.x > gd.aMax || newSine.x < gd.aMin)
            newSine.x = Random.Range(gd.aMin, gd.aMax);
        if (newSine.y > gd.bMax || newSine.y < gd.bMin)
            newSine.y = Random.Range(gd.bMin, gd.bMax);
        if (newSine.z > gd.cMax || newSine.z < gd.cMin)
            newSine.z = Random.Range(gd.cMin, gd.cMax);
        return newSine;
    }
    public Vector3 GetRandomSineFactors()
    {
        return new Vector3(Random.Range(gd.aMin, gd.aMax), Random.Range(gd.bMin, gd.bMax), Random.Range(gd.cMin, gd.cMax));
    }
    private Vector3 GetValidSpaceForNode() {
        bool validFound = false;
        int counter = 0; // safety brake on the while loop

        Vector3 target = Vector3.zero; 

        while (!validFound)
        {
            if (counter++ > 5)
                return ILLEGALVECTOR;

            target = origin + new Vector3(Random.Range(0f, gd.areaForInstantiation.x), Random.Range(0f, gd.areaForInstantiation.y), Random.Range(0f, gd.areaForInstantiation.z));

            if (!Physics.CheckSphere(origin+target, NODE_INSTANTIATION_RADIAL_CLEARANCE))
                validFound = true;
        }
        return target;
    }
    public float GetRandomRotFactor()
    {
        return Random.Range(0, 180);
    }
    private float GetTweakRotFactor(float curRot)
    {
        float newRot = curRot + Random.Range(-5f, 5f);
        if (newRot > 180 || newRot < 0)
            newRot = Random.Range(0f, 180f);
        return newRot;
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

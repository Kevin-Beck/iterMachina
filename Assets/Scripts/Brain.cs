using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Brain : MonoBehaviour
{
    Vector3 origin;

    [SerializeField]
    LegoBox parts;

    [SerializeField]
    Vector3 instantiationDimension;

    [SerializeField]
    float spaceBetween;

    [SerializeField]
    int numberOfDesiredNodes;

    [SerializeField]
    GameObject nodePrefab;

    [SerializeField]
    GameObject jointPrefab;

    [SerializeField]
    GameObject markerPrefab;

    List<GameObject> nodes = new List<GameObject>();
    List<GameObject> joints = new List<GameObject>();

    [SerializeField]
    GameObject testingNode; // temp

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node
    // Start is called before the first frame update

    
    public void TestBuilds()
    {
        
        int good = 0;
        int bad = 0;
        origin = transform.position;
        for (int i = 0; i < 1000; i++)
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
        int val = nodes.Count;
        for (int j = 0; j < val; j++)
        {
            parts.returnNode(nodes[0]);
            nodes.Remove(nodes[0]);
        }

        // Destroy all the joints
        // remove them from the list
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
    public void tempMakeJointObject()
    {
        Instantiate(jointPrefab, testingNode.transform.position + new Vector3(3, 0, 0), Quaternion.FromToRotation(testingNode.transform.position, new Vector3(0, 100, 0)));
    }
    public void ConstructBodyFromGene()
    {

    }
    private bool GetValidConnectionToNode(GameObject newNode, GameObject oldNode)
    {
        // returns true if a valid connection is constructed
        // adds the connected object to both nodes

        // get direction vector between the two nodes
        Vector3 newNodePosition = newNode.transform.position;
        Vector3 oldNodePosition = oldNode.transform.position;

        Vector3 vector = (newNodePosition - oldNodePosition);

        Physics.SyncTransforms();
        Ray ray = new Ray(oldNodePosition+vector.normalized*1.5f, vector.normalized);
        RaycastHit[] rch = Physics.SphereCastAll(ray, .5f, vector.magnitude-3f);

        if (rch.Length > 0)
            return false;
        Debug.Log(rch.Length);
        Debug.DrawRay(oldNodePosition + vector.normalized * 1.5f, vector-(vector.normalized*3), Color.green, 60.0f, true);
        


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

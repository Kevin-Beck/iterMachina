using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    Vector3 origin;

    [SerializeField]
    float instantiationRadius;

    [SerializeField]
    int numberOfDesiredNodes;

    [SerializeField]
    GameObject nodePrefab;

    [SerializeField]
    GameObject jointPrefab;

    [SerializeField]
    List<GameObject> nodes = new List<GameObject>();

    Vector3 ILLEGALVECTOR = new Vector3(-999, -999, -999); // This is returned if no valid position is found for node
    // Start is called before the first frame update
    void Start()
    {
        int good = 0;
        int bad = 0;
        origin = transform.position;
        for(int i = 0; i < 100; i++)
        {
            if (ConstructNewRandomBody())
                good++;
            else
                bad++;
            if (i == 99)
            { }
            else
            {
                foreach (GameObject n in nodes)
                {
                    Destroy(n);
                }
            }
        }        
        Debug.Log("Good: " + good + "  Bad: " + bad);
    } 

    public bool ConstructNewRandomBody() // returns false if body fails to be constructed
    {
        for(int i = 0; i < numberOfDesiredNodes; i++)
        {
            Vector3 position = GetValidPositionForNode();
            if (position == ILLEGALVECTOR)
                return false;
            GameObject a = Instantiate(nodePrefab, origin+position, Quaternion.identity);
            // TODO instead of instantiating from a prefab, get one from a data object in the scene
            // the data object will distribute nodes and joints to anyone who requests one
            a.GetComponent<Rigidbody>().useGravity = false; // TEMP
            nodes.Add(a);
            // Pass nodes to joint constructor and use joint Script constructor to create the
            // scaling and connections
        }
        return true;
    }
    public void ConstructBodyFromGene()
    {

    }
    private Vector3 GetValidPositionForNode() {
        bool validFound = false;
        int counter = 0; // safety brake on the while loop
        // issue
        Vector3 target = Vector3.zero;  // potentially our returned valid location
        // issue;

        while (!validFound)
        {
            if (counter++ > 50)
                return ILLEGALVECTOR;
                
            // Create a random location, and spawn a node there
            // target is invalid if it collides with anything
            target = new Vector3(Random.Range(0, instantiationRadius), Random.Range(0, instantiationRadius), Random.Range(0, instantiationRadius));
            if (!Physics.CheckSphere(origin+target, 1f))
                validFound = true;
        }
        return target;
    }
}

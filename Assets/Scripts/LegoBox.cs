using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LegoBox : MonoBehaviour
{
    GameObject nodePrefab = null;    
    List<GameObject> nodeBox;
    Vector3 nodeSpawnStart;
    int nodeCounter;
    
    GameObject jointPrefab = null;
    List<GameObject> jointBox;
    Vector3 jointSpawnStart;
    int jointCounter;

    // Connections
    UIController ui;
    GameData gd;


    public void Awake()
    {
        nodeCounter = -1;
        nodeSpawnStart = gameObject.GetComponent<Transform>().position;
        nodeBox = new List<GameObject>();

        jointCounter = -1;
        jointSpawnStart = gameObject.GetComponent<Transform>().position + new Vector3(0, -3, 0);
        jointBox = new List<GameObject>();

        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();


        jointPrefab = gd.jointPrefab;
        nodePrefab = gd.nodePrefab;
    }
    public void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        /*
        for (int i = 0; i < 400; i++)
            createMoreNodes();
        for (int i = 0; i < 400; i++)
            CreateMoreJoints();
            */
    }

    public void FixedUpdate()
    {
            
    }

    public GameObject getNode(Vector3 pos)
    {
        if (nodeCounter == -1)
            createMoreNodes();
        GameObject nodeToSend = nodeBox[nodeCounter];
        nodeBox.RemoveAt(nodeCounter);
        nodeCounter--;
        nodeToSend.GetComponent<Transform>().position = pos;

        return nodeToSend;
    }
    public void returnNode(GameObject returnedNode)
    {
        Rigidbody rb = returnedNode.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        nodeBox.Add(returnedNode);
        nodeCounter++;
        returnedNode.GetComponent<Transform>().SetPositionAndRotation(new Vector3(nodeCounter*2,0,0) + nodeSpawnStart, Quaternion.identity);
    }
    private void createMoreNodes()
    {
        nodeCounter++;
        nodeBox.Add(Instantiate(nodePrefab, new Vector3(nodeCounter * 2, 0, 0) + nodeSpawnStart, Quaternion.identity));
    }

    public GameObject GetJoint(Vector3 pos)
    {
        if (jointCounter == -1)
            CreateMoreJoints();
        GameObject jointToSend = jointBox[jointCounter];
        jointBox.RemoveAt(jointCounter);
        jointCounter--;
        jointToSend.GetComponent<Transform>().position = pos;

        return jointToSend;
    }

    private void CreateMoreJoints()
    {
        jointCounter++;
        jointBox.Add(Instantiate(jointPrefab, new Vector3(jointCounter * 2, 0, 0) + jointSpawnStart, Quaternion.identity));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LegoBox : MonoBehaviour
{
    List<GameObject> nodeBox;
    int nodeCounter;
    List<GameObject> jointBox;
    int jointCounter;
    
    GameData gd;

    public void Awake()
    {
        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        gd.lb = gameObject.GetComponent<LegoBox>();

        nodeCounter = -1;
        nodeBox = new List<GameObject>();

        jointCounter = -1;
        jointBox = new List<GameObject>();
    }

    public GameObject GetNode(Vector3 pos)
    {
        if (nodeCounter == -1)
            CreateMoreNodes();
        GameObject nodeToSend = nodeBox[nodeCounter];
        nodeBox.RemoveAt(nodeCounter);
        nodeCounter--;
        nodeToSend.GetComponent<Transform>().position = pos;
        return nodeToSend;
    }
    public void ReturnNode(GameObject returnedNode)
    {
        Rigidbody rb = returnedNode.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        nodeBox.Add(returnedNode);
        nodeCounter++;
        returnedNode.GetComponent<Transform>().SetPositionAndRotation(new Vector3(nodeCounter*2,0,0) + gd.NodeSpawnerPosition, Quaternion.identity);
    }
    private void CreateMoreNodes()
    {
        nodeCounter++;
        nodeBox.Add(Instantiate(gd.nodePrefab, new Vector3(nodeCounter * 2, 0, 0) + gd.NodeSpawnerPosition, Quaternion.identity));
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
        jointBox.Add(Instantiate(gd.jointPrefab, new Vector3(jointCounter * gd.jointSpawnerSpacing, 0, 0) + gd.JointSpawnerPosition, Quaternion.identity));
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LegoBox : MonoBehaviour
{
    [SerializeField]
    GameObject nodePrefab;
    
    List<GameObject> nodeBox;

    Vector3 nodeSpawnStart;
    int nodeCounter;

    public void Start()
    {
        nodeCounter = -1;
        nodeSpawnStart = gameObject.GetComponent<Transform>().position;
        nodeBox = new List<GameObject>();
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
        nodeBox.Add(Instantiate(nodePrefab, nodeSpawnStart, Quaternion.identity));
    }
}

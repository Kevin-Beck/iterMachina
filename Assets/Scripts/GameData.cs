using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{


    [Header("Population/Terrain Information")]
    [SerializeField]
    public int distanceBetweenInstantiations;
    [SerializeField]
    public int numberPerRow = 0;
    [SerializeField]
    public int generationNumber = 0;

    [Header("Brain Information")]
    [SerializeField]
    public Vector3 areaForInstantiation;
    [SerializeField]
    public int numberOfNodes = 0;
    [SerializeField]
    public int additionalConnections = 0;
    [Space(10)]

    [Header("Prefabs")]
    [SerializeField]
    public GameObject brainPrefab = null;
    [SerializeField]
    public GameObject nodePrefab = null;
    [SerializeField]
    public GameObject jointPrefab = null;

    [Header("Connections")]
    [SerializeField]
    UIController ui;



    public void Awake()
    {
        
    }

    public void Start() // Get Connections to relevant controllers
    {
        ui = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
    }
    


    void Update()
    {

    }
}

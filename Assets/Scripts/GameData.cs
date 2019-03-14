using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("Positions Of Spawners")]
    [SerializeField]
    public Vector3 NodeSpawnerPosition;
    public float nodeSpawnerSpacing = 2;
    public Vector3 JointSpawnerPosition;
    public float jointSpawnerSpacing = 2;

    [Header("Population/Terrain Information")]
    [SerializeField]
    public int distanceBetweenInstantiations;
    public int numberPerRow = 0;
    public int MAX_PER_ROW = 15;
    public int generationNumber = 0;
    public float testingtime = 0;
    public float bestScore = 0;

    [Header("Brain Information")]
    [SerializeField]
    public Vector3 areaForInstantiation;
    public int numberOfNodes = 0;
    public int additionalConnections = 0;
    [Space(10)]
    public float chanceToMutateJoint = 0.0f;
    public float chanceToRerollJointEntirely = 0.0f;

    [Header("Prefabs")]
    [SerializeField]
    public GameObject brainPrefab = null;
    public GameObject nodePrefab = null;
    public GameObject jointPrefab = null;

    [Header("Connections")]
    [SerializeField]
    public UIController ui;
    public PopulationController pc;
    public LegoBox lb;
    public TerrainController tc;

    [Header("Sine Factors")]
    [Header("Sin(a*Cos(b*time)+c)")]
    public float aMin = 0f;
    public float aMax = 5f;
    public float bMin = 0f;
    public float bMax = 3.2f;
    public float cMin = -1.8f;
    public float cMax = 1.8f;


    public void Start() // Get Connections to relevant controllers
    {
        ui = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        pc = GameObject.FindGameObjectWithTag("PopulationController").GetComponent<PopulationController>();
        lb = GameObject.FindGameObjectWithTag("LegoBox").GetComponent<LegoBox>();
        tc = GameObject.FindGameObjectWithTag("TerrainController").GetComponent<TerrainController>();
    }    
}

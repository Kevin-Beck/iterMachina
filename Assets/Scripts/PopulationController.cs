using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PopulationController : MonoBehaviour
{
    // Data for size of field
    Vector3 instantiationFieldSize;
    int numberPerRow = 0;
    int generationNumber = 0;
    int distanceBetweenInstantiations = 0;

    GameObject brainPrefab = null;

    List<Brain> generation;
    
    UIController userInterface;
    GameData gameData;

    float timerMax = 10;
    float timer = -5;

    DNA currentBestDNA;
    float maxBestScore;

    public void Awake()
    {
        generation = new List<Brain>();
    }
    public void Start()
    {
        userInterface = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        gameData = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();

        distanceBetweenInstantiations = gameData.distanceBetweenInstantiations;
        numberPerRow = gameData.numberPerRow;
        generationNumber = gameData.generationNumber;
        brainPrefab = gameData.brainPrefab;
        maxBestScore = 0;
    }
    public void TEMPLOOP3456()
    {
        CalculateAllScores();
        DeconstructPopulation();
        CopyBestDNAFromBrains();
        GeneratePopulationFromBestDNA();
    }
    public void FixedUpdate()
    {

    }
    public void GenerateRandomPopulation()
    {
        foreach (Brain bs in generation)
            bs.MakeRandomBody();
    }

    public void GenerateBrains()
    {
        numberPerRow = gameData.numberPerRow;
        if(generation.Count!= numberPerRow*numberPerRow)
        {
            foreach (Brain bs in generation)
                Destroy(bs.gameObject);
            generation.Clear();

            for (int i = 0; i < numberPerRow; i++)
            {
                for (int j = 0; j < numberPerRow; j++)
                    generation.Add(Instantiate(brainPrefab, new Vector3(i * distanceBetweenInstantiations, 0, j * distanceBetweenInstantiations), Quaternion.identity).GetComponent<Brain>());
            }
        }
    }
    public void GeneratePopulationFromBestDNA()
    {
        for(int i = 0; i < generation.Count; i++)
        {
            if (i == 0)
                generation.ElementAt(0).MakeDNABody();
            else
                generation.ElementAt(i).MakeMutatedDNABody();
        }
    }
    public void CalculateAllScores()
    {
        foreach (Brain bs in generation)
        {
            float curScore = bs.CalculateCurrentScore();
            if (curScore > maxBestScore)
            {
                maxBestScore = curScore;
                currentBestDNA = bs.GetDNA();
                Debug.Log("best: " + maxBestScore);
            }
        }
    }
    public void DeconstructPopulation()
    {
        foreach (Brain bs in generation)
            bs.DeconstructBody();    
    }
    public void ToggleAllRenders()
    {
        foreach (Brain bs in generation)
            bs.ToggleAllRenderers();
    }
    public void ToggleAllMuscles()
    {
        foreach (Brain bs in generation)
            bs.ToggleAllMuscles();
    }
    public void CopyBestDNAFromBrains()
    {
        for (int i = 0; i < generation.Count; i++)
            generation.ElementAt(i).SetDNA(currentBestDNA);
    }
}

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
    }

    public void GenerateRandomPopulation()
    {
        if(!generation.Any())
            GenerateBrains();

        Debug.Log(generation.Count);

        foreach (Brain bs in generation)
            bs.MakeRandomBody();
    }

    public void GenerateBrains()
    {
        for (int i = 0; i < numberPerRow; i++)
        {
            for (int j = 0; j < numberPerRow; j++)
                generation.Add(Instantiate(brainPrefab, new Vector3(i * distanceBetweenInstantiations, 0, j * distanceBetweenInstantiations), Quaternion.identity).GetComponent<Brain>());
        }
    }
    public void GeneratePopulationFromPositionOne()
    {
        CopyBestDNAFromBrains();
        foreach (Brain bs in generation)
            bs.MakeDNABody();
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
        DNA best = generation.ElementAt(0).GetDNA();
        for (int i = 0; i < generation.Count; i++)
        {
            generation.ElementAt(i).SetDNA(best);
        }
    }

}

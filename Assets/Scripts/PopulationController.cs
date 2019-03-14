using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

public class PopulationController : MonoBehaviour
{
    List<Brain> generation;
    GameData gd;
    
    DNA currentBestDNA;
    //TODO instead of just the best DNA take the Top x
    //When one is created, it has a chance to take any of their DNA intermingled
    //Then slight variations are created.
    // bot takes 3 joints from A and 1 joint from B
    // then all joints are slightly tweaked
    // or bot takes all 4 joints from B
    // then all joints are slightly tweaked.

    // TODO check on the variances of the sineFactors
    // Perhaps squeeze them into closer margins for less chaos
    
    // TODO try copying 1 joint's timings to other joints in the same bot

    // TODO try creating symetric bots

    // Potentially output the individual instructions to see what is going on

    public void Awake()
    {
        gd = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
        generation = new List<Brain>();
    }
    public void Start()
    {
        GenerateBrains();

        //   Invoke("GenerateRandomPopulation", 1);
        Invoke("TestLoop", 1);
       // Invoke("TEMPLOOP3456", gd.testingtime);
        
    }
    public void TestLoop()
    {
        StreamReader reader = new StreamReader("Assets/Resources/test.txt");
        currentBestDNA = new DNA(reader.ReadLine());


        CopyBestDNAToBrains();
        GeneratePopulationFromBestDNA();
        Invoke("TEMPLOOP3456", gd.testingtime);
    }
    public void TEMPLOOP3456()
    {
        
        gd.testingtime++;
        bool reset = false;
        if (Random.Range(0f, 1f) > .95f)
        {
            reset = true;
        }
        CalculateAllScores();
        DeconstructPopulation();

        gd.numberPerRow = 8;

        if(reset)
        {
            gd.testingtime = 10;
            gd.bestScore = 0;
            reset = false;
        }

        GenerateBrains();
        CopyBestDNAToBrains();
        GeneratePopulationFromBestDNA();
        Invoke("TEMPLOOP3456", gd.testingtime);
    }

    public void GenerateRandomPopulation()
    {
        foreach (Brain bs in generation)
            bs.MakeRandomBody();
    }
    public void GenerateBrains()
    {
        if(generation.Count != gd.numberPerRow*gd.numberPerRow)
        {
            foreach (Brain bs in generation)
                Destroy(bs.gameObject);
            generation.Clear();

            for (int i = 0; i < gd.numberPerRow; i++)
            {
                for (int j = 0; j < gd.numberPerRow; j++)
                    generation.Add(Instantiate(gd.brainPrefab, new Vector3(i * gd.distanceBetweenInstantiations, 0, j * gd.distanceBetweenInstantiations), Quaternion.identity).GetComponent<Brain>());
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
            if (curScore > gd.bestScore)
            {
                gd.bestScore = curScore;
                currentBestDNA = bs.GetDNA();
                Debug.Log("best: " + gd.bestScore);
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
    public void CopyBestDNAToBrains()
    {
        for (int i = 0; i < generation.Count; i++)
            generation.ElementAt(i).SetDNA(currentBestDNA);
    }
}
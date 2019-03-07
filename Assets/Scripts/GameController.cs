using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField]
    int numberPerRow = 0;

    List<GameObject> generation;

    [SerializeField]
    GameObject brainPrefab = null;

    public void Awake()
    {
        generation = new List<GameObject>();
    }
    public void ReloadScene0()
    {
        SceneManager.LoadScene(0);
    }
    public void GenerateRandomPopulation()
    {
        if(!generation.Any())
        {
            for (int i = 0; i < numberPerRow; i++)
            {
                for (int j = 0; j < numberPerRow; j++)
                    generation.Add(Instantiate(brainPrefab, new Vector3(i * 25, 0, j * 25), Quaternion.identity));
            }
        }else
        {
            foreach (GameObject brainObject in generation)
                brainObject.GetComponent<Brain>().MakeRandomBody();
        }

    }
    public void GeneratePopulationFromPositionOne()
    {
        CopyBestDNAFromBrains();
        foreach (GameObject brainObject in generation)
            brainObject.GetComponent<Brain>().MakeDNABody();
    }
    public void DeconstructPopulation()
    {
        foreach (GameObject brainObject in generation)
        {
            brainObject.GetComponent<Brain>().DeconstructBody();
        }       
    }
    public void ToggleAllRenders()
    {
        foreach (GameObject brainObject in generation)
            brainObject.GetComponent<Brain>().ToggleAllRenderers();
    }
    public void ToggleAllMuscles()
    {
        foreach (GameObject brainObject in generation)
            brainObject.GetComponent<Brain>().ToggleAllMuscles();
    }
    public void CopyBestDNAFromBrains()
    {
        DNA best = generation.ElementAt(0).GetComponent<Brain>().GetDNA();
        for (int i = 0; i < generation.Count; i++)
        {
            generation.ElementAt(i).GetComponent<Brain>().SetDNA(best);
        }
    }

}

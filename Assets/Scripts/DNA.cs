using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Linq;

public class DNA
{
    public List<Vector3> nodePositions;
    public List<Instruction> designInstructions;

    public DNA()
    {
        nodePositions = new List<Vector3>();
        designInstructions = new List<Instruction>();
    }
    // TEMP COMMENTING OUT STRING
    /* This does not properly account for rotation of joint objects
    public DNA(string dnaString)
    {
        nodePositions = new List<Vector3>();
        designInstructions = new List<Instruction>();


        string s = dnaString.Replace(" ", String.Empty);
        string[] values = s.Split(',');
        float[] valuesf = Array.ConvertAll(values, float.Parse);
        //
        int numberOfNodes = (int)valuesf[valuesf.Length - 2];
        Debug.Log(numberOfNodes);
        int numberOfInstructions = (int)valuesf[valuesf.Length-1];

        for(int i = 0; i < numberOfNodes*3; i++)
        {
            float x = valuesf[i];
            float y = valuesf[++i];
            float z = valuesf[++i];

            AddToPositions(new Vector3(x, y, z));
        }
        for (int i = numberOfNodes * 3; i < values.Length - 6; i++)
        {
            int baseNode = (int) valuesf[i];
            int targetNode = (int) valuesf[++i];
            float a = valuesf[++i];
            float b = valuesf[++i];
            float c = valuesf[++i];

            AddToInstructions(new Instruction(baseNode, targetNode, new Vector3(a, b, c)));
        }
        Debug.Log(toData());
    }
    */
    public DNA(List<Vector3> pos, List<Instruction> inst)
    {
        nodePositions = pos;
        designInstructions = inst;
    }
    public DNA(DNA copy)
    {
        nodePositions = new List<Vector3>();
        designInstructions = new List<Instruction>();
        for(int i = 0; i < copy.nodePositions.Count; i++)
        {
            Vector3 pos = new Vector3(copy.nodePositions[i].x, copy.nodePositions[i].y, copy.nodePositions[i].z);
            nodePositions.Add(pos);
        }
        for(int i = 0; i < copy.designInstructions.Count; i++)
        {
            Instruction inst = new Instruction(copy.designInstructions[i].baseNode, copy.designInstructions[i].targetNode, new Vector3(copy.designInstructions[i].GetSineFactors().x, copy.designInstructions[i].GetSineFactors().y, copy.designInstructions[i].GetSineFactors().z), copy.designInstructions[i].GetRotation());
            designInstructions.Add(inst);
        }
    }
    public void Awake()
    {
        nodePositions = new List<Vector3>();
        designInstructions = new List<Instruction>();
    }

    public string toData()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Vector3 np in nodePositions)
        {
            sb.Append(np.x + ", ");
            sb.Append(np.y + ", ");
            sb.Append(np.z + ", ");
        }
        foreach(Instruction i in designInstructions)
        {
            sb.Append(i.toData() + ", ");
        }
        sb.Append(nodePositions.Count + ", ");
        sb.Append(designInstructions.Count);
        return sb.ToString();
    }
    public void ClearDNA()
    {
        nodePositions.Clear();
        designInstructions.Clear();
    }
    public void AddToPositions(Vector3 position)
    {
        nodePositions.Add(position);
    }
    public void AddToInstructions(Instruction x)
    {
        designInstructions.Add(x);
    }
}

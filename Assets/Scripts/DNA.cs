using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DNA
{
    public List<Vector3> nodePositions;
    public List<Instruction> designInstructions;

    public DNA()
    {
        nodePositions = new List<Vector3>();
        designInstructions = new List<Instruction>();
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
            Instruction inst = new Instruction(copy.designInstructions[i].baseNode, copy.designInstructions[i].targetNode, new Vector3(copy.designInstructions[i].GetSineFactors().x, copy.designInstructions[i].GetSineFactors().y, copy.designInstructions[i].GetSineFactors().z));
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

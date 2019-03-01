using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DNA
{
    List<Vector3> nodePositions;
    List<Instruction> designInstructions;

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
}

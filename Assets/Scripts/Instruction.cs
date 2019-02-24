using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction
{
    public Instruction() { }
    public Instruction(int baseN, int targetN, int rot, float A, float B, float C) {
        baseNode = baseN;
        targetNode = targetN;
        rotation = rot; // 0-180
        sineA = A;
        sineB = B;
        sineC = C;
    }
    int baseNode; // The starting node
    int targetNode; // the target node
    int rotation; // angle the joint is turned

    // Within the joint is a controller that runs off of the equation
    // sin(A*sin(Bx)+C) These factors are created as part of the instruction set
    float sineA;
    float sineB;
    float sineC;
    // To gene is used internally by the Brain for construction
    public string toGene()
    {
        return "" +
            baseNode.ToString() + " " +
            targetNode.ToString() + " " +
            rotation.ToString() + " " +
            sineA.ToString() + " " +
            sineB.ToString() + " " +
            sineC.ToString();

    }
    // To String is used externally for data analysis
    public string toData()
    {

        // For simplicity in making csv's later I'm making the Instruction csv when toStringing
        return "" +
            baseNode.ToString() + ", " +
            targetNode.ToString() + ", " +
            rotation.ToString() + ", " +
            sineA.ToString() + ", " +
            sineB.ToString() + ", " +
            sineC.ToString() + "";
    }
}

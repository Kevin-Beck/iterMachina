using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction
{
    public Instruction() { }
    public Instruction(int baseN, int targetN, Vector3 sine, float rot) {
        baseNode = baseN;
        targetNode = targetN;
        sineA = sine.x;
        sineB = sine.y;
        sineC = sine.z;
        rotation = rot;
    }
    public int baseNode; // The starting node
    public int targetNode; // the target node

    // Within the joint is a controller that runs off of the equation
    // sin(A*sin(Bx)+C) These factors are created as part of the instruction set
    float sineA;
    float sineB;
    float sineC;

    float rotation;
    
    public void SetRotation(float rot)
    {
        rotation = rot;
    }
    public float GetRotation()
    {
        return rotation;
    }
    // To gene is used internally by the Brain for construction
    public Vector3 GetSineFactors()
    {
        return new Vector3(sineA, sineB, sineC);
    }
    public void SetSineFactors(Vector3 value)
    {
        sineA = value.x;
        sineB = value.y;
        sineC = value.z;
    }

    // To String is used externally for data analysis
    public string toData()
    {

        // For simplicity in making csv's later I'm making the Instruction csv when toStringing
        return "" +
            baseNode.ToString() + ", " +
            targetNode.ToString() + ", " +
            sineA.ToString() + ", " +
            sineB.ToString() + ", " +
            sineC.ToString() + "";
    }
}

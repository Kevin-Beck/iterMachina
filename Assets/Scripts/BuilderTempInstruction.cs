using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderTempInstruction
{
    public GameObject baseNode;
    public GameObject tailNode;
    public GameObject joint;

    public BuilderTempInstruction(GameObject baseN, GameObject tailN, GameObject jointO)
    {
        baseNode = baseN;
        tailNode = tailN;
        joint = jointO;
    }    
}

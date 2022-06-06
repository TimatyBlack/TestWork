using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private bool canBeTaken = true;

    public bool Take()
    {
        if(canBeTaken)
        {
            canBeTaken = false;
            return true;
        }
        return false;
    }

    
}
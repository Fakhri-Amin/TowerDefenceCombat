using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimitModifier : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}

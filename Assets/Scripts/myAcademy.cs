using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class myAcademy : Academy
{
    public override void InitializeAcademy()
    {
        Monitor.SetActive(true);
    }
}

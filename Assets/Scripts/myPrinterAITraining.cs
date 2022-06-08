using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPrinterAITraining : MonoBehaviour
{
    GameObject[] crowd;
    float steps;
    int stepsLimit = 1000; //50000
    bool oneTimeAction1 = true;
    float[] combinedStats = new float[5];
    float cCenter;
    float cPredator;

    void FixedUpdate()
    {
        if (GameObject.FindGameObjectsWithTag("Prey").Length == 15 && oneTimeAction1)
        {
            AddCrowd();
            oneTimeAction1 = false;

            Debug.Log("crowd added");
        }

        if (!oneTimeAction1)
        {
            steps += 0.1f;

            if (steps >= stepsLimit)
            {
                AddUpAngles();
                AddUpExtraMetrix();
                CombineAllAgentsStatsAndAverage();
                PrintAll();
                steps = 0;
            }
        }
    }

    void AddCrowd()
    {
        crowd = GameObject.FindGameObjectsWithTag("Prey");
    }

    void AddUpAngles()
    {
        for (int i = 0; i < crowd.Length; i++)
        {
            crowd[i].GetComponent<myPreyAgent>().agentData[0] =
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myPreyAgent>().Cohesion());
            crowd[i].GetComponent<myPreyAgent>().agentData[1] =
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myPreyAgent>().Alignment());
            crowd[i].GetComponent<myPreyAgent>().agentData[2] =
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myPreyAgent>().Avoidance());
            crowd[i].GetComponent<myPreyAgent>().agentData[3] =
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myPreyAgent>().Forage());
            crowd[i].GetComponent<myPreyAgent>().agentData[4] =
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myPreyAgent>().AvoidPredator());
        }
    }

    void CombineAllAgentsStatsAndAverage()
    {
        for (int i = 0; i < crowd.Length; i++)
        {
            for (int j = 0; j < crowd[i].GetComponent<myPreyAgent>().agentData.Length; j++)
            {
                combinedStats[j] += crowd[i].GetComponent<myPreyAgent>().agentData[j];
            }
        }

        for (int i = 0; i < combinedStats.Length; i++)
        {
            combinedStats[i] /= crowd.Length;
        }
    }

    void AddUpExtraMetrix()
    {
        Vector3 cohCenter = new Vector3();
        float combinedDistFromCCenter = 0;
        float combinedDistFromPredator = 0;

        for (int i = 0; i < crowd.Length; i++)
        {
            cohCenter += crowd[i].transform.position;
        }
        cohCenter /= crowd.Length;

        for (int i = 0; i < crowd.Length; i++)
        {
            combinedDistFromCCenter += Vector3.Distance(crowd[i].transform.position, cohCenter);
            combinedDistFromPredator += Vector3.Distance(crowd[i].transform.position, FindObjectOfType<myPredator>().transform.position);
        }
        cCenter = combinedDistFromCCenter / crowd.Length;
        cPredator = combinedDistFromPredator / crowd.Length;
    }

    void PrintAll()
    {
        Debug.Log("\n"
        + combinedStats[0] + "\n"
        + combinedStats[1] + "\n"
        + combinedStats[2] + "\n"
        + combinedStats[3] + "\n"
        + combinedStats[4] + "\n"
        + cCenter + "\n"
        + cPredator
        );
    }
}

//Debug.Log("steps: " + FindObjectOfType<myAcademy>().GetComponent<myAcademy>().GetTotalStepCount() + ", frames: " + frames);

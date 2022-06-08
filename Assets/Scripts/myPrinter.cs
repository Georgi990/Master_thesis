using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myPrinter : MonoBehaviour
{
    GameObject[] crowd;
    int frames;
    int counter;
    int framesLimit = 100;
    int counterLimit = 5;
    bool oneTimeAction = true;
    bool oneTimeAction1 = true;
    float[] combinedStats = new float[5];

    float[] combinedExtraStats = new float[2];

    float cCenter;
    float cPredator;

    void FixedUpdate()
    {
        if (GameObject.FindGameObjectsWithTag("CrowdMember").Length == 15 && oneTimeAction1)
        {
            AddCrowd();
            oneTimeAction1 = false;

            Debug.Log("crowd added");
        }

        if (!oneTimeAction1)
        {
            frames++;

            if (counter < counterLimit && frames == framesLimit)
            {
                AddUpAngles();
                AddUpExtraMetrix();
                frames = 0;
                counter++;
            }

            if (counter < counterLimit)
            {
                Debug.Log("frames: " + frames + " , " + "counter: " + counter);
            }

            if (counter == counterLimit && oneTimeAction)
            {
                AverageSums();
                CombineExtraMetrixAndAverage();
                CombineAllAgentsStatsAndAverage();
                PrintAll();
                oneTimeAction = false;
            }
        }
    }

    void AddCrowd()
    {
        crowd = GameObject.FindGameObjectsWithTag("CrowdMember");
    }

    void AddUpAngles()
    {
        for (int i = 0; i < crowd.Length; i++)
        {
            crowd[i].GetComponent<myCrowdMember>().agentData[0] += 
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myCrowdMember>().Cohesion());
            crowd[i].GetComponent<myCrowdMember>().agentData[1] +=
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myCrowdMember>().Alignment());
            crowd[i].GetComponent<myCrowdMember>().agentData[2] +=
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myCrowdMember>().Avoidance());
            crowd[i].GetComponent<myCrowdMember>().agentData[3] +=
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myCrowdMember>().Forage());
            crowd[i].GetComponent<myCrowdMember>().agentData[4] +=
                Vector3.Angle(crowd[i].transform.forward, crowd[i].GetComponent<myCrowdMember>().AvoidPredator());
        }
    }

    void AverageSums()
    {
        for (int i = 0; i < crowd.Length; i++)
        {
            for (int j = 0; j < crowd[i].GetComponent<myCrowdMember>().agentData.Length; j++)
            {
                crowd[i].GetComponent<myCrowdMember>().agentData[j] /= counterLimit;
            }
        }
    }

    void CombineAllAgentsStatsAndAverage()
    {
        for (int i = 0; i < crowd.Length; i++)
        {
            for (int j = 0; j < crowd[i].GetComponent<myCrowdMember>().agentData.Length; j++)
            {
                combinedStats[j] += crowd[i].GetComponent<myCrowdMember>().agentData[j];
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
            combinedDistFromPredator += Vector3.Distance(crowd[i].transform.position, FindObjectOfType<FlockPredator>().transform.position);
        }
        cCenter += combinedDistFromCCenter / crowd.Length;
        cPredator += combinedDistFromPredator / crowd.Length;
    }

    void CombineExtraMetrixAndAverage()
    {
        combinedExtraStats[0] = cCenter / counterLimit;
        combinedExtraStats[1] = cPredator / counterLimit;
    }

    void PrintAll()
    {
        Debug.Log("\n"
        + combinedStats[0] + "\n"
        + combinedStats[1] + "\n"
        + combinedStats[2] + "\n"
        + combinedStats[3] + "\n"
        + combinedStats[4] + "\n"
        + combinedExtraStats[0] + "\n"
        + combinedExtraStats[1]
        );
    }
}

//OLD PrintAll():
//void PrintAll()
//{
//    for (int i = 0; i < crowd.Length; i++)
//    {
//        Debug.Log(crowd[i].name + "\n"
//    + "coh angle: " + crowd[i].GetComponent<myCrowdMember>().agentData[0] + "\n"
//    + "align angle: " + crowd[i].GetComponent<myCrowdMember>().agentData[1] + "\n"
//    + "avoid angle: " + crowd[i].GetComponent<myCrowdMember>().agentData[2] + "\n"
//    + "forage angle: " + crowd[i].GetComponent<myCrowdMember>().agentData[3] + "\n"
//    + "avoid pred angle: " + crowd[i].GetComponent<myCrowdMember>().agentData[4]
//    );
//    }
//}

//if (Input.GetKeyDown(KeyCode.Space))
//{
//    Debug.Log(this.name + "\n"
//        + "coh angle: " + Vector3.Angle(this.transform.forward, Cohesion()) + "\n"
//        + "align angle: " + Vector3.Angle(this.transform.forward, Alignment()) + "\n"
//        + "avoid angle: " + Vector3.Angle(this.transform.forward, Avoidance()) + "\n"
//        + "forage angle: " + Vector3.Angle(this.transform.forward, Forage()) + "\n"
//        + "avoid pred angle: " + Vector3.Angle(this.transform.forward, AvoidPredator())
//        );
//}
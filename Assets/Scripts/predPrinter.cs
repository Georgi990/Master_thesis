using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predPrinter : MonoBehaviour
{
    int frames;
    int counter;
    int framesLimit = 100;
    int counterLimit = 100;
    bool oneTimeAction = true;

    float memmory;
    int timesAte;

    void FixedUpdate()
    {
        frames++;

        if (counter < counterLimit && frames == framesLimit)
        {
            AddUpMemmory();
            frames = 0;
            counter++;
        }

        if (counter < counterLimit)
        {
            Debug.Log("frames: " + frames + " , " + "counter: " + counter);
        }

        if (counter == counterLimit && oneTimeAction)
        {
            AverageMemmory();
            PrintMemmory();
            PrintEaten();
            oneTimeAction = false;
        }
    }

    void AddUpMemmory()
    {
        //memmory += FindObjectOfType<FlockPredator>().GetComponent<FlockPredator>().radiusMemmory.Count;
        memmory += FindObjectOfType<myPredator>().GetComponent<myPredator>().radiusMemory.Count;
    }

    void AverageMemmory()
    {
        memmory /= counterLimit;
    }

    void PrintMemmory()
    {
        Debug.Log("Memmory: " + memmory);
    }

    void PrintEaten()
    {
        //foreach (GameObject crowdMember in GameObject.FindGameObjectsWithTag("CrowdMember"))
        //{
        //    timesAte += crowdMember.GetComponent<myCrowdMember>().timesCaught;
        //}

        foreach (GameObject crowdMember in GameObject.FindGameObjectsWithTag("Prey"))
        {
            timesAte += crowdMember.GetComponent<myPreyAgent>().timesCaught;
        }

        Debug.Log("Times ate: " + timesAte);
    }
}
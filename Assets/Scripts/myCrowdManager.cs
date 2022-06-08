using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCrowdManager : MonoBehaviour
{
    public myCrowdMember agentPrefab;
    public int startingCount;
    public float yPos = 0;
    public float agentDensity = 15f;
    public static List<myCrowdMember> agents = new List<myCrowdMember>();
    public GameObject floorHolder;
    public static GameObject floor;

    void Start()
    {
        floor = floorHolder;

        for (int i = 0; i < startingCount; i++)
        {
            myCrowdMember newAgent = Instantiate(
                 agentPrefab,
                 new Vector3(Random.insideUnitCircle.x * agentDensity, yPos, Random.insideUnitCircle.y * agentDensity),
                 Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                 this.transform
                 );
            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
        }
    }
}

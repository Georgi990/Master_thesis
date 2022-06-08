using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using MLAgents;

//for angles calculations:
using System.Linq;

public class myPreyAgent : Agent
{
    public ThirdPersonCharacter character;
    public float speed;

    public Transform floor;
    Collider floorCol;
    float spawnAreaPadder = 2;

    public Transform predator;
    Rigidbody predatorRBody;
    public LayerMask wallMask;
    public LayerMask foodMask;

    public int timesCaught;
    bool isFrompredator;

    void Start()
    {
        floorCol = floor.GetComponent<Collider>();
        predatorRBody = predator.GetComponent<Rigidbody>();
        myLastPosition = this.transform.position;
    }

    public override void AgentReset()
    {
        this.transform.position = new Vector3(
            Random.Range(floorCol.bounds.min.x + spawnAreaPadder,
            floorCol.bounds.max.x - spawnAreaPadder),
            0f,
            Random.Range(floorCol.bounds.min.z + spawnAreaPadder,
            floorCol.bounds.max.z - spawnAreaPadder)
            );
        this.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));

        //for predator printer
        if (isFrompredator)
        {
            timesCaught++;
            isFrompredator = false;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(this.transform.position); //8 observations before going global
        AddVectorObs(GetMyVelocity().x);
        AddVectorObs(GetMyVelocity().z);
        myLastPosition = this.transform.position;
        AddVectorObs(this.transform.forward.normalized);

        //predator vision adds up 9 vector obs (for brain 1503Night3) (for a total of 17)
        /*AddVectorObs(predator.transform.position);
        AddVectorObs(predatorRBody.velocity.x);
        AddVectorObs(predatorRBody.velocity.z);
        AddVectorObs(Vector3.Distance(this.transform.position, predator.transform.position));
        AddVectorObs(predator.transform.forward.normalized);*/
    }

    public override void AgentAction(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        character.Move(Vector3.ClampMagnitude(controlSignal, 1.0f) * speed * Time.deltaTime, false, false);

        CheckIfCollidingWithFood();

        /*if (Physics.CheckSphere(this.transform.position, this.GetComponent<CapsuleCollider>().radius + 0.1f, wallMask))
        {
            SetReward(-0.5f);
            Done();
        }*/

        if (Vector3.Distance(this.transform.position, predator.position) <= 0.65f)
        {
            isFrompredator = true;
            SetReward(-1.0f);
            Done();
        }

        Monitor.Log("reward", GetReward());
        FellOffThePlatform();

        ColorByGrouping();
        DrawAngles();
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    void FellOffThePlatform()
    {
        if (this.transform.position.y <= -20f)
        {
            this.transform.position = Vector3.zero;
        }
    }

    Vector3 myLastPosition;
    Vector3 GetMyVelocity()
    {
        return (myLastPosition - this.transform.position) / Time.deltaTime;
    }

    void CheckIfCollidingWithFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.4f, foodMask);
        if (hitColliders.Length != 0)
        {
            SetReward(0.5f);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                hitColliders[i].gameObject.transform.position = GetRandomPos();
            }
        }
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(floorCol.bounds.min.x + spawnAreaPadder, floorCol.bounds.max.x - spawnAreaPadder),
            floor.position.y + 0.5f,
            Random.Range(floorCol.bounds.min.z + spawnAreaPadder, floorCol.bounds.max.z - spawnAreaPadder));
    }


    //------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------
    public float[] agentData = new float[5];

    float neighborRadius = 3f;

    List<myPreyAgent> GetNeighbors()
    {
        List<myPreyAgent> neighbors = new List<myPreyAgent>();

        foreach (myPreyAgent a in FindObjectsOfType<myPreyAgent>())
        {
            float distance = Vector3.Distance(this.transform.position, a.transform.position);
            if (distance <= neighborRadius && a != this)
            {
                neighbors.Add(a);
            }
        }
        return neighbors;
    }

    public Vector3 Cohesion()
    {
        Vector3 currentVelocity = new Vector3();
        float agentSmoothTime = 0.5f;
        Vector3 cohMove = Vector3.zero;
        if (GetNeighbors().Count == 0)
        {
            return Vector3.zero;
        }
        foreach (myPreyAgent a in GetNeighbors())
        {
            cohMove += a.transform.position;
        }
        cohMove /= GetNeighbors().Count;
        cohMove -= this.transform.position;
        cohMove = Vector3.SmoothDamp(this.transform.forward, cohMove, ref currentVelocity, agentSmoothTime);
        return cohMove;
    }

    float avoidanceRadius = 2f;

    public Vector3 Avoidance()
    {
        Vector3 avoidMove = Vector3.zero;
        int nAvoid = 0;
        if (GetNeighbors().Count == 0)
        {
            return Vector3.zero;
        }
        foreach (myPreyAgent a in GetNeighbors())
        {
            float distance = Vector3.Distance(this.transform.position, a.transform.position);
            if (distance < avoidanceRadius)
            {
                avoidMove += this.transform.position - a.transform.position;
                nAvoid++;
            }
        }
        if (nAvoid > 0)
        {
            avoidMove /= nAvoid;
        }
        return avoidMove;
    }

    public Vector3 Alignment()
    {
        Vector3 alignMove = Vector3.zero;
        if (GetNeighbors().Count == 0)
        {
            return this.transform.forward;
        }
        foreach (myPreyAgent a in GetNeighbors())
        {
            alignMove += a.transform.forward;
        }
        alignMove /= GetNeighbors().Count;
        return alignMove;
    }

    public Vector3 AvoidPredator()
    {
        if (Vector3.Distance(this.transform.position, predator.transform.position) <= avoidanceRadius + 1)
            return this.transform.position - predator.transform.position;
        else return Vector3.zero;
    }

    public Vector3 Forage()
    {
        return (GameObject.FindGameObjectsWithTag("Food")
                .OrderBy(t => (t.transform.position - this.transform.position).sqrMagnitude).FirstOrDefault()
                .transform.position - this.transform.position).normalized;
    }

    void DrawAngles()
    {
        Vector3 startPoint = this.transform.position;
        startPoint.y = 1f;
        Debug.DrawRay(startPoint, this.transform.forward * 4, Color.cyan, 0, false);
    }

    void ColorByGrouping()
    {
        this.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.magenta, GetNeighbors().Count / 4f);
    }
}

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Debug.Log(this.name + "\n"
//        + "neighbours :" + GetNeighbors().Count + "\n"
//        + "cohesion :" + Cohesion() + "\n"
//        + "alignment :" + Alignment() + "\n"
//        + "avoidance :" + Avoidance() + "\n"
//        + "forage :" + Forage() + "\n"
//        + "pred avoidance :" + AvoidPredator() + "\n"
//        );
//}

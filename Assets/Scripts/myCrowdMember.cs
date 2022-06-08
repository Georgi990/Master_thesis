using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Linq;

public class myCrowdMember : MonoBehaviour
{
    float neighborRadius = 3f;
    float avoidanceRadius = 2f;
    float maxSpeed = 100f; //5f def //150f for variation
    float driveFactor = 40f; //10f def //40f for variation
    float cohesionWeight = 1f;
    float avoidanceWeight = 3.5f;
    float alignmentWeight = 1f;
    
    Collider floorCol;
    public ThirdPersonCharacter character;

    float squareMaxSpeed;

    float raySphereRadius = .27f;
    float obstCollisionAvoidDst = 5;
    public LayerMask obstacleMask;
    float obstacleAvoidanceForceWeight = 3f;

    //float crudeWallRepelWeight = 1f;
    //float centeringWeight = 0.1f;

    GameObject predator;
    float predatorAvoidWeight = 3f;
    float spawnAreaPadder = 2;
    float forageWeight = 1f;
    public LayerMask foodMask;

    public float[] agentData = new float[5];

    void Start()
    {
        floorCol = myCrowdManager.floor.GetComponent<Collider>();
        squareMaxSpeed = maxSpeed * maxSpeed;

        predator = GameObject.FindGameObjectWithTag("Predator");
    }

    void FixedUpdate()
    {
        Move();
        ifCoughtByPredator();
        CheckIfCollidingWithFood();
        drawAngles();
        ColorByGrouping();
        ifFellOffThePlatform();
    }

    List<myCrowdMember> GetNeighbors()
    {
        List<myCrowdMember> neighbors = new List<myCrowdMember>();

        foreach (myCrowdMember a in myCrowdManager.agents)
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
        foreach (myCrowdMember a in GetNeighbors())
        {
            cohMove += a.transform.position;
        }
        cohMove /= GetNeighbors().Count;
        cohMove -= this.transform.position;
        cohMove = Vector3.SmoothDamp(this.transform.forward, cohMove, ref currentVelocity, agentSmoothTime);
        return cohMove;
    }

    public Vector3 Avoidance()
    {
        Vector3 avoidMove = Vector3.zero;
        int nAvoid = 0;
        if (GetNeighbors().Count == 0)
        {
            return Vector3.zero;
        }
        foreach (myCrowdMember a in GetNeighbors())
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
        foreach (myCrowdMember a in GetNeighbors())
        {
            alignMove += a.transform.forward;
        }
        alignMove /= GetNeighbors().Count;
        return alignMove;
    }

    Vector3 ObstacleAvoidanceForce()
    {
        Vector3[] rayDirections = FibonacciRays.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = this.transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(this.transform.position, dir);
            if (!Physics.SphereCast(ray, raySphereRadius, obstCollisionAvoidDst, obstacleMask))
            {
                return dir;
            }
        }
        //return this.transform.forward;
        return Vector3.zero;
    }

    public Vector3 AvoidPredator()
    {
        if (Vector3.Distance(this.transform.position, predator.transform.position) <= avoidanceRadius+1)
            return this.transform.position - predator.transform.position;
        else return Vector3.zero;
    }

    public Vector3 Forage()
    {
        return (GameObject.FindGameObjectsWithTag("Food")
                .OrderBy(t => (t.transform.position - this.transform.position).sqrMagnitude).FirstOrDefault()
                .transform.position - this.transform.position).normalized;
    }

    public int timesCaught;
    void ifCoughtByPredator()
    {
        if (Vector3.Distance(this.transform.position, predator.transform.position) <= 0.65f)
        {
            this.transform.position = new Vector3(
            Random.Range(floorCol.bounds.min.x + spawnAreaPadder,
            floorCol.bounds.max.x - spawnAreaPadder),
            0f,
            Random.Range(floorCol.bounds.min.z + spawnAreaPadder,
            floorCol.bounds.max.z - spawnAreaPadder)
            );
            //for pred printer:
            timesCaught++;
        }
    }

    void CheckIfCollidingWithFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 0.4f, foodMask);
        if (hitColliders.Length != 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                hitColliders[i].gameObject.transform.position = GetRandomPos();
            }
        }
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(floorCol.bounds.min.x + spawnAreaPadder, floorCol.bounds.max.x - spawnAreaPadder),
            myCrowdManager.floor.transform.position.y + 0.5f,
            Random.Range(floorCol.bounds.min.z + spawnAreaPadder, floorCol.bounds.max.z - spawnAreaPadder));
    }

    void Move()
    {
        Vector3 move = Vector3.zero;

        int numOfBehavs = 6;
        Vector3[] behaviors = new Vector3[numOfBehavs];
        behaviors[0] = Cohesion();
        behaviors[1] = Alignment();
        behaviors[2] = Avoidance();
        behaviors[3] = ObstacleAvoidanceForce();
        behaviors[4] = AvoidPredator();
        behaviors[5] = Forage();

        float[] weights = new float[numOfBehavs];
        weights[0] = cohesionWeight;
        weights[1] = alignmentWeight;
        weights[2] = avoidanceWeight;
        weights[3] = obstacleAvoidanceForceWeight;
        weights[4] = predatorAvoidWeight;
        weights[5] = forageWeight;

        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i] * weights[i];
            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }
                move += partialMove;
            }
        }
        move *= driveFactor;
        if (move.sqrMagnitude > squareMaxSpeed)
        {
            move = move.normalized * maxSpeed;
        }

        character.Move(move * Time.deltaTime, false, false);
    }

    void ifFellOffThePlatform()
    {
        if (this.transform.position.y <= -20f)
        {
            this.transform.position = Vector3.zero;
        }
    }

    void drawAngles()
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

//private void OnDrawGizmosSelected()
//{
//    Gizmos.color = Color.red;
//    Debug.DrawLine(transform.position, transform.position + ObstacleAvoidanceForce() * obstCollisionAvoidDst);
//    Gizmos.DrawWireSphere(transform.position + ObstacleAvoidanceForce() * obstCollisionAvoidDst, raySphereRadius);
//}

//Vector3 crudeWallRepelForce()
//{
//    Vector3 crudeWallRepel = Vector3.zero;
//    if (Vector3.Distance(this.transform.position, Vector3.zero) >= floorCollider.bounds.size.x / 2 - 1)
//    {
//        crudeWallRepel = floorCollider.bounds.center - this.transform.position;
//        crudeWallRepel.y = 0;
//    }

//    return crudeWallRepel;
//}

//Vector3 centeringForce()
//{
//    Vector3 center = new Vector3();
//    float radius = 15f;
//    float t;
//    Vector3 centerOffset = center - this.transform.position;
//    t = centerOffset.magnitude / radius;
//    if (t < 0.9f)
//    {
//        return Vector3.zero;
//    }
//    centerOffset.y = 0;
//    return centerOffset * t * t;
//}

//void teleportToOtherSide()
//{
//    if (this.transform.position.x >= floorCollider.bounds.size.x / 2 - 0.5 ||
//        this.transform.position.x <= (floorCollider.bounds.size.x / 2 - 0.5) * -1)
//    {
//        Vector3 pos = this.transform.position;
//        pos.x *= -1;
//        pos.x *= .9f;
//        this.transform.position = pos;
//    }

//    if (this.transform.position.z >= floorCollider.bounds.size.z / 2 - 0.5 ||
//        this.transform.position.z <= (floorCollider.bounds.size.z / 2 - 0.5) * -1)
//    {
//        Vector3 pos = this.transform.position;
//        pos.z *= -1;
//        pos.z *= .9f;
//        this.transform.position = pos;
//    }
//}

//Debug.Log(this.name + " cohesion: " + Cohesion());
//Debug.Log(this.name + " alignment: " + Alignment());
//Debug.Log(this.name + " avoidance: " + Avoidance());
//Debug.Log("");

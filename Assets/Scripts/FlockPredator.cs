using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Linq;

public class FlockPredator : MonoBehaviour
{
    public ThirdPersonCharacter character;
    public float speed = 60;
    public float predtorVisionRadius = 7.5f;

    bool oneTimeActionIsDone;

    void Start()
    {
        this.transform.GetChild(1).gameObject.transform.localScale = new Vector3(predtorVisionRadius * 2, 0.007f, predtorVisionRadius * 2);
    }

    private void FixedUpdate()
    {
        character.Move(MoveDirection() * speed * Time.deltaTime, false, false);

        FellOffThePlatform();

        if (!oneTimeActionIsDone)
        {
            foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
            {
                Physics.IgnoreCollision(food.GetComponent<Collider>(), this.GetComponent<Collider>());
            }
            oneTimeActionIsDone = true;
        }
        FailSafe();

        drawAngles();
    }

    public List<Transform> radiusMemmory = new List<Transform>();
    Vector3 MoveDirection()
    {
        List<Transform> preyWithinRadius = new List<Transform>();

        foreach (GameObject prey in GameObject.FindGameObjectsWithTag("CrowdMember"))
        {
            if (Vector3.Distance(this.transform.position, prey.transform.position) <= predtorVisionRadius)
            {
                preyWithinRadius.Add(prey.transform);
                if (!radiusMemmory.Contains(prey.transform)) radiusMemmory.Add(prey.transform);
            }
            else
            {
                if (radiusMemmory.Contains(prey.transform)) radiusMemmory.Remove(prey.transform);
            }
        }

        if (preyWithinRadius.Count < 2)
        {
            radiusMemmory.Clear();
            return (GameObject.FindGameObjectsWithTag("CrowdMember")
                .OrderBy(t => (t.transform.position - this.transform.position).sqrMagnitude).FirstOrDefault()
                .transform.position - this.transform.position).normalized;
        }
        else
        {
            if (radiusMemmory.Count > 0) return (radiusMemmory[radiusMemmory.Count - 1].position - this.transform.position).normalized;
            else return Vector3.zero;
        }
    }

    void FellOffThePlatform()
    {
        if (this.transform.position.y <= -20f)
        {
            this.transform.position = Vector3.zero;
        }
    }

    void FailSafe()
    {
        if (radiusMemmory.Count > 20) radiusMemmory.Clear();
    }

    void drawAngles()
    {
        Vector3 startPoint = this.transform.position;
        Color orange = new Color(255, 165, 0);
        startPoint.y = 1f;
        Debug.DrawRay(startPoint, this.transform.forward * 4, orange, 0, false);
    }
}

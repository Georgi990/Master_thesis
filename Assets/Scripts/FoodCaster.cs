using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCaster : MonoBehaviour
{
    public int numberOfPieces = 10;
    public GameObject foodPrefab;
    public Transform floor;
    MeshCollider floorCol;
    List<Vector3> usedInitPoses;
    float spawnAreaPadder = 2;

    void Start()
    {
        floorCol = floor.GetComponent<MeshCollider>();
        usedInitPoses = new List<Vector3>();

        for (int i = 0; i < numberOfPieces; i++)
        {
            Vector3 tmp = GetRandomPos();

            GameObject Food = Instantiate(
                 foodPrefab,
                 tmp,
                 Quaternion.identity
                 );

            usedInitPoses.Add(tmp);
        }

        usedInitPoses.Clear();
    }

    Vector3 GetRandomPos()
    {
        Vector3 tmpVector = new Vector3(Random.Range(floorCol.bounds.min.x + spawnAreaPadder, floorCol.bounds.max.x - spawnAreaPadder),
            floor.position.y + foodPrefab.transform.localScale.y / 2,
            Random.Range(floorCol.bounds.min.z + spawnAreaPadder, floorCol.bounds.max.z - spawnAreaPadder));

        if (usedInitPoses.Contains(tmpVector)) return GetRandomPos();
        return tmpVector;
    }
}

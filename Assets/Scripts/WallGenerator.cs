using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public Transform floorParent;
    public float gridSize = 1f;

    void Start()
    {
        GenerateWalls();
    }

    void GenerateWalls()
    {
        foreach (Transform floor in floorParent)
        {
            Vector3 pos = floor.position;

            CheckDirection(pos + new Vector3(gridSize, 0, 0));
            CheckDirection(pos + new Vector3(-gridSize, 0, 0));
            CheckDirection(pos + new Vector3(0, 0, gridSize));
            CheckDirection(pos + new Vector3(0, 0, -gridSize));
        }
    }

    void CheckDirection(Vector3 checkPos)
    {
        Collider[] hits = Physics.OverlapSphere(checkPos, 0.2f);

        if (hits.Length == 0)
        {
            Instantiate(wallPrefab, checkPos, Quaternion.identity);
        }
    }
}
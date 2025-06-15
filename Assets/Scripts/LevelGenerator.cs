using System;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelParts;
    [SerializeField] private Vector3 nextPartPosition;
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDespawn;
    [SerializeField] private Transform player;


    void Update()
    {
        deletePlatform();
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        while (Vector2.Distance(player.position, nextPartPosition) < distanceToSpawn)
        {
            Transform part = levelParts[UnityEngine.Random.Range(0, levelParts.Length)];
            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("Start").position.x, 0);
            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);
            nextPartPosition = newPart.Find("End").position;
        }
    }

    private void deletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);
            if (Vector2.Distance(player.position, partToDelete.position) > distanceToDespawn)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPos;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;

    public float maxOpDist;

    float opDist;
    float opCooldown;
    public float opCooldownDur;





    void Start()
    {
        playerLastPos = player.transform.position;
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPos;
        playerLastPos = player.transform.position;

        string directionName = getDirectionName(moveDir);
        CheckAndSpawnChunk(directionName);

        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }

        if (directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }

        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }

        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }

    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);

        }
    }

    string getDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            //Moving horizontally more than vertically
            if (direction.y > 0.5f)
            {
                //moving upward
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                return direction.x < 0 ? "Right Down" : "Left Down";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //moving vertically more than horizontally
            if (direction.x > 0.5f)
            {
                return direction.y > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.x < -0.5f)
            {
                return direction.y < 0 ? "Right Down" : "Left Down";
            }
            else
            {
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPos)
    {
        int rand = UnityEngine.Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPos, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        opCooldown -= Time.deltaTime;
        if (opCooldown <= 0f)
        {
            opCooldown = opCooldownDur;
        }
        else
        {
            return;
        }
        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformTypes
{
    StaticPlatform,
    CrumblingPlatform,
    MovingPlatform
}

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] GameObject staticPlatformPrefab;
    [SerializeField] GameObject crumblingPlatformPrefab;
    [SerializeField] GameObject movingPlatformPrefab;

    [SerializeField]
    GameObject[] platformTypePrefabs;

    [SerializeField] int startingPlatforms;
    [SerializeField] int checkpointPlatform;
    [SerializeField] float checkpointPlatformSize;

    [Space(20)]
    [SerializeField] bool shouldIncreaseDifficulty;

    [Header("Platform's position and transform related variables")]
    [SerializeField] Vector2 platformStartPosition;
    [SerializeField] float distanceBetweenPlatforms;
    [SerializeField] float platformStartSize;
    [SerializeField] float platformEndSize;
    [SerializeField] float platformPosX;

    float minDistanceBetweenPlatforms;
    float minPlatformSize;
    float maxPlatformSize;
    float chanceOfCrumblingPlatform = 0.2f;
    float chanceOfMovingPlatform = 0.5f;

    int nextPlatformIndex;
    public int NextPlatformIndex { get { return nextPlatformIndex; } }

    bool canSpawnCrumblingPlatforms;
    bool canSpawnMovingPlatforms;

    private void Awake()
    {
        ObjectPool.Clear();
    }

    private void Start()
    {
        minDistanceBetweenPlatforms = distanceBetweenPlatforms;
        minPlatformSize = platformEndSize - 1.5f;
        maxPlatformSize = platformEndSize;

        for (int i = 0; i < platformTypePrefabs.Length; i++)
        {
            for(int j = 0; j < startingPlatforms; j++)
            {
                var platform = Instantiate(platformTypePrefabs[i], transform);
                ObjectPool.Add<PlatformStatic>(platform.GetComponent<PlatformStatic>());
            }
        }
        
        for (int i = 0; i < startingPlatforms; i++)
        {
            PlacePlatform();
        }

        //ObstacleManager.instance.SpawnObstacles();
    }
    public void PlacePlatform()
    {
        float platformProb = UnityEngine.Random.value;

        Platform platformObj = GetPlatformType(platformProb);
        Type type = platformObj.GetType(); ;

        var platform = platformObj.transform;
        platform.gameObject.SetActive(true);

        SetPlatformSize(platform);
        SetPlatformPosition(platform);
        SetPlatformProperties(type, platform);

        //ObstacleManager.instance.PlaceObstacle(platform, UnityEngine.Random.Range(-0.1f, 0.1f));

        if (shouldIncreaseDifficulty)
            ChangePlatforms();
    }

    private Platform GetPlatformType(float platformProb)
    {
        return ObjectPool.Get<PlatformStatic>();

        /*Platform platformObj;
        if (canSpawnCrumblingPlatforms && platformProb < chanceOfCrumblingPlatform && ObjectPool.GetActiveInPool<PlatformCrumbling>() < startingPlatforms)
        {
            platformObj = ObjectPool.Get<PlatformCrumbling>();
        }
        else if (canSpawnMovingPlatforms && platformProb < chanceOfMovingPlatform && ObjectPool.GetActiveInPool<PlatformMoving>() < startingPlatforms)
        {
            platformObj = ObjectPool.Get<PlatformMoving>();
        }
        else if (ObjectPool.GetActiveInPool<PlatformStatic>() < startingPlatforms)
        {
            platformObj = ObjectPool.Get<PlatformStatic>();
        }
        else
        {
            platformObj = ObjectPool.Get<Platform>();
        }

        return platformObj;*/
    }

    private void SetPlatformPosition(Transform platform)
    {
        float distance;
        //if (UnityEngine.Random.value < 0.65f)
        //{
        //    distance = UnityEngine.Random.Range(minDistanceBetweenPlatforms - minDistanceBetweenPlatforms / 3, minDistanceBetweenPlatforms + minDistanceBetweenPlatforms / 3);
        //}
        //else
        //{
        //    distance = UnityEngine.Random.Range(minDistanceBetweenPlatforms, distanceBetweenPlatforms);
        //}

        var xPos = UnityEngine.Random.Range(-platformPosX, platformPosX);
        if (Mathf.Abs(platformStartPosition.x - xPos) < 1f)
        {
            distance = UnityEngine.Random.Range(distanceBetweenPlatforms, distanceBetweenPlatforms + 0.5f);
        }
        else
        {
            distance = UnityEngine.Random.Range(0.15f, distanceBetweenPlatforms);
        }
        platform.localPosition = new Vector2(xPos, platformStartPosition.y + distance);
        platformStartPosition = platform.position;
    }

    private void SetPlatformSize(Transform platform)
    {
        if (checkpointPlatform != -1 && (nextPlatformIndex % checkpointPlatform) - (checkpointPlatform - 1) == 0)
        {
            platform.localScale = new Vector2(checkpointPlatformSize, platform.localScale.y);
        }
        else
        {
            var scale = UnityEngine.Random.Range(minPlatformSize, maxPlatformSize);
            platform.localScale = new Vector2(scale, 5);
        }
    }

    private void SetPlatformProperties(Type type, Transform platform)
    {
        nextPlatformIndex++;
        platform.gameObject.name = $"{type} {nextPlatformIndex}";
        Debug.Log(platform.name, platform.gameObject);

        var platformScript = platform.GetComponent<Platform>();
        if (platformScript != null)
        {
            platformScript.Index = nextPlatformIndex;
            platformScript.Type = type;
        }
        else
        {
            Debug.Log(":/");
        }
    }

    void ChangePlatforms()
    {
        if(nextPlatformIndex % 50 == 0)
        {
            minPlatformSize = (minPlatformSize < platformStartSize) ? platformStartSize : minPlatformSize - 0.5f;
            maxPlatformSize = (maxPlatformSize < platformStartSize) ? platformStartSize : maxPlatformSize - 0.25f;
            platformPosX = (platformPosX >= 4) ? 4 : platformPosX + 0.25f;
            distanceBetweenPlatforms = (distanceBetweenPlatforms >= 5) ? 5 : distanceBetweenPlatforms + 0.25f;
            chanceOfCrumblingPlatform = (chanceOfCrumblingPlatform >= 0.65f) ? 0.65f : chanceOfCrumblingPlatform + 0.05f;
        }

        if (nextPlatformIndex % 100 == 0)
            canSpawnMovingPlatforms = true;
        if(nextPlatformIndex % 300 == 0)
            canSpawnCrumblingPlatforms = true;
    }
}

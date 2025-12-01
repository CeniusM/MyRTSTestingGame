using System;
using UnityEngine;

// Used to get information about the world, such as terrain types, obstacles, entities at locations, etc.
// in an easy way to simplify other game logic scripts.
public class WorldInfoFetcher : MonoBehaviour
{
    private static WorldInfoFetcher _instance;
    public static WorldInfoFetcher GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindFirstObjectByType<WorldInfoFetcher>();
            if (_instance == null)
            {
                Debug.LogError("No WorldInfoFetcher instance found in the scene!");
            }
        }
        return _instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update() { }

    public BaseEntity GetEntityAtPosition(Vector2 position)
    {
        throw new NotImplementedException();
    }

    public BaseEntity[] GetEntitiesInArea(Vector2 start, Vector2 end)
    {
        throw new NotImplementedException();
    }

    public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        throw new NotImplementedException();
    }

    public Vector2 WorldToScreenPosition(Vector2 worldPosition)
    {
        throw new NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private List<PathFinding> creepersInGame = new List<PathFinding>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
    public List<PathFinding> GetCreepersInGame()
    {
        return creepersInGame;
    }
    public void Subscribe(PathFinding _creeper)
    { 
        if(!(creepersInGame.Contains(_creeper)))
        {
            creepersInGame.Add(_creeper);
        }
    }
    public void UnSubscribe(PathFinding _creeper)
    {
        if (creepersInGame.Contains(_creeper))
        {
            creepersInGame.Remove(_creeper);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private CharacterStats playerStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(this);
    }

    void Update()
    {
        bool playerIsDead = playerStats.CurrentHealth == 0;
        if (playerIsDead)
        {
            NotifyObservers();
        }
    }

    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void AddObservers(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObservers(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}

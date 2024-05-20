using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public Map map;

    public string mapName;
    public Sprite mapOnSprite;
    public Sprite mapOffSprite;
    public float gameDuration;
    public int enemiesPerWave;
    public int enemiesGrowUpPerWave;
    public int coinsToUnlock;
    public EnemySpawnProperties[] enemyPrefabs;
    public EnemySpawnProperties[] bossPrefab;
    public GameObject GetEnemy()
    {
        int total = 0;
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            total += enemyPrefabs[i].appearRarity;
        }

        int random = Random.Range(0, total);
        int curRarity = 0;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            curRarity += enemyPrefabs[i].appearRarity;
            if (curRarity > random)
            {
                return enemyPrefabs[i].enemyPrefab;
            }
        }
        return null;
    }

    public GameObject GetBoss()
    {
        int total = 0;
        for (int i = 0; i < bossPrefab.Length; i++)
        {
            total += bossPrefab[i].appearRarity;
        }

        int random = Random.Range(0, total);
        int curRarity = 0;

        for (int i = 0; i < bossPrefab.Length; i++)
        {
            curRarity += bossPrefab[i].appearRarity;
            if (curRarity > random)
            {
                return bossPrefab[i].enemyPrefab;
            }
        }
        return null;
    }

}

[System.Serializable]
public enum Difficulty
{
    Easy, Medium, Hard, Nightmare
}

public enum Map
{
    Tropical, Volcano, Snow, Babylon, Countryside, Downtown
}

[System.Serializable]
public class EnemySpawnProperties
{
    public GameObject enemyPrefab;
    public int appearRarity;
}

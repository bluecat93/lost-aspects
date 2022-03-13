using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesList : MonoBehaviour
{
    private List<Transform> enemies;

    private void Awake()
    {
        enemies = new List<Transform>();
    }

    public List<Transform> getEnemies()
    {
        return enemies;
    }

    public void addEnemy(Transform enemy)
    {
        enemies.Add(enemy);
    }

    public void removeEnemy(Transform enemy)
    {
        enemies.Remove(enemy);
    }
}

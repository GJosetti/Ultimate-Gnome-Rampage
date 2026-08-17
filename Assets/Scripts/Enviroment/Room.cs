using System.Collections.Generic;
using UnityEngine;
public class Room : MonoBehaviour
{
    [SerializeField]
    List<BaseEnemy> enemies;
    [SerializeField]
    bool isCleared = false;
    [SerializeField]
    int room;

    void Start()
    {
        // se assina o evento de cada inimigo já presente na lista no início
        foreach (BaseEnemy enemy in enemies)
        {
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    void HandleEnemyDeath(BaseEnemy enemy)
    {
        enemy.OnDeath -= HandleEnemyDeath; // evita memory leak/dupla assinatura
        enemies.Remove(enemy);

        if (enemies.Count <= 0)
        {
            isCleared = true;
        }
    }

    void Update()
    {
       
        if (room == GameManager.room)
        {
            GameManager.inCombat = !isCleared;
        }
        
    }
}
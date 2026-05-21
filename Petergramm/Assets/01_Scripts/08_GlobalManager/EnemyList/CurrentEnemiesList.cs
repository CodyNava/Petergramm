using System;
using System.Collections.Generic;
using _01_Scripts._07_Enemy.Runtime;
using NaughtyAttributes;
using UnityEngine;

public class CurrentEnemiesList : MonoBehaviour
{
    public List<GameObject> enemies;


   
    
    //todo wenn gegner spawned hier reinschreiben am besten über den spawner dann
    public void AddEnemies(GameObject enemy)
    {
        enemies.Add(enemy);
        Debug.Log(" CURRENT ENEMIES " + enemies.Count);
    }
    
}



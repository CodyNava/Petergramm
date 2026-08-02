using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   [CreateAssetMenu (menuName = "EnemySpawner/Enemy Spawn Data")]
   public class EnemySpawnDataSO : ScriptableObject
   {

      public EnemySpawnData spawnData;

   }

   [Serializable]
   public struct EnemySpawnData
   {
      public Vector3 spawnPoint;
      public List<GameObject> enemies;
   }
}

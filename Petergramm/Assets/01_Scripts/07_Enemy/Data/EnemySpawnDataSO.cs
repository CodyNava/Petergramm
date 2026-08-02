using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   [CreateAssetMenu (menuName = "EnemySpawner/Enemy Spawn Data")]
   public class EnemySpawnDataSO : ScriptableObject
   {
      public WaveData[] waveData;
      public SpawnData spawnData;
   }

   
}

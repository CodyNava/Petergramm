using System;
using System.Collections.Generic;
using _01_Scripts._07_Enemy.Runtime;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   
   public enum EnemyTypes
   {
      FatZombie,
      NormalZombie 
   }
   
   [Serializable]
   public struct SpawnData
   {
      public Vector3 spawnPoint;
      public List<EnemyRuntime> enemies;
   }
   
   
   [Serializable]
   public struct WaveData
   {
      public int waveTime;
      public int enemyCount;
      [Header("StatMultiplier")]
      public float moneyMult;
      public float hpMult;
      public EnemyTypeList[] enemyTypes;
      
   }

   [Serializable]
   public struct EnemyTypeList
   {
      public EnemyTypes spawnableEnemies;
      public int enemyCount;
      
   }
   
}
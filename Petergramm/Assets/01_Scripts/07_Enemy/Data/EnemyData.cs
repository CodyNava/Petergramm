using System;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   public enum EnemyStatTypes
   {
      MoveSpeed,
      MaxHp,
      Damage,
      AttacksPerSecond,
      Range,
      Armor
   }

   public enum EnemyArmorTypes
   {
      Cloth,
      Mail,
      Plate
   }
   
   public enum EnemyPassiveTypes
   {
      Regeneration,
      MoveSpeedAura,
      HealAura
   }

   public enum EnemyAbilityTypes
   {
      DeathRattle,
      Summoning,
   }
   
   public enum EnemyMovementTypes
   {
      Normal,
      Flying,
      Running
   }
   
   [Serializable]
   public struct EnemyStats
   {
      public int maxHp;
      public float damage;
      public float attacksPerSecond;
      public float range;
      public EnemyMovement movement;
      public EnemyArmor armor;
   }

   [Serializable]
   public struct EnemyPassive
   {
      public EnemyPassiveTypes enemyPassive;
      public float effectValue;
   }
   
   [Serializable]
   public struct EnemyAbility
   {
      public EnemyAbilityTypes enemyAbility;
      public float amount;
      public float frequency;
      public GameObject summonedMonster;
   }
   
   [Serializable]
   public struct EnemyMovement
   {
      public EnemyMovementTypes  movementType;
      public float moveSpeed;
   }
   
   [Serializable]
   public struct EnemyArmor
   {
      public EnemyArmorTypes  armorType;
      public float armor;
   }
}
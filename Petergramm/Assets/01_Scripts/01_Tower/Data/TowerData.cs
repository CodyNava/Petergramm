using System;
using NUnit.Framework;

namespace _01_Scripts._01_Tower.Data
{
   public enum TowerStatType
   {
      MaxHp,
      Damage,
      Range,
      AttacksPerSecond,
      Energy,
      BaseProjectileAmount
   }

   public enum TowerEffectType
   {
      ExtraProjectileAmount,
      SlowPercent,
      BounceCount
   }

   public enum TowerDamageType
   {
      Pierce,
      Normal,
      Impact,
   }
   
   public enum TowerProjectileType
   {
      Basketball,
      Baseball
   }
   
   //Hier nutze ich structs da diese nur value types sind
   //und mehr sollen die auch nicht sein dazu auch noch viel billiger
   [Serializable]
   public struct TowerStats
   {
      public float maxHp;
      public short damage;
      public float range;
      public float attacksPerSecond;
      public int energy;
      public int baseProjectileAmount;
   }

   

   [Serializable]
   public struct TowerStatModifier
   {
      public TowerStatType statType;
      public float additiveValue;
   }

   [Serializable]
   public struct TowerEffectModifier
   {
      public TowerEffectType effectType;
      public float value;
   }

   [Serializable]
   public struct UpgradeEffectModifier
   {
      public TowerEffectType effectType;
      public float addPerStack;
      public float maxBonus;
   }

   [Serializable]
   public class TowerEffectValues
   {
      public int projectileAmount;
      public float slowPercent;
      public int bounceCount;

      public void Reset()
      {
         this.projectileAmount = 0;
         this.slowPercent = 0;
         this.bounceCount = 0;
      }
   }
}
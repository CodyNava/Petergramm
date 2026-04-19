using System;

namespace _01_Scripts._01_Tower.Data
{
   public enum TowerStatType
   {
      MaxHp,
      Damage,
      Range,
      AttacksPerSecond
   }

   public enum TowerEffectType
   {
      AdditionalTargets,
      SlowPercent,
      BounceCount
   }

   [Serializable]
   public struct TowerStats
   {
      public float maxHp;
      public float damage;
      public float range;
      public float attacksPerSecond;
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
      public int additionalTargets;
      public float slowPercent;
      public int bounceCount;

      public void Reset()
      {
         this.additionalTargets = 0;
         this.slowPercent = 0;
         this.bounceCount = 0;
      }
   }
}
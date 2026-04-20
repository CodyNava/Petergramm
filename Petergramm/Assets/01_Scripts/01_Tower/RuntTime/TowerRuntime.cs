using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using UnityEngine;
using NaughtyAttributes;

namespace _01_Scripts._01_Tower.RuntTime
{
   public class TowerRuntime : MonoBehaviour
   {
      [Header("BaseStats")]
      [SerializeField] private TowerBaseSO towerBase;

      [Header("Runtime")]
      [SerializeField] private TowerStats currentStats;
      [SerializeField] private TowerEffectValues currentEffects = new();
      [SerializeField] private List<TowerUpgradeSO> appliedUpgrades = new();

      //Getter
      public TowerBaseSO TowerBase => this.towerBase;
      public TowerStats CurrentStats => this.currentStats;
      public TowerEffectValues CurrentEffects => this.currentEffects;
      public List<TowerUpgradeSO> AppliedUpgrades => this.appliedUpgrades;

      private void Awake() { this.ReApplyRuntimeValues(); }

      public void Initialize(TowerBaseSO newTowerBase)
      {
         this.towerBase = newTowerBase;
         this.appliedUpgrades.Clear();
         this.ReApplyRuntimeValues();
      }

      public bool TryAddUpgrade(TowerUpgradeSO upgrade)
      {
         int currentStacks = this.GetUpgradeStackCount(upgrade);
         if (currentStacks >= upgrade.maxStacks) return false;

         this.appliedUpgrades.Add(upgrade);
         this.ReApplyRuntimeValues();
         return true;
      }

      private int GetUpgradeStackCount(TowerUpgradeSO upgrade)
      {
         var count = 0;
         foreach (TowerUpgradeSO t in this.appliedUpgrades)
         {
            if (t == upgrade) count++;
         }

         return count;
      }

      public void ReApplyRuntimeValues()
      {
         if (this.towerBase == null) return;

         this.currentStats = this.towerBase.baseStats;
         this.currentEffects.Reset();

         this.ApplyInnateEffects();
         this.ApplyUpgrades();
      }

      
      private void ApplyUpgrades()
      {
         var stackCounts = new Dictionary<TowerUpgradeSO, int>();

         foreach (TowerUpgradeSO upgrade in this.appliedUpgrades)
         {
            if (!stackCounts.TryAdd(upgrade, 1)) stackCounts[upgrade]++;
         }

         foreach (KeyValuePair<TowerUpgradeSO, int> pair in stackCounts)
         {
            TowerUpgradeSO upgrade = pair.Key;
            int stacks = pair.Value;

            for (var i = 0; i < upgrade.statModifiers.Capacity; i++)
            {
               TowerStatModifier modifer = upgrade.statModifiers[i];
               float totalBonus = modifer.additiveValue * stacks;
               this.ApplyStatValue(modifer.statType, totalBonus);
            }

            for (var i = 0; i < upgrade.effectModifiers.Count; i++)
            {
               UpgradeEffectModifier modifier = upgrade.effectModifiers[i];
               float totalBonus = modifier.addPerStack * stacks;
               totalBonus = Mathf.Clamp(totalBonus, -modifier.maxBonus, modifier.maxBonus);
               this.ApplyEffectValue(modifier.effectType, totalBonus);
            }
         }
      }

      private void ApplyStatValue(TowerStatType modiferStatType, float totalBonus)
      {
         switch (modiferStatType)
         {
            case TowerStatType.MaxHp: this.currentStats.maxHp += totalBonus; break;

            case TowerStatType.Damage: this.currentStats.damage += totalBonus; break;

            case TowerStatType.Range: this.currentStats.range += totalBonus; break;

            case TowerStatType.AttacksPerSecond: this.currentStats.attacksPerSecond += totalBonus; break;
         }
      }

      private void ApplyEffectValue(TowerEffectType modifierEffectType, float totalBonus)
      {
         switch (modifierEffectType)
         {
            case TowerEffectType.AdditionalTargets:
               this.currentEffects.additionalTargets += Mathf.RoundToInt(totalBonus); break;

            case TowerEffectType.SlowPercent: this.currentEffects.slowPercent += totalBonus; break;

            case TowerEffectType.BounceCount: this.currentEffects.bounceCount += Mathf.RoundToInt(totalBonus); break;
         }
      }

      private void ApplyInnateEffects()
      {

         for (int i = 0; i < this.towerBase.innateEffects.Count; i++)
         {
            TowerEffectModifier modifier = this.towerBase.innateEffects[i];
            this.ApplyEffectValue(modifier.effectType, modifier.value);
         }
         
      }
   }
}
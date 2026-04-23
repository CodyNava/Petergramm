using _01_Scripts._07_Enemy.Data;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyRuntime : MonoBehaviour
    {

        [SerializeField] private EnemyBaseSO enemyBase;
        [SerializeField] private EnemyStats currentStats;
        
        //Getter
        public EnemyStats  CurrentStats => this.currentStats;
        public EnemyBaseSO EnemyBase => enemyBase;
        private void Start() => ApplyStats();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void ApplyStats()
        {
             currentStats.maxHp = enemyBase.stats.maxHp;
             currentStats.armor = enemyBase.stats.armor;
             currentStats.damage = enemyBase.stats.damage;
             currentStats.attacksPerSecond = enemyBase.stats.attacksPerSecond;
             currentStats.movement = enemyBase.stats.movement;
             currentStats.range = enemyBase.stats.range;
        }
        
    }
}

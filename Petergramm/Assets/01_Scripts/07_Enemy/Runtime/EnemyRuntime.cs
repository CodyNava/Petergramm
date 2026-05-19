using System.Collections;
using _01_Scripts._07_Enemy.Data;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyRuntime : MonoBehaviour
    {

        [SerializeField] private EnemyBaseSO enemyBase;
        [SerializeField] private EnemyStats currentStats;

        private bool _slowed;
        //Getter
        public EnemyStats  CurrentStats => this.currentStats;
        public EnemyBaseSO EnemyBase => enemyBase;
        private void Awake() => ApplyStats();
        
        private void ApplyStats()
        {
             currentStats.maxHp = enemyBase.stats.maxHp;
             currentStats.armor = enemyBase.stats.armor;
             currentStats.damage = enemyBase.stats.damage;
             currentStats.attacksPerSecond = enemyBase.stats.attacksPerSecond;
             currentStats.movement = enemyBase.stats.movement;
             currentStats.range = enemyBase.stats.range;
        }

        private void Update()
        {
            
        }
        public void ApplySlow(byte slow)
        {
            //todo needs duration etc (just a test case)
            if (slow <= 0 || _slowed) return;
            var currentMovementSpeed = this.currentStats.movement.moveSpeed;
            var moveSpeedSlowDif = (currentMovementSpeed * slow / 100f);
            this.currentStats.movement.moveSpeed = currentMovementSpeed - moveSpeedSlowDif;
            //this.currentStats.movement.moveSpeed /= slow;
            _slowed = true;
        }
        
       
    }
}

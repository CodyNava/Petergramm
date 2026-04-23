using _01_Scripts._07_Enemy.Data;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private EnemyRuntime enemyRuntime;
        private float _cd;

        private void Update()
        {
            this._cd -= Time.deltaTime;
            float attacksPerSecond = this.enemyRuntime.CurrentStats.attacksPerSecond;

            if (attacksPerSecond <= 0f) return;

            if (this._cd <= 0f)
            {
                this._cd = attacksPerSecond;
                this.Attack();
            }
        }

        private void Attack()
        {
            EnemyStats stats = this.enemyRuntime.CurrentStats;
            var damage = stats.damage;
            var range = stats.range;
            
            
            //TODO ATTACKING
        }
    }
}

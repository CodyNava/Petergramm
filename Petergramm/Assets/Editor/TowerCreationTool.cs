using _01_Scripts._01_Tower.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TowerCreationTool : EditorWindow
    {
        //todo: erweitern um DamageType
        public string towerName;
        public float hitPoints;
        public float damage;
        public float range;
        public float attacksPerSecond;
        
        [MenuItem("Creation Tools/Tower Creation")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TowerCreationTool));
        }

        private void OnGUI()
        {
            towerName = EditorGUILayout.TextField("Tower Name", towerName);
            hitPoints = EditorGUILayout.FloatField("HitPoints", hitPoints);
            damage = EditorGUILayout.FloatField("Damage", damage);
            range = EditorGUILayout.FloatField("Range", range);
            attacksPerSecond = EditorGUILayout.FloatField("Attacks per Second", attacksPerSecond);

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Tower"))
            {
                CreateTower();
            }
        }

        private void CreateTower()
        {
            var newTowerBase = CreateInstance<TowerBaseSO>();
            var newTowerAttack = CreateInstance<TowerAttackSO>();
            var newTowerProjectile = CreateInstance<ProjectileSO>();
            
            AssetDatabase.CreateFolder("Assets/03_SO/Tower", towerName);
            
            AssetDatabase.CreateAsset(newTowerBase, $"Assets/03_SO/Tower/{towerName}/{towerName}.asset");
            newTowerBase.name = towerName;
            newTowerBase.baseStats.maxHp = hitPoints;
            newTowerBase.baseStats.damage = damage;
            newTowerBase.baseStats.range = range;
            newTowerBase.baseStats.attacksPerSecond = attacksPerSecond;
            
            AssetDatabase.CreateAsset(newTowerProjectile, $"Assets/03_SO/Tower/{towerName}/{towerName}Projectile.asset");
            
            
            AssetDatabase.CreateAsset(newTowerAttack, $"Assets/03_SO/Tower/{towerName}/{towerName}Attack.asset");
            newTowerAttack.projectile = newTowerProjectile;
            
            AssetDatabase.SaveAssets();
        }
    }
}
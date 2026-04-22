using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TowerCreationTool : EditorWindow
    {
        //todo: erweitern um DamageType
        public string towerName;
        public Sprite icon;
        public float hitPoints;
        public float damage;
        public float range;
        public float attacksPerSecond;

        private TowerBaseSO _createdTowerBase;
        private TowerAttackSO _createdAttackData;
        private ProjectileSO _createdProjectile;

        private readonly List<string> _warnings = new();
        private bool _hasErrors;

        [MenuItem("Creation Tools/Tower Creation")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TowerCreationTool));
        }

        private void OnGUI()
        {
            towerName = EditorGUILayout.TextField("Tower Name", towerName);
            icon = EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false) as Sprite;
            hitPoints = EditorGUILayout.FloatField("HitPoints", hitPoints);
            damage = EditorGUILayout.FloatField("Damage", damage);
            range = EditorGUILayout.FloatField("Range", range);
            attacksPerSecond = EditorGUILayout.FloatField("Attacks per Second", attacksPerSecond);

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Tower"))
            {
                _hasErrors = CheckValidInput();
                if (!_hasErrors)
                {
                    AssetDatabase.CreateFolder("Assets/03_SO/Tower", towerName);
                    AssetDatabase.CreateFolder("Assets/04_Prefabs/Tower", towerName);
                    CreateTower();
                    CreateAttackData();
                    CreateProjectile();
                    AddConnections();
                    AssetDatabase.SaveAssets();
                }
            }

            if (_hasErrors)
            {
                ShowWarning(_warnings);
            }
        }

        #region Creation

        private void CreateTower()
        {
            _createdTowerBase = CreateInstance<TowerBaseSO>();

            var tempTower = new GameObject();
            var towerPrefab =
                PrefabUtility.SaveAsPrefabAsset(tempTower, $"Assets/04_Prefabs/Tower/{towerName}/{towerName}.prefab");
            DestroyImmediate(tempTower);

            AssetDatabase.CreateAsset(_createdTowerBase, $"Assets/03_SO/Tower/{towerName}/{towerName}.asset");
            _createdTowerBase.towerName = towerName;
            _createdTowerBase.icon = icon;
            _createdTowerBase.towerPrefab = towerPrefab;
            _createdTowerBase.baseStats.maxHp = hitPoints;
            _createdTowerBase.baseStats.damage = damage;
            _createdTowerBase.baseStats.range = range;
            _createdTowerBase.baseStats.attacksPerSecond = attacksPerSecond;
        }

        private void CreateProjectile()
        {
            _createdProjectile = CreateInstance<ProjectileSO>();

            var tempProjectile = new GameObject();
            var projectilePrefab = PrefabUtility.SaveAsPrefabAsset(tempProjectile,
                $"Assets/04_Prefabs/Tower/{towerName}/{towerName}Projectile.prefab");
            DestroyImmediate(tempProjectile);

            AssetDatabase.CreateAsset(_createdProjectile,
                $"Assets/03_SO/Tower/{towerName}/{towerName}Projectile.asset");
            _createdProjectile.projectilePrefab = projectilePrefab;
        }

        private void CreateAttackData()
        {
            _createdAttackData = CreateInstance<TowerAttackSO>();
            AssetDatabase.CreateAsset(_createdAttackData,
                $"Assets/03_SO/Tower/{towerName}/{towerName}AttackData.asset");
        }

        private void AddConnections()
        {
            _createdTowerBase.attackData = _createdAttackData;
            _createdAttackData.projectile = _createdProjectile;
        }
        
        #endregion

        #region InputChecks

        private bool CheckValidInput()
        {
            _warnings.Clear();
            if (string.IsNullOrWhiteSpace(towerName))
            {
                _warnings.Add("Missing Tower Name");
            }

            if (icon == null)
            {
                _warnings.Add("Missing Tower Icon");
            }

            if (hitPoints <= 0)
            {
                _warnings.Add("Invalid Tower Hp");
            }

            if (damage <= 0)
            {
                _warnings.Add("Invalid Tower Damage");
            }

            if (range <= 0)
            {
                _warnings.Add("Invalid Tower Range");
            }

            if (attacksPerSecond <= 0)
            {
                _warnings.Add("Invalid Attacks Per Second");
            }

            return _warnings.Count != 0;
        }

        private void ShowWarning(List<string> warnings)
        {
            foreach (var warning in warnings)
            {
                EditorGUILayout.HelpBox(warning, MessageType.Error);
            }
        }

        #endregion
    }
}
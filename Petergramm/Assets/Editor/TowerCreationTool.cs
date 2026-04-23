using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.RuntTime;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TowerCreationTool : EditorWindow
    {
        public string towerName;
        public Sprite icon;
        public float maxHitPoints;
        public short damage;
        public float range;
        public float attacksPerSecond;
        public TowerEffectType towerEffect;
        public int effectCount;
        public TowerUpgradeSO upgrade;

        public byte projectileSpeed;

        private TowerBaseSO _createdTowerBase;
        private TowerAttackSO _createdAttackData;
        private ProjectileSO _createdProjectile;
        private readonly List<TowerUpgradeSO> _upgrades = new();

        private bool _showUpgrades;
        private bool _newUpgrades;
        private readonly List<string> _warnings = new();
        private bool _hasErrors;

        [MenuItem("Creation Tools/Tower Creation")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TowerCreationTool));
        }

        private void OnGUI()
        {
            #region TowerSettings

            towerName = EditorGUILayout.TextField(new GUIContent("Tower Name",
                "The name the tower is supposed to have in-game.\n" +
                "<b>This field cannot be left empty</b>"), towerName);
            icon = EditorGUILayout.ObjectField(new GUIContent("Icon",
                "The icon the tower is supposed to have in-game.\n" +
                "<b>This field cannot be left empty</b>"), icon, typeof(Sprite), false) as Sprite;
            maxHitPoints = EditorGUILayout.Slider(new GUIContent("Maximum HP",
                "The maximum amount of hit points this tower is supposed to have in-game.\n" +
                "<b>This value CANNOT be 0.</b>"), maxHitPoints, 0f, 100f);
            damage = (short)EditorGUILayout.Slider(new GUIContent("Damage",
                "The Damage amount this tower is supposed to deal per shot.\n" +
                "<b>This value CANNOT be 0.</b>"), damage, 0f, 20f);
            range = EditorGUILayout.Slider(new GUIContent("Attack Range",
                "The range in which the tower can attack the enemies.\n" +
                "<b>This value CANNOT be 0.</b>"), range, 0f, 5f);
            attacksPerSecond = EditorGUILayout.Slider(new GUIContent("Attacks per Second",
                "How many attacks this tower is supposed to do per second.\n" +
                "<b>This value CANNOT be 0.</b>"), attacksPerSecond, 0f, 5f);
            towerEffect = (TowerEffectType)EditorGUILayout.EnumPopup(new GUIContent("Tower Effect",
                "The Initial effect this tower is supposed to have.\n" +
                "<b>Additional Targets:</b> This tower can attack multiple enemies at once.\n" +
                "<b>Slow Percent:</b> This tower slows enemies on attack.\n" +
                "<b>Bounce Count:</b> This towers projectile bounces in between enemies."), towerEffect);
            effectCount = EditorGUILayout.IntSlider(new GUIContent("Effect amount",
                "How many times the effect can occur/How strong the effect is."), effectCount, 0, 5);
            _showUpgrades = EditorGUILayout.Foldout(_showUpgrades, "Upgrades");

            if (_showUpgrades)
            {
                if (_upgrades.Count != 0)
                {
                    EditorGUILayout.BeginVertical();
                    foreach (var item in _upgrades)
                    {
                        EditorGUILayout.HelpBox(item.upgradeName, MessageType.None);
                    }
                    EditorGUILayout.EndVertical();
                }

                if (!_newUpgrades)
                {
                    if (GUILayout.Button("Add Upgrade"))
                    {
                        _newUpgrades = true;
                    }
                }

                if (_newUpgrades)
                {
                    upgrade = EditorGUILayout.ObjectField(new GUIContent("Upgrade Type",
                            "Choose one of the upgrades to add it to your tower"),
                        upgrade, typeof(TowerUpgradeSO), false) as TowerUpgradeSO;
                    if (GUILayout.Button("Finish Upgrade"))
                    {
                        _upgrades.Add(upgrade);
                        _newUpgrades = false;
                    }
                }
            }

            #endregion

            #region ProjectileSettings

            projectileSpeed =(byte)
                EditorGUILayout.Slider(
                    new GUIContent("Projectile Speed",
                        "The speed at which the projectile travels. \n<b>This value CANNOT be 0.</b>"), projectileSpeed,
                    0f, 3f);

            #endregion

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
            tempTower.AddComponent<TowerRuntime>();
            tempTower.AddComponent<TowerHealth>();
            tempTower.AddComponent<TowerAttack>();
            
            var towerPrefab =
                PrefabUtility.SaveAsPrefabAsset(tempTower, $"Assets/04_Prefabs/Tower/{towerName}/{towerName}.prefab");
            DestroyImmediate(tempTower);

            AssetDatabase.CreateAsset(_createdTowerBase, $"Assets/03_SO/Tower/{towerName}/{towerName}.asset");
            _createdTowerBase.towerName = towerName;
            _createdTowerBase.icon = icon;
            _createdTowerBase.towerPrefab = towerPrefab;
            _createdTowerBase.baseStats.maxHp = maxHitPoints;
            _createdTowerBase.baseStats.damage = damage;
            _createdTowerBase.baseStats.range = range;
            _createdTowerBase.baseStats.attacksPerSecond = attacksPerSecond;
            _createdTowerBase.innateEffects.Add(new TowerEffectModifier()
                { effectType = towerEffect, value = effectCount });

            foreach (var towerUpgrade in _upgrades)
            {
                CreateInstance<TowerUpgradeSO>();
                _createdTowerBase.availableUpgrades.Add(towerUpgrade);
            }

            var runtime = towerPrefab.GetComponent<TowerRuntime>();
            var towerAsset = AssetDatabase.LoadAssetAtPath<TowerBaseSO>($"Assets/03_SO/Tower/{towerName}/{towerName}.asset");
            runtime.TowerBase = towerAsset;
            PrefabUtility.SavePrefabAsset(towerPrefab);
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
            _createdProjectile.speed = projectileSpeed;
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

            if (maxHitPoints <= 0)
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

            if (projectileSpeed <= 0)
            {
                _warnings.Add("Invalid Projectile Speed");
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
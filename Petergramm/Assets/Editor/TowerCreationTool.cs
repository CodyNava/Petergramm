using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.RuntTime;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    enum Page
    {
        Tower,
        Projectile,
        Upgrade,
        Enemy,
        Abilities,
        Passives
    }

    public class TowerCreationTool : EditorWindow
    {
        //General Stats
        [SerializeField] private string towerName;
        [SerializeField] private Sprite icon;
        [SerializeField] private float maxHitPoints;
        [SerializeField] private short damage;
        [SerializeField] private TowerDamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int energyUsage;
        [SerializeField] private TowerEffectType towerEffect;
        [SerializeField] private int effectCount;

        //Projectile Settings
        [SerializeField] private TowerProjectileType projectileType;
        [SerializeField] private int projectileAmount;
        [SerializeField] private float projectileSpeed;

        //Upgrade Settings
        [SerializeField] private TowerUpgradeSO upgrade;

        private bool _showUpgrades;
        private bool _newUpgrades;

        //Editor Window
        private Page _currentPage;

        //For Creation
        private TowerBaseSO _createdTowerBase;
        private TowerAttackSO _createdAttackData;
        private ProjectileSO _createdProjectile;
        private readonly List<TowerUpgradeSO> _upgrades = new();

        //Warnings
        private readonly List<string> _warnings = new();
        private bool _hasErrors;
        
        private const string TowerSOPath = "Assets/03_SO/Tower";
        private const string TowerPrefabPath = "Assets/04_Prefabs/Tower";
        private const string ProjectilePath = "Assets/04_Prefabs/Projectiles";

        #region Editor
        [MenuItem("Creation Tools/Tower Creation")]
        public static void ShowMyEditor()
        {
            var window = GetWindow<TowerCreationTool>();
            window.titleContent = new GUIContent("Tower Creation");

            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(400));

            if (GUILayout.Button("Tower Settings"))
            {
                _currentPage = Page.Tower;
            }

            if (GUILayout.Button("Projectile Settings"))
            {
                _currentPage = Page.Projectile;
            }

            if (GUILayout.Button("Upgrade Settings"))
            {
                _currentPage = Page.Upgrade;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            switch (_currentPage)
            {
                case Page.Tower:
                    TowerSetup();
                    break;
                case Page.Projectile:
                    ProjectileSetup();
                    break;
                case Page.Upgrade:
                    TowerUpgradeSetup();
                    break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Tower"))
            {
                _hasErrors = CheckValidInput();
                if (!_hasErrors)
                {
                    var towerPath = $"{TowerSOPath}/{towerName}";
                    if (!AssetDatabase.IsValidFolder(towerPath))
                    {
                        AssetDatabase.CreateFolder(TowerSOPath, towerName);
                    }
                    var prefabPath = $"{TowerPrefabPath}/{towerName}";
                    if (!AssetDatabase.IsValidFolder(prefabPath))
                    {
                        AssetDatabase.CreateFolder(TowerPrefabPath, towerName);
                    }
                    
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
        #endregion

        #region Setup
        private void TowerSetup()
        {
            towerName = EditorGUILayout.TextField(new GUIContent("Tower Name",
                    "The name the tower is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty</b>"),
                towerName);
            icon = EditorGUILayout.ObjectField(new GUIContent("Icon",
                    "The icon the tower is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty</b>"),
                icon, typeof(Sprite), false) as Sprite;
            maxHitPoints = EditorGUILayout.Slider(new GUIContent("Maximum HP",
                    "The maximum amount of hit points this tower is supposed to have in-game.\n" +
                    "<b>This value CANNOT be 0.</b>"),
                maxHitPoints, 0f, 100f);
            damage = (short)EditorGUILayout.Slider(new GUIContent("Damage",
                    "The Damage amount this tower is supposed to deal per shot.\n" +
                    "<b>This value CANNOT be 0.</b>"),
                damage, 0f, 20f);
            damageType = (TowerDamageType)EditorGUILayout.EnumPopup(new GUIContent("Damage Type",
                    "The type of damage this tower is supposed to deal.\n" +
                    "<b>Pierce:</b> .\n" +
                    "<b>Normal:</b> .\n" +
                    "<b>Impact:</b> .\n"),
                damageType);
            range = EditorGUILayout.Slider(new GUIContent("Attack Range",
                    "The range in which the tower can attack the enemies.\n" +
                    "<b>This value CANNOT be 0.</b>"),
                range, 0f, 5f);
            attacksPerSecond = EditorGUILayout.Slider(new GUIContent("Attacks per Second",
                    "How many attacks this tower is supposed to do per second.\n" +
                    "<b>This value CANNOT be 0.</b>"),
                attacksPerSecond, 0f, 5f);
            energyUsage = EditorGUILayout.IntSlider(new GUIContent("Energy Usage",
                    "How much energy does this tower require to keep running.\n" +
                    "<b>This value CANNOT be 0.</b>"),
                energyUsage, 0, 10);
            towerEffect = (TowerEffectType)EditorGUILayout.EnumPopup(new GUIContent("Tower Effect",
                    "The Initial effect this tower is supposed to have.\n" +
                    "<b>Additional Targets:</b> This tower can attack multiple enemies at once.\n" +
                    "<b>Slow Percent:</b> This tower slows enemies on attack.\n" +
                    "<b>Bounce Count:</b> This towers projectile bounces in between enemies."),
                towerEffect);
            effectCount = EditorGUILayout.IntSlider(new GUIContent("Effect amount",
                    "How many times the effect can occur/How strong the effect is."),
                effectCount, 0, 5);
        }

        private void ProjectileSetup()
        {
            projectileType = (TowerProjectileType)EditorGUILayout.EnumPopup(new GUIContent("Projectile Type",
                    "The type of projectile this tower shoots.\n" +
                    "<b>Basketball:</b> .\n" +
                    "<b>Baseball:</b> .\n"),
                projectileType);
            projectileAmount = EditorGUILayout.IntSlider(new GUIContent("Projectile Amount",
                    "How many projectiles this tower fires per attack.\n" +
                    "<b>This value cannot be 0.</b>"),
                projectileAmount, 0, 10);

            projectileSpeed = (byte)
                EditorGUILayout.Slider(
                    new GUIContent("Projectile Speed",
                        "The speed at which the projectile travels. \n<b>This value CANNOT be 0.</b>"),
                    projectileSpeed, 0f, 3f);
        }

        private void TowerUpgradeSetup()
        {
            _showUpgrades = EditorGUILayout.Foldout(_showUpgrades, "Upgrades");

            if (_showUpgrades)
            {
                if (_upgrades.Count != 0)
                {
                    EditorGUILayout.BeginVertical();
                    foreach (var item in _upgrades)
                    {
                        if (item == null)
                        {
                            continue;
                        }
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
                    if (upgrade != null)
                    {
                        if (GUILayout.Button("Finish Upgrade"))
                        {
                            _upgrades.Add(upgrade);
                            _newUpgrades = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Creation

        private void CreateTower()
        {
            _createdTowerBase = CreateInstance<TowerBaseSO>();

            var tempTower = new GameObject();
            tempTower.AddComponent<TowerRuntime>();
            tempTower.AddComponent<TowerHealth>();
            tempTower.AddComponent<TowerAttack>();

            var towerPrefab =
                PrefabUtility.SaveAsPrefabAsset(tempTower, $"{TowerPrefabPath}/{towerName}/{towerName}.prefab");
            DestroyImmediate(tempTower);

            AssetDatabase.CreateAsset(_createdTowerBase, $"{TowerSOPath}/{towerName}/{towerName}.asset");
            _createdTowerBase.towerName = towerName;
            _createdTowerBase.icon = icon;
            _createdTowerBase.towerPrefab = towerPrefab;
            _createdTowerBase.baseStats.maxHp = maxHitPoints;
            _createdTowerBase.baseStats.damage = damage;
            _createdTowerBase.baseStats.range = range;
            _createdTowerBase.baseStats.attacksPerSecond = attacksPerSecond;
            _createdTowerBase.baseStats.energy = energyUsage;
            _createdTowerBase.baseStats.baseProjectileAmount = projectileAmount;
            _createdTowerBase.innateEffects.Add(new TowerEffectModifier()
                { effectType = towerEffect, value = effectCount });

            foreach (var towerUpgrade in _upgrades)
            {
                _createdTowerBase.availableUpgrades.Add(towerUpgrade);
            }

            var runtime = towerPrefab.GetComponent<TowerRuntime>();
            var towerAsset =
                AssetDatabase.LoadAssetAtPath<TowerBaseSO>($"{TowerSOPath}/{towerName}/{towerName}.asset");
            runtime.TowerBase = towerAsset;
            PrefabUtility.SavePrefabAsset(towerPrefab);
        }

        private void CreateProjectile()
        {
            _createdProjectile = CreateInstance<ProjectileSO>();

            AssetDatabase.CreateAsset(_createdProjectile,
                $"{TowerSOPath}/{towerName}/{towerName}Projectile.asset");
            _createdProjectile.projectilePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    $"{ProjectilePath}/{projectileType.ToString()}.prefab");
            _createdProjectile.speed = (byte)projectileSpeed;
        }

        private void CreateAttackData()
        {
            _createdAttackData = CreateInstance<TowerAttackSO>();
            _createdAttackData.damageType = damageType;
            _createdAttackData.baseProjectileCount = projectileAmount;
            _createdAttackData.projectileType = projectileType;
            AssetDatabase.CreateAsset(_createdAttackData,
                $"{TowerSOPath}/{towerName}/{towerName}AttackData.asset");
        }

        private void AddConnections()
        {
            _createdTowerBase.attackData = _createdAttackData;
            _createdAttackData.projectile = _createdProjectile;
        }

        #endregion

        #region InputValidation

        private bool CheckValidInput()
        {
            _warnings.Clear();
            if (AssetDatabase.LoadAssetAtPath<TowerBaseSO>($"{TowerSOPath}/{towerName}/{towerName}.asset"))
            {
                _warnings.Add("Tower already exists.");
            }
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

            if (energyUsage <= 0)
            {
                _warnings.Add("Invalid Energy Usage");
            }

            if (projectileSpeed <= 0)
            {
                _warnings.Add("Invalid Projectile Speed");
            }

            if (projectileAmount <= 0)
            {
                _warnings.Add("Invalid Projectile Amount");
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
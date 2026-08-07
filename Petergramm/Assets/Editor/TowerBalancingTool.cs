using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    enum BalancePage
    {
        Tower,
        Projectile,
        Effects,
        Upgrades
    }

    public class TowerBalancingTool : EditorWindow
    {
        // Loaded Tower
        [SerializeField] private TowerBaseSO selectedTower;

        // Tower Stats
        [SerializeField] private string towerName;
        [SerializeField] private Sprite icon;
        [SerializeField] private float maxHitPoints;
        [SerializeField] private short damage;
        [SerializeField] private TowerDamageType damageType;
        [SerializeField] private float range;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private int energyUsage;

        // Projectile Settings
        [SerializeField] private ProjectileSO projectile;
        [SerializeField] private TowerProjectileType projectileType;
        [SerializeField] private int projectileAmount;
        [SerializeField] private float projectileSpeed;

        // Effects
        [SerializeField] private TowerEffectType towerEffect;
        [SerializeField] private int effectCount;

        // Upgrade Settings
        private readonly List<TowerUpgradeSO> _upgrades = new();
        private List<TowerUpgradeSO> _allUpgrades = new();
        private TowerUpgradeSO _selectedUpgrade;
        private TowerUpgradeSO _upgradeToAdd;

        // Editor State
        private BalancePage _currentPage;

        // Available Towers
        private List<TowerBaseSO> _availableTowers = new();

        // Validation
        private readonly List<string> _warnings = new();
        private bool _hasErrors;

        // Paths
        private const string TowerSOPath = "Assets/03_SO/Tower";

        #region Editor

        [MenuItem("Balancing Tools/Tower Balancing")]
        public static void ShowMyEditor()
        {
            var window = GetWindow<TowerBalancingTool>();

            window.titleContent = new GUIContent("Tower Balancing");

            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(200));

            if (GUILayout.Button("Tower Stats"))
            {
                _currentPage = BalancePage.Tower;
            }

            if (GUILayout.Button("Projectile"))
            {
                _currentPage = BalancePage.Projectile;
            }

            if (GUILayout.Button("Effects"))
            {
                _currentPage = BalancePage.Effects;
            }

            if (GUILayout.Button("Upgrades"))
            {
                _currentPage = BalancePage.Upgrades;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save Changes"))
            {
                UpdateValues();
            }

            if (GUILayout.Button("Revert Changes"))
            {
                ReloadSelectedTower();
            }

            EditorGUILayout.EndVertical();

            // Main Content
            EditorGUILayout.BeginVertical();

            switch (_currentPage)
            {
                case BalancePage.Tower:
                    TowerBalanceSetup();
                    break;

                case BalancePage.Projectile:
                    ProjectileBalanceSetup();
                    break;

                case BalancePage.Effects:
                    EffectBalanceSetup();
                    break;

                case BalancePage.Upgrades:
                    UpgradeBalanceSetup();
                    break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_hasErrors)
            {
                ShowWarnings(_warnings);
            }
        }

        #endregion

        #region Tower Selection

        private void TowerSelection()
        {
            _availableTowers = FetchTowers();

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;
            EditorGUILayout.ObjectField(
                new GUIContent("Selected Tower"),
                selectedTower,
                typeof(TowerBaseSO),
                false
            );

            GUI.enabled = true;

            if (GUILayout.Button(
                    EditorGUIUtility.IconContent("d_pick"),
                    GUILayout.Width(20)))
            {
                ShowTowerMenu();
            }

            EditorGUILayout.EndHorizontal();

            if (selectedTower != null)
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox(
                    $"Editing: {selectedTower.towerName}",
                    MessageType.Info
                );
            }
        }

        private List<TowerBaseSO> FetchTowers()
        {
            var guids = AssetDatabase.FindAssets(
                "t:TowerBaseSO",
                new[] { TowerSOPath }
            );

            var towers = new List<TowerBaseSO>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var tower = AssetDatabase.LoadAssetAtPath<TowerBaseSO>(path);

                if (tower != null)
                {
                    towers.Add(tower);
                }
            }

            return towers;
        }


        private void ShowTowerMenu()
        {
            var menu = new GenericMenu();

            foreach (var tower in _availableTowers)
            {
                menu.AddItem(
                    new GUIContent(tower.towerName),
                    tower == selectedTower,
                    () =>
                    {
                        LoadTower(tower);
                        Repaint();
                    });
            }

            menu.ShowAsContext();
        }

        private void LoadTower(TowerBaseSO tower)
        {
            selectedTower = tower;

            towerName = tower.towerName;
            icon = tower.icon;
            maxHitPoints = tower.baseStats.maxHp;
            damage = tower.baseStats.damage;
            range = tower.baseStats.range;
            attacksPerSecond = tower.baseStats.attacksPerSecond;
            energyUsage = tower.baseStats.energy;

            projectileAmount =
                tower.baseStats.baseProjectileAmount;

            _upgrades.Clear();

            foreach (var upgrade in tower.availableUpgrades)
            {
                _upgrades.Add(upgrade);
            }

            if (tower.innateEffects.Count > 0)
            {
                towerEffect =
                    tower.innateEffects[0].effectType;

                effectCount =
                    (int)tower.innateEffects[0].value;
            }

            if (tower.attackData != null)
            {
                damageType =
                    tower.attackData.damageType;

                projectileType =
                    tower.attackData.projectileType;

                if (tower.attackData.projectile != null)
                {
                    projectile =
                        tower.attackData.projectile;

                    projectileSpeed =
                        projectile.speed;
                }
            }
        }

        #endregion

        #region Tower Balance Setup

        private void TowerBalanceSetup()
        {
            TowerSelection();

            if (selectedTower == null)
            {
                EditorGUILayout.HelpBox(
                    "No tower selected.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "General Tower Stats",
                EditorStyles.boldLabel
            );

            towerName = EditorGUILayout.TextField(
                new GUIContent(
                    "Tower Name",
                    "The name the tower is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty</b>"
                ),
                towerName
            );

            icon = EditorGUILayout.ObjectField(
                new GUIContent(
                    "Icon",
                    "The icon the tower is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty</b>"
                ),
                icon,
                typeof(Sprite),
                false
            ) as Sprite;

            maxHitPoints = EditorGUILayout.Slider(
                new GUIContent(
                    "Maximum HP",
                    "The maximum amount of hit points this tower is supposed to have in-game.\n" +
                    "<b>This value CANNOT be 0.</b>"
                ),
                maxHitPoints,
                0f,
                100f
            );

            damage = (short)EditorGUILayout.Slider(
                new GUIContent(
                    "Damage",
                    "The Damage amount this tower is supposed to deal per shot.\n" +
                    "<b>This value CANNOT be 0.</b>"
                ),
                damage,
                0f,
                50f
            );

            damageType = (TowerDamageType)EditorGUILayout.EnumPopup(
                new GUIContent(
                    "Damage Type",
                    "The type of damage this tower is supposed to deal.\n" +
                    "<b>Pierce:</b> .\n" +
                    "<b>Normal:</b> .\n" +
                    "<b>Impact:</b> .\n"
                ),
                damageType
            );

            range = EditorGUILayout.Slider(
                new GUIContent(
                    "Attack Range",
                    "The range in which the tower can attack the enemies.\n" +
                    "<b>This value CANNOT be 0.</b>"
                ),
                range,
                0f,
                15f
            );

            attacksPerSecond = EditorGUILayout.Slider(
                new GUIContent(
                    "Attacks per Second",
                    "How many attacks this tower is supposed to do per second.\n" +
                    "<b>This value CANNOT be 0.</b>"
                ),
                attacksPerSecond,
                0f,
                5f
            );

            energyUsage = EditorGUILayout.IntSlider(
                new GUIContent(
                    "Energy Usage",
                    "How much energy does this tower require to keep running.\n" +
                    "<b>This value CANNOT be 0.</b>"
                ),
                energyUsage,
                0,
                10
            );

            projectileAmount = EditorGUILayout.IntSlider(
                new GUIContent(
                    "Projectile Amount",
                    "How many projectiles this tower fires per attack.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                projectileAmount,
                0,
                10
            );
        }

        #endregion

        #region Projectile Balance Setup

        private void ProjectileBalanceSetup()
        {
            if (selectedTower == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a tower first.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.LabelField(
                "Projectile Settings",
                EditorStyles.boldLabel
            );

            projectileType =
                (TowerProjectileType)EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Projectile Type",
                        "The type of projectile this tower shoots.\n" +
                        "<b>Basketball:</b> .\n" +
                        "<b>Baseball:</b> .\n"
                    ),
                    projectileType
                );

            projectileAmount =
                EditorGUILayout.IntSlider(
                    new GUIContent(
                        "Projectile Amount",
                        "How many projectiles this tower fires per attack.\n" +
                        "<b>This value cannot be 0.</b>"
                    ),
                    projectileAmount,
                    0,
                    10
                );

            projectileSpeed =
                EditorGUILayout.Slider(
                    new GUIContent(
                        "Projectile Speed",
                        "The speed at which the projectile travels.\n" +
                        "<b>This value CANNOT be 0.</b>"
                    ),
                    projectileSpeed,
                    0f,
                    25f
                );

            EditorGUILayout.Space();
        }

        #endregion

        #region Effect Balance Setup

        private void EffectBalanceSetup()
        {
            if (selectedTower == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a tower first.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.LabelField(
                "Tower Effects",
                EditorStyles.boldLabel
            );

            towerEffect =
                (TowerEffectType)EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Tower Effect",
                        "The Initial effect this tower is supposed to have.\n" +
                        "<b>Additional Targets:</b> This tower can attack multiple enemies at once.\n" +
                        "<b>Slow Percent:</b> This tower slows enemies on attack.\n" +
                        "<b>Bounce Count:</b> This towers projectile bounces in between enemies."
                    ),
                    towerEffect
                );

            effectCount =
                EditorGUILayout.IntSlider(
                    new GUIContent(
                        "Effect amount",
                        "How many times the effect can occur/How strong the effect is."
                    ),
                    effectCount,
                    0,
                    50
                );
        }

        #endregion

        #region Upgrade Balance Setup

        private void UpgradeBalanceSetup()
        {
            if (selectedTower == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a tower first.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.LabelField(
                "Available Upgrades",
                EditorStyles.boldLabel
            );

            DrawCurrentUpgrades();

            EditorGUILayout.Space();

            DrawAddUpgrade();
        }

        private void DrawCurrentUpgrades()
        {
            if (selectedTower.availableUpgrades.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "This tower has no upgrades.",
                    MessageType.Info
                );

                return;
            }

            for (int i = 0; i < selectedTower.availableUpgrades.Count; i++)
            {
                var upgrade =
                    selectedTower.availableUpgrades[i];

                EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.LabelField(
                    upgrade.upgradeName
                );

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(80)))
                {
                    selectedTower.availableUpgrades.RemoveAt(i);

                    EditorUtility.SetDirty(selectedTower);

                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawAddUpgrade()
        {
            EditorGUILayout.LabelField(
                "Add Upgrade",
                EditorStyles.boldLabel
            );

            if (_allUpgrades.Count == 0)
            {
                _allUpgrades = FetchUpgrades();
            }

            _upgradeToAdd =
                (TowerUpgradeSO)EditorGUILayout.ObjectField(
                    "Upgrade",
                    _upgradeToAdd,
                    typeof(TowerUpgradeSO),
                    false
                );

            if (_upgradeToAdd == null)
            {
                return;
            }

            if (selectedTower.availableUpgrades.Contains(_upgradeToAdd))
            {
                EditorGUILayout.HelpBox(
                    "Upgrade already assigned.",
                    MessageType.Warning
                );

                return;
            }

            if (GUILayout.Button("Add Upgrade"))
            {
                selectedTower.availableUpgrades.Add(
                    _upgradeToAdd
                );


                EditorUtility.SetDirty(selectedTower);


                _upgradeToAdd = null;
            }
        }

        #endregion

        #region Saving

        private void UpdateValues()
        {
            _hasErrors = CheckValidInput();

            if (selectedTower == null)
            {
                _warnings.Add("No tower selected.");
                _hasErrors = true;
                return;
            }

            _hasErrors = false;

            selectedTower.towerName = towerName;
            selectedTower.icon = icon;
            selectedTower.baseStats.maxHp = maxHitPoints;
            selectedTower.baseStats.damage = damage;
            selectedTower.baseStats.range = range;
            selectedTower.baseStats.attacksPerSecond = attacksPerSecond;
            selectedTower.baseStats.energy = energyUsage;
            selectedTower.baseStats.baseProjectileAmount = projectileAmount;

            if (selectedTower.innateEffects.Count > 0)
            {
                var effect = selectedTower.innateEffects[0];
                effect.effectType = towerEffect;
                effect.value = effectCount;

                selectedTower.innateEffects[0] = effect;
            }

            if (selectedTower.attackData != null)
            {
                selectedTower.attackData.damageType = damageType;
                selectedTower.attackData.projectileType = projectileType;
                if (selectedTower.attackData.projectile != null)
                {
                    selectedTower.attackData.projectile.speed = (byte)projectileSpeed;
                }
            }

            selectedTower.availableUpgrades.Clear();

            foreach (var upgrade in _upgrades)
            {
                selectedTower.availableUpgrades.Add(upgrade);
            }

            EditorUtility.SetDirty(selectedTower);

            if (selectedTower.attackData != null)
            {
                EditorUtility.SetDirty(selectedTower.attackData);
            }

            if (selectedTower.attackData?.projectile != null)
            {
                EditorUtility.SetDirty(selectedTower.attackData.projectile);
            }

            foreach (var upgrade in selectedTower.availableUpgrades)
            {
                EditorUtility.SetDirty(upgrade);
            }

            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Validation

        private bool CheckValidInput()
        {
            _warnings.Clear();

            if (selectedTower == null)
            {
                _warnings.Add("No tower selected.");
            }

            if (string.IsNullOrWhiteSpace(towerName))
            {
                _warnings.Add("Tower name cannot be empty.");
            }

            if (maxHitPoints <= 0)
            {
                _warnings.Add("Invalid tower HP.");
            }

            if (damage <= 0)
            {
                _warnings.Add("Invalid tower damage.");
            }

            if (range <= 0)
            {
                _warnings.Add("Invalid tower range.");
            }

            if (attacksPerSecond <= 0)
            {
                _warnings.Add("Invalid attacks per second.");
            }

            if (energyUsage <= 0)
            {
                _warnings.Add("Invalid energy usage.");
            }

            if (projectileAmount <= 0)
            {
                _warnings.Add("Invalid projectile amount.");
            }

            if (projectileSpeed <= 0)
            {
                _warnings.Add("Invalid projectile speed.");
            }

            return _warnings.Count > 0;
        }

        private void ShowWarnings(List<string> warnings)
        {
            foreach (var warning in warnings)
            {
                EditorGUILayout.HelpBox(
                    warning,
                    MessageType.Error
                );
            }
        }

        #endregion

        #region Utility

        private void ReloadSelectedTower()
        {
            if (selectedTower == null)
                return;
            LoadTower(selectedTower);
        }

        private void OnDisable()
        {
            _warnings.Clear();
            _availableTowers.Clear();
            _upgrades.Clear();
        }

        private List<TowerUpgradeSO> FetchUpgrades()
        {
            var guids = AssetDatabase.FindAssets(
                "t:TowerUpgradeSO"
            );


            var upgrades = new List<TowerUpgradeSO>();


            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var upgrade =
                    AssetDatabase.LoadAssetAtPath<TowerUpgradeSO>(path);


                if (upgrade != null)
                {
                    upgrades.Add(upgrade);
                }
            }


            return upgrades;
        }

        #endregion
    }
}
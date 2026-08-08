using System.Collections.Generic;
using _01_Scripts._07_Enemy.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyBalancingTool : EditorWindow
    {
        // Selected Enemy
        [SerializeField] private EnemyBaseSO selectedEnemy;

        // General Settings
        [SerializeField] private string enemyName;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int hitpoints;
        [SerializeField] private float damage;
        [SerializeField] private float range;
        [SerializeField] private float attacksPerSecond;

        // Movement Settings
        [SerializeField] private EnemyMovementTypes movement;
        [SerializeField] private float movementSpeed;

        // Armor Settings
        [SerializeField] private EnemyArmorTypes armor;
        [SerializeField] private float armorStrength;

        // Passive Settings
        [SerializeField] private EnemyPassiveTypes passive;
        [SerializeField] private float passiveStrength;

        // Ability Settings
        [SerializeField] private EnemyAbilityTypes ability;
        [SerializeField] private float amount;
        [SerializeField] private float frequency;
        [SerializeField] private GameObject summonPrefab;

        // Editor Window
        private Page _currentPage;

        // Available Enemies
        private List<EnemyBaseSO> _availableEnemies = new();

        // Warnings
        private readonly List<string> _warnings = new();
        private bool _hasErrors;

        // Paths
        private const string EnemySOPath = "Assets/03_SO/Enemies";

        #region Editor

        [MenuItem("Balancing Tools/Enemy Balancing")]
        public static void ShowWindow()
        {
            var window = GetWindow<EnemyBalancingTool>();

            window.titleContent = new GUIContent("Enemy Balancing");

            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
            window._availableEnemies = window.FetchEnemies();
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(
                GUILayout.Width(400)
            );

            if (GUILayout.Button("Enemy Settings"))
            {
                _currentPage = Page.Enemy;
            }

            if (GUILayout.Button("Passives"))
            {
                _currentPage = Page.Passives;
            }

            if (GUILayout.Button("Abilities"))
            {
                _currentPage = Page.Abilities;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            switch (_currentPage)
            {
                case Page.Enemy:
                    EnemyBalanceSetup();
                    break;

                case Page.Passives:
                    PassiveBalanceSetup();
                    break;

                case Page.Abilities:
                    AbilityBalanceSetup();
                    break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (selectedEnemy != null)
            {
                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Save Changes"))
                {
                    _hasErrors = CheckValidInput();

                    if (!_hasErrors)
                    {
                        UpdateChanges();
                    }
                }

                if (GUILayout.Button("Reload Enemy"))
                {
                    ReloadSelectedEnemy();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (_hasErrors)
            {
                ShowWarning(_warnings);
            }
        }

        #endregion

        #region Enemy Balance Setup

        private void EnemyBalanceSetup()
        {
            EnemySelection();

            if (selectedEnemy == null)
            {
                EditorGUILayout.HelpBox(
                    "No enemy selected.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "General Enemy Settings",
                EditorStyles.boldLabel
            );

            enemyName = EditorGUILayout.TextField(
                new GUIContent(
                    "Enemy Name",
                    "The name this enemy is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty.</b>"
                ),
                enemyName
            );

            enemyPrefab = EditorGUILayout.ObjectField(
                new GUIContent(
                    "Enemy Prefab",
                    "The visual prefab for this enemy.\n" +
                    "<b>This cannot be left empty.</b>"
                ),
                enemyPrefab,
                typeof(GameObject),
                false
            ) as GameObject;

            hitpoints = EditorGUILayout.IntSlider(
                new GUIContent(
                    "Maximum HP",
                    "The amount of hit points this enemy is supposed to have.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                hitpoints,
                0,
                500
            );

            damage = EditorGUILayout.Slider(
                new GUIContent(
                    "Damage",
                    "The amount of damage this enemy is supposed to deal.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                damage,
                0f,
                100f
            );

            range = EditorGUILayout.Slider(
                new GUIContent(
                    "Range",
                    "The attack range of this enemy.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                range,
                0f,
                50f
            );

            attacksPerSecond = EditorGUILayout.Slider(
                new GUIContent(
                    "Attacks per Second",
                    "How many attacks this enemy can do per second.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                attacksPerSecond,
                0f,
                20f
            );

            movement = (EnemyMovementTypes)EditorGUILayout.EnumPopup(
                new GUIContent(
                    "Movement Type",
                    "How is this enemy supposed to move."
                ),
                movement
            );

            movementSpeed = EditorGUILayout.Slider(
                new GUIContent(
                    "Movement Speed",
                    "How fast is this enemy supposed to be moving?\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                movementSpeed,
                0f,
                10f
            );

            armor = (EnemyArmorTypes)EditorGUILayout.EnumPopup(
                new GUIContent(
                    "Armor Type",
                    "The type of armor the enemy is supposed to have."
                ),
                armor
            );

            armorStrength = EditorGUILayout.Slider(
                new GUIContent(
                    "Armor Strength",
                    "The strength of the armor. A higher strength means less damage.\n" +
                    "<b>This value cannot be 0.</b>"
                ),
                armorStrength,
                0f,
                75f
            );
        }

        #endregion

        #region Enemy Selection

        private void EnemySelection()
        {
            if (_availableEnemies.Count == 0)
            {
                _availableEnemies = FetchEnemies();
            }

            if (_availableEnemies.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No enemies found.",
                    MessageType.Warning
                );

                return;
            }

            string[] enemyNames = new string[_availableEnemies.Count];

            for (int i = 0; i < _availableEnemies.Count; i++)
            {
                enemyNames[i] =
                    _availableEnemies[i].enemyName;
            }

            int currentIndex = _availableEnemies.IndexOf(selectedEnemy);

            int newIndex =
                EditorGUILayout.Popup(
                    new GUIContent(
                        "Enemy",
                        "Select the enemy you want to balance."
                    ),
                    currentIndex < 0 ? 0 : currentIndex,
                    enemyNames
                );

            if (selectedEnemy == null ||
                newIndex != currentIndex)
            {
                ReloadSelectedEnemy(
                    _availableEnemies[newIndex]
                );
            }
        }

        private List<EnemyBaseSO> FetchEnemies()
        {
            var guids =
                AssetDatabase.FindAssets(
                    "t:EnemyBaseSO",
                    new[] { EnemySOPath }
                );

            var enemies = new List<EnemyBaseSO>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var enemy = AssetDatabase.LoadAssetAtPath<EnemyBaseSO>(path);

                if (enemy != null)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        private void ReloadSelectedEnemy(
            EnemyBaseSO enemy)
        {
            if (enemy == null) return;

            selectedEnemy = enemy;

            enemyName = selectedEnemy.enemyName;
            enemyPrefab = selectedEnemy.prefab;

            hitpoints = selectedEnemy.stats.maxHp;
            damage = selectedEnemy.stats.damage;
            range = selectedEnemy.stats.range;
            attacksPerSecond = selectedEnemy.stats.attacksPerSecond;


            movement = selectedEnemy.stats.movement.movementType;
            movementSpeed = selectedEnemy.stats.movement.moveSpeed;

            armor = selectedEnemy.stats.armor.armorType;
            armorStrength = selectedEnemy.stats.armor.armorValue;

            _warnings.Clear();
            _hasErrors = false;

            Repaint();
        }


        private void ReloadSelectedEnemy()
        {
            if (selectedEnemy == null) return;

            ReloadSelectedEnemy(selectedEnemy);
        }

        #endregion

        #region Passive Balance Setup

        private void PassiveBalanceSetup()
        {
            if (selectedEnemy == null)
            {
                EditorGUILayout.HelpBox(
                    "No enemy selected.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.LabelField(
                "Current Passives",
                EditorStyles.boldLabel
            );

            DrawCurrentPassives();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Add Passive",
                EditorStyles.boldLabel
            );

            DrawAddPassive();
        }


        private void DrawCurrentPassives()
        {
            if (selectedEnemy.passives.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "This enemy has no passives.",
                    MessageType.Info
                );

                return;
            }

            for (int i = 0;
                 i < selectedEnemy.passives.Count;
                 i++)
            {
                var localPassive = selectedEnemy.passives[i];

                EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.LabelField(
                    localPassive.enemyPassive.ToString(),
                    GUILayout.Width(150)
                );

                float newStrength =
                    EditorGUILayout.Slider(
                        localPassive.effectValue,
                        0f,
                        5f
                    );

                if (!Mathf.Approximately(newStrength, localPassive.effectValue))
                {
                    localPassive.effectValue = newStrength;

                    selectedEnemy.passives[i] =
                        localPassive;

                    EditorUtility.SetDirty(
                        selectedEnemy
                    );
                }

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(80)))
                {
                    selectedEnemy.passives.RemoveAt(i);

                    EditorUtility.SetDirty(
                        selectedEnemy
                    );

                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }


        private void DrawAddPassive()
        {
            passive = (EnemyPassiveTypes)EditorGUILayout.EnumPopup(new GUIContent(
                    "Passive Type",
                    "Choose a passive to add to this enemy."
                ),
                passive
            );

            passiveStrength =
                EditorGUILayout.Slider(
                    new GUIContent(
                        "Passive Strength",
                        "How strong the effect of the passive is."
                    ),
                    passiveStrength,
                    0f,
                    5f
                );


            if (GUILayout.Button("Add Passive"))
            {
                var newPassive =
                    new EnemyPassive
                    {
                        enemyPassive = passive,
                        effectValue = passiveStrength
                    };

                selectedEnemy.passives.Add(
                    newPassive
                );

                EditorUtility.SetDirty(
                    selectedEnemy
                );

                passiveStrength = 0f;
            }
        }

        #endregion

        #region Ability Balance Setup

        private void AbilityBalanceSetup()
        {
            if (selectedEnemy == null)
            {
                EditorGUILayout.HelpBox(
                    "No enemy selected.",
                    MessageType.Warning
                );

                return;
            }

            EditorGUILayout.LabelField(
                "Current Abilities",
                EditorStyles.boldLabel
            );

            DrawCurrentAbilities();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Add Ability",
                EditorStyles.boldLabel
            );

            DrawAddAbility();
        }


        private void DrawCurrentAbilities()
        {
            if (selectedEnemy.abilities.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "This enemy has no abilities.",
                    MessageType.Info
                );

                return;
            }

            for (int i = 0;
                 i < selectedEnemy.abilities.Count;
                 i++)
            {
                var localAbility = selectedEnemy.abilities[i];

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(
                    localAbility.enemyAbility.ToString(),
                    EditorStyles.boldLabel
                );

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(80)))
                {
                    selectedEnemy.abilities.RemoveAt(i);

                    EditorUtility.SetDirty(
                        selectedEnemy
                    );

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    break;
                }

                EditorGUILayout.EndHorizontal();

                float newAmount =
                    EditorGUILayout.Slider(
                        new GUIContent(
                            "Amount",
                            "How many times the ability is triggered in a single use."
                        ),
                        localAbility.amount,
                        0f,
                        10f
                    );

                float newFrequency =
                    EditorGUILayout.Slider(
                        new GUIContent(
                            "Frequency",
                            "How often the ability is cast."
                        ),
                        localAbility.frequency,
                        0f,
                        10f
                    );

                GameObject newSummonPrefab =
                    EditorGUILayout.ObjectField(
                        new GUIContent(
                            "Summon Type",
                            "What monster is summoned by the ability."
                        ),
                        localAbility.summonedMonster,
                        typeof(GameObject),
                        false
                    ) as GameObject;

                if (!Mathf.Approximately(newAmount, localAbility.amount) ||
                    !Mathf.Approximately(newFrequency, localAbility.frequency) ||
                    newSummonPrefab != localAbility.summonedMonster)
                {
                    localAbility.amount = newAmount;
                    localAbility.frequency = newFrequency;
                    localAbility.summonedMonster = newSummonPrefab;

                    selectedEnemy.abilities[i] =
                        localAbility;

                    EditorUtility.SetDirty(
                        selectedEnemy
                    );
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawAddAbility()
        {
            ability =
                (EnemyAbilityTypes)EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Ability",
                        "Choose an ability to add to this enemy."
                    ),
                    ability
                );

            amount =
                EditorGUILayout.Slider(
                    new GUIContent(
                        "Amount",
                        "How many times the ability is triggered in a single use."
                    ),
                    amount,
                    0f,
                    10f
                );

            frequency =
                EditorGUILayout.Slider(
                    new GUIContent(
                        "Frequency",
                        "How often the ability is cast."
                    ),
                    frequency,
                    0f,
                    10f
                );

            summonPrefab =
                EditorGUILayout.ObjectField(
                    new GUIContent(
                        "Summon Type",
                        "What monster is summoned by the ability."
                    ),
                    summonPrefab,
                    typeof(GameObject),
                    false
                ) as GameObject;

            if (GUILayout.Button("Add Ability"))
            {
                var newAbility =
                    new EnemyAbility
                    {
                        enemyAbility = ability,
                        amount = amount,
                        frequency = frequency,
                        summonedMonster = summonPrefab
                    };

                selectedEnemy.abilities.Add(newAbility);

                EditorUtility.SetDirty(selectedEnemy);

                amount = 0f;
                frequency = 0f;
                summonPrefab = null;
            }
        }

        #endregion

        #region Input Validation

        private bool CheckValidInput()
        {
            _warnings.Clear();

            if (selectedEnemy == null)
            {
                _warnings.Add(
                    "No Enemy Selected"
                );

                return true;
            }

            if (string.IsNullOrWhiteSpace(enemyName))
            {
                _warnings.Add(
                    "Missing Enemy Name"
                );
            }

            if (enemyPrefab == null)
            {
                _warnings.Add(
                    "Missing Enemy Prefab"
                );
            }

            if (hitpoints <= 0)
            {
                _warnings.Add(
                    "Invalid Enemy HP"
                );
            }

            if (damage <= 0)
            {
                _warnings.Add(
                    "Invalid Enemy Damage"
                );
            }

            if (range <= 0)
            {
                _warnings.Add(
                    "Invalid Enemy Range"
                );
            }

            if (attacksPerSecond <= 0)
            {
                _warnings.Add(
                    "Invalid Attacks Per Second"
                );
            }

            if (movementSpeed <= 0)
            {
                _warnings.Add(
                    "Invalid Movement Speed"
                );
            }

            if (armorStrength <= 0)
            {
                _warnings.Add(
                    "Invalid Armor Strength"
                );
            }

            return _warnings.Count != 0;
        }


        private void ShowWarning(List<string> warnings)
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

        #region Save

        private void UpdateChanges()
        {
            if (selectedEnemy == null)
                return;

            selectedEnemy.enemyName = enemyName;
            selectedEnemy.prefab = enemyPrefab;


            selectedEnemy.stats.maxHp = hitpoints;
            selectedEnemy.stats.damage = damage;
            selectedEnemy.stats.range = range;
            selectedEnemy.stats.attacksPerSecond = attacksPerSecond;

            selectedEnemy.stats.movement.movementType = movement;
            selectedEnemy.stats.movement.moveSpeed = movementSpeed;

            selectedEnemy.stats.armor.armorType = armor;
            selectedEnemy.stats.armor.armorValue = armorStrength;

            EditorUtility.SetDirty(selectedEnemy);
            AssetDatabase.SaveAssets();

            _warnings.Clear();
            _hasErrors = false;
        }

        #endregion
    }
}
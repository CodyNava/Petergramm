using System.Collections.Generic;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.DamageRules;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyCreationTool : EditorWindow
    {
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

        // For Creation
        private EnemyBaseSO _createdEnemyBase;

        private readonly List<EnemyPassive> _passives = new();
        private readonly List<EnemyAbility> _abilities = new();

        // Editor Window
        private Page _currentPage;

        // Passive State
        private bool _showPassives;
        private bool _newPassive;

        // Ability State
        private bool _showAbilities;
        private bool _newAbility;

        // Warnings
        private readonly List<string> _warnings = new();
        private bool _hasErrors;

        // Paths
        private const string EnemySOPath = "Assets/03_SO/Enemies";
        private const string DamageRulesPath = "Assets/03_SO/EquationRules/DamageRules.asset";
        private const string EnemyPrefabPath = "Assets/04_Prefabs/Enemies";
        
        #region Editor

        [MenuItem("Creation Tools/Enemy Creation")]
        public static void ShowWindow()
        {
            var window = GetWindow<EnemyCreationTool>();
            window.titleContent = new GUIContent("Enemy Creation");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(400));

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
                    EnemySetup();
                    break;

                case Page.Passives:
                    PassivesSetup();
                    break;

                case Page.Abilities:
                    AbilitiesSetup();
                    break;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Enemy"))
            {
                _hasErrors = CheckValidInput();

                if (!_hasErrors)
                {
                    CreateEnemyFolders();
                    CreateEnemy();
                    AssetDatabase.SaveAssets();
                }
            }

            if (_hasErrors)
            {
                ShowWarning(_warnings);
            }
        }


        private void CreateEnemyFolders()
        {
            var enemyPath = $"{EnemySOPath}/{enemyName}";

            if (!AssetDatabase.IsValidFolder(enemyPath))
            {
                AssetDatabase.CreateFolder(
                    EnemySOPath,
                    enemyName
                );
            }

            var prefabPath = $"{EnemyPrefabPath}/{enemyName}";

            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder(
                    EnemyPrefabPath,
                    enemyName
                );
            }
        }

        #endregion

        #region Setup

        private void EnemySetup()
        {
            EditorGUILayout.LabelField("General Enemy Settings", EditorStyles.boldLabel);


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

        private void PassivesSetup()
        {
            _showPassives =
                EditorGUILayout.Foldout(
                    _showPassives,
                    "Passives"
                );

            if (!_showPassives)
                return;

            EditorGUILayout.LabelField(
                "Selected Passives",
                EditorStyles.boldLabel
            );

            DrawSelectedPassives();

            EditorGUILayout.Space();

            DrawAddPassive();
        }

        private void DrawSelectedPassives()
        {
            if (_passives.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No passives assigned.",
                    MessageType.Info
                );

                return;
            }

            for (int i = 0; i < _passives.Count; i++)
            {
                var currentPassive = _passives[i];

                EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.LabelField(
                    currentPassive.enemyPassive.ToString()
                );

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(80)))
                {
                    _passives.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawAddPassive()
        {
            EditorGUILayout.LabelField("Add Passive", EditorStyles.boldLabel);

            if (!_newPassive)
            {
                if (GUILayout.Button("Add Passive"))
                {
                    _newPassive = true;
                }

                return;
            }

            passive = (EnemyPassiveTypes)EditorGUILayout.EnumPopup(
                new GUIContent(
                    "Passive Type",
                    "Choose one passive for the enemy."
                ),
                passive
            );

            passiveStrength = EditorGUILayout.Slider(
                new GUIContent(
                    "Passive Strength",
                    "How strong the effect of your passive is."
                ),
                passiveStrength,
                0f,
                5f
            );

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Passive"))
            {
                if (CheckPassiveInput())
                {
                    var newPassive = new EnemyPassive
                    {
                        enemyPassive = passive,
                        effectValue = passiveStrength
                    };

                    _passives.Add(newPassive);
                    _newPassive = false;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                _newPassive = false;
            }

            EditorGUILayout.EndHorizontal();
        }


        private void AbilitiesSetup()
        {
            _showAbilities = EditorGUILayout.Foldout(_showAbilities, "Abilities");

            if (!_showAbilities) return;

            EditorGUILayout.LabelField("Selected Abilities", EditorStyles.boldLabel);

            DrawSelectedAbilities();

            EditorGUILayout.Space();

            DrawAddAbility();
        }

        private void DrawSelectedAbilities()
        {
            if (_abilities.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No abilities assigned.",
                    MessageType.Info
                );
                return;
            }

            for (int i = 0; i < _abilities.Count; i++)
            {
                var currentAbility = _abilities[i];

                EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.LabelField(
                    currentAbility.enemyAbility.ToString()
                );

                if (GUILayout.Button(
                        "Remove",
                        GUILayout.Width(80)))
                {
                    _abilities.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawAddAbility()
        {
            EditorGUILayout.LabelField("Add Ability", EditorStyles.boldLabel);

            if (!_newAbility)
            {
                if (GUILayout.Button("Add Ability"))
                {
                    _newAbility = true;
                }

                return;
            }

            ability = (EnemyAbilityTypes)EditorGUILayout.EnumPopup(
                new GUIContent(
                    "Ability",
                    "Choose one ability for the enemy."
                ),
                ability
            );

            amount = EditorGUILayout.Slider(
                new GUIContent(
                    "Amount",
                    "How many times the ability is triggered in a single use."
                ),
                amount,
                0f,
                10f
            );

            frequency = EditorGUILayout.Slider(
                new GUIContent(
                    "Frequency",
                    "How often the ability is cast."
                ),
                frequency,
                0f,
                10f
            );

            if (ability == EnemyAbilityTypes.Summoning)
            {
                summonPrefab = EditorGUILayout.ObjectField(
                    new GUIContent(
                        "Summon Type",
                        "What monster is summoned by the ability"
                    ),
                    summonPrefab,
                    typeof(GameObject),
                    false
                ) as GameObject;
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Ability"))
            {
                if (CheckAbilityInput())
                {
                    var newAbility = new EnemyAbility
                    {
                        enemyAbility = ability,
                        amount = amount,
                        frequency = frequency,
                        summonedMonster = summonPrefab
                    };

                    _abilities.Add(newAbility);
                    _newAbility = false;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                _newAbility = false;
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Creation

        private void CreateEnemy()
        {
            _createdEnemyBase = CreateInstance<EnemyBaseSO>();

            CreateEnemyPrefab();
            CreateEnemyAsset();
            AddConnections();
        }


        private void CreateEnemyPrefab()
        {
            if (enemyPrefab == null) return;

            var tempEnemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);

            tempEnemy.AddComponent<EnemyRuntime>();
            var prefabPath = $"{EnemyPrefabPath}/{enemyName}/{enemyName}.prefab";

            PrefabUtility.SaveAsPrefabAsset(tempEnemy, prefabPath);
            DestroyImmediate(tempEnemy);
        }


        private void CreateEnemyAsset()
        {
            var enemyPath = $"{EnemySOPath}/{enemyName}/{enemyName}.asset";

            AssetDatabase.CreateAsset(_createdEnemyBase, enemyPath);

            _createdEnemyBase.enemyName = enemyName;
            _createdEnemyBase.prefab = enemyPrefab;
            _createdEnemyBase.damageRules = AssetDatabase.LoadAssetAtPath<DamageEquationDataSO>(DamageRulesPath);

            _createdEnemyBase.stats.maxHp = hitpoints;
            _createdEnemyBase.stats.damage = damage;
            _createdEnemyBase.stats.range = range;
            _createdEnemyBase.stats.attacksPerSecond = attacksPerSecond;
            _createdEnemyBase.stats.movement.movementType = movement;
            _createdEnemyBase.stats.movement.moveSpeed = movementSpeed;
            _createdEnemyBase.stats.armor.armorType = armor;
            _createdEnemyBase.stats.armor.armorValue = armorStrength;

            foreach (var enemyPassive in _passives)
            {
                _createdEnemyBase.passives.Add(
                    enemyPassive
                );
            }

            foreach (var enemyAbility in _abilities)
            {
                _createdEnemyBase.abilities.Add(
                    enemyAbility
                );
            }
        }


        private void AddConnections()
        {
            var prefabPath = $"{EnemyPrefabPath}/{enemyName}/{enemyName}.prefab";

            var enemyPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (enemyPrefabAsset == null) return;

            var runtime = enemyPrefabAsset.GetComponent<EnemyRuntime>();

            if (runtime == null) return;

            runtime.EnemyBase = _createdEnemyBase;

            PrefabUtility.SavePrefabAsset(enemyPrefabAsset);
        }

        #endregion

        #region Input Validation

        private bool CheckValidInput()
        {
            _warnings.Clear();

            if (string.IsNullOrWhiteSpace(enemyName))
            {
                _warnings.Add("Missing Enemy Name");
            }

            if (enemyPrefab == null)
            {
                _warnings.Add("Missing Enemy Prefab");
            }

            if (hitpoints <= 0)
            {
                _warnings.Add("Invalid Enemy HP");
            }

            if (damage <= 0)
            {
                _warnings.Add("Invalid Enemy Damage");
            }

            if (range <= 0)
            {
                _warnings.Add("Invalid Enemy Range");
            }

            if (attacksPerSecond <= 0)
            {
                _warnings.Add("Invalid Attacks Per Second");
            }

            if (movementSpeed <= 0)
            {
                _warnings.Add("Invalid Movement Speed");
            }

            if (armorStrength <= 0)
            {
                _warnings.Add("Invalid Armor Strength");
            }

            var enemyPath = $"{EnemySOPath}/{enemyName}/{enemyName}.asset";

            if (AssetDatabase.LoadAssetAtPath<EnemyBaseSO>(enemyPath) != null)
            {
                _warnings.Add("Enemy already exists.");
            }

            return _warnings.Count != 0;
        }

        private bool CheckPassiveInput()
        {
            _warnings.Clear();

            if (passiveStrength <= 0)
            {
                _warnings.Add("Invalid Passive Strength");
            }

            return _warnings.Count == 0;
        }

        private bool CheckAbilityInput()
        {
            _warnings.Clear();

            if (amount <= 0)
            {
                _warnings.Add("Invalid Ability Amount");
            }

            if (frequency <= 0)
            {
                _warnings.Add("Invalid Ability Frequency");
            }

            if (ability == EnemyAbilityTypes.Summoning &&
                summonPrefab == null)
            {
                _warnings.Add("Missing Summon Prefab");
            }

            return _warnings.Count == 0;
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

        #region Misc

        private void ResetCreationState()
        {
            _createdEnemyBase = null;

            _passives.Clear();
            _abilities.Clear();

            _newPassive = false;
            _newAbility = false;

            _warnings.Clear();

            _hasErrors = false;
        }

        private void OnDisable()
        {
            ResetCreationState();
        }

        #endregion
    }
}
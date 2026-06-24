using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }
    private Character current = null;

    [SerializeField]
    private GameObject attackPosition;
    private TurnSystem turnSystem;

    [Header("Battle Participants")]
    [SerializeField]
    private List<PlayerStats> playerStats;

    [SerializeField]
    private PlayerHealthUI playerHealthUI;

    [SerializeField]
    private List<EnemyStats> enemyStats;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform[] playerSpawnPoints;

    [SerializeField]
    private Transform[] enemySpawnPoints;

    [Header("Runtime Battle Data")]
    public List<Character> characters = new List<Character>();
    public List<Character> enemyCharacters = new List<Character>();
    public List<CharacterView> characterViews = new List<CharacterView>();

    [SerializeField]
    private GameObject turnArrow;
    private GameObject activeArrow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (GameStateManager.Instance.HasSaveData())
            GameStateManager.Instance.LoadGame();
        else
            GameStateManager.Instance.NewGame();

        characterViews.Clear();

        for (int i = 0; i < playerStats.Count; i++)
        {
            var c = playerStats[i];
            Character character;
            if (GameStateManager.Instance.HasSaveData())
            {
                var savedChar = GameStateManager.Instance.CurrentData.Characters.FirstOrDefault(s =>
                    s.name == c.Name
                );
                if (savedChar != null)
                {
                    character = CharacterFactory.CreateFromSaveData(savedChar);
                    character.Actions = c.Actions;
                }
                else
                {
                    character = CharacterFactory.CreateFromPlayerStats(c);
                }
            }
            else
            {
                character = CharacterFactory.CreateFromPlayerStats(c);
            }
            characters.Add(character);
            playerHealthUI.Initialize(character);

            if (c.prefab != null && i < playerSpawnPoints.Length)
            {
                var view = InstantiateCharacterView(
                    c.prefab,
                    character.Name,
                    playerSpawnPoints[i].position
                );
                character.OnDeath += () => view.PlayDeathAnimation();
                characterViews.Add(view);
            }
        }

        var nameCount = new Dictionary<string, int>();
        foreach (var c in enemyStats)
        {
            if (!nameCount.ContainsKey(c.Name))
                nameCount[c.Name] = 0;
            nameCount[c.Name]++;
        }

        var currentCount = new Dictionary<string, int>();
        for (int i = 0; i < enemyStats.Count; i++)
        {
            var c = enemyStats[i];
            var character = CharacterFactory.CreateFromEnemyStats(c);

            var baseName = character.Name;
            if (!currentCount.ContainsKey(baseName))
                currentCount[baseName] = 0;
            currentCount[baseName]++;

            if (nameCount[baseName] > 1)
                character.Name = $"{baseName} {currentCount[baseName]}";

            enemyCharacters.Add(character);

            if (c.prefab != null && i < enemySpawnPoints.Length)
            {
                var view = InstantiateCharacterView(
                    c.prefab,
                    character.Name,
                    enemySpawnPoints[i].position
                );
                var HealthBar = view.transform.parent.GetComponentInChildren<EnemyHealthBar>();

                character.OnHealthChanged += (hp, maxHp) =>
                {
                    float fillAmount = (float)hp / maxHp;
                    HealthBar.UpdateHealthBar(fillAmount);
                };
                character.OnDeath += () => view.PlayDeathAnimation();
                characterViews.Add(view);
            }
        }

        SpecialBarManager.Instance.ResetBar();

        var allCharacters = characters.Concat(enemyCharacters).ToList();
        turnSystem = new TurnSystem(allCharacters);

        turnSystem.OnTurnStarted += HandleTurnStart;
        turnSystem.OnTurnOrderUpdated += HandleTurnOrderUpdated;
        turnSystem.OnTurnEnded += HandleTurnEnd;
        turnSystem.StartTurn();
    }

    private CharacterView InstantiateCharacterView(
        GameObject prefab,
        string characterName,
        Vector3 position
    )
    {
        var instance = Instantiate(prefab, position, Quaternion.identity);
        var view = instance.GetComponentInChildren<CharacterView>();
        if (view != null)
            view.gameObject.name = characterName;
        return view;
    }

    void HandleTurnOrderUpdated(Character[] turnOrder)
    {
        UIManager.Instance.UpdateOrderUI(turnOrder);
    }

    void HandleTurnStart(Character character)
    {
        if (BattleIsOver())
            return;

        current = character;
        Debug.Log($"{character.Name} started their turn.");

        if (current.IsDead)
        {
            Debug.Log($"{current.Name} is dead.");
            turnSystem.EndTurn();
            return;
        }

        // Process status effects that trigger at the start of the turn
        current.ProcessStartOfTurnEffects();

        if (current.IsDead)
        {
            Debug.Log($"{current.Name} died from a status effect.");
            turnSystem.EndTurn();
            return;
        }

        // If the effect killed the character, end their turn immediately

        if (!character.IsPlayerControlled)
        {
            Invoke(nameof(ExecuteEnemyTurn), 1f);
        }
        else
        {
            ShowPlayerActionMenu();
            if (activeArrow == null)
                activeArrow = Instantiate(turnArrow);
            activeArrow
                .GetComponent<TurnIndicator>()
                .SetTarget(GetCharacterView(character).transform);
        }
    }

    bool BattleIsOver()
    {
        var aliveEnemies = enemyCharacters.Where(e => !e.IsDead).ToList();
        if (aliveEnemies.Count == 0)
        {
            BattleEnded();
            return true;
        }

        var alivePlayers = characters.Where(c => c.IsPlayerControlled && !c.IsDead).ToList();
        if (alivePlayers.Count == 0)
        {
            GameOver();
            return true;
        }
        return false;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
    }

    void ExecuteEnemyTurn()
    {
        if (current == null)
            return;

        var alivePlayers = characters.Where(c => c.IsPlayerControlled && !c.IsDead).ToList();

        if (BattleIsOver())
            return;

        var target = alivePlayers[Random.Range(0, alivePlayers.Count)];
        int damage = current.Attack;

        var currentView = GetCharacterView(current);
        var targetView = GetCharacterView(target);

        AttackAction(currentView, target, targetView, damage, -1);
    }

    void BattleEnded()
    {
        Debug.LogError("Battle Won! Distributing XP...");
        int totalExp = enemyCharacters.Sum(e => e.Experience);
        foreach (var player in characters.Where(c => c.IsPlayerControlled))
        {
            player.AddXp(totalExp);
        }

        GameStateManager.Instance.UpdateFromCharacter(characters);
        GameStateManager.Instance.SaveGame();
    }

    void HandleTurnEnd(Character character)
    {
        UIManager.Instance.SetButtonInteractable(false);
        // Process status effects that trigger at end of turn (tick durations)
        character.ProcessEndOfTurnEffects();
        if (activeArrow != null)
            activeArrow.GetComponent<TurnIndicator>().RemoveTarget();

        if (BattleIsOver())
            return;
    }

    public void PlayerChoseAction()
    {
        turnSystem.EndTurn();
    }

    public void AttackAction(
        CharacterView currentView,
        Character target,
        CharacterView targetView,
        int damage,
        int direction = 1
    )
    {
        bool isCrit = false;
        if (current != null && current.IsPlayerControlled)
        {
            float value = Random.value;
            isCrit = value < current.CritChance;
            if (isCrit)
                damage = Mathf.RoundToInt(damage * current.CritMultiplier);
        }

        Vector3 cameraDirection = (
            Camera.main.transform.position - targetView.transform.position
        ).normalized;
        Vector3 attackPos = targetView.transform.position + cameraDirection * 1f;

        currentView.PerformAttackMove(
            currentView.transform.position,
            attackPosition.transform.position,
            damage,
            onAttack: (dmg, effectConfig) =>
            {
                target.TakeDamage(dmg);
                if (currentView != null)
                {
                    VisualEffectManager.Instance.ShowFloatingText(
                        dmg.ToString(),
                        attackPos,
                        isCrit ? Color.yellow : Color.red
                    );
                    targetView.PerformHitReaction(targetView.transform.position, direction);
                    var config = effectConfig;
                    VisualEffectManager.Instance.PlayAttackEffect(
                        targetView.transform.position,
                        currentView.transform.position,
                        isCrit,
                        config
                    );
                }
            },
            onReturnComplete: () =>
            {
                if (characters.Where(c => c.IsPlayerControlled).All(c => c.IsDead))
                {
                    Debug.Log("Game Over!");
                    return;
                }

                turnSystem.EndTurn();
            }
        );
    }

    public void PlayerChoseAttack(int damage)
    {
        if (current == null)
            return;

        if (activeArrow != null)
            activeArrow.GetComponent<TurnIndicator>().RemoveTarget();

        var target = enemyCharacters.FirstOrDefault(e => !e.IsDead);

        var currentView = GetCharacterView(current);
        var targetView = GetCharacterView(target);

        AttackAction(currentView, target, targetView, damage, 1);
    }

    public void PlayerChoseItem(ItemAction item)
    {
        if (current == null || !current.IsPlayerControlled)
            return;

        if (item.targetType != ItemTargetType.Self)
            return;

        var (amount, success) = item.Apply(current);
        var currentView = GetCharacterView(current);
        Vector3 cameraDirection = (
            Camera.main.transform.position - currentView.transform.position
        ).normalized;
        Vector3 numberPos = currentView.transform.position + cameraDirection * 1f;
        VisualEffectManager.Instance.ShowFloatingText(amount.ToString(), numberPos, Color.green);
        VisualEffectManager.Instance.PlayHealEffect(currentView.transform.position);
        turnSystem.EndTurn();

        // var target = enemyCharacters.FirstOrDefault(e => !e.IsDead);
        // item.Apply(target);
    }

    private CharacterView GetCharacterView(Character character)
    {
        return characterViews.Find(view => view.name == character.Name);
    }

    void ShowPlayerActionMenu()
    {
        if (current == null || !current.IsPlayerControlled)
            return;

        var availableActions = new List<IAction>();

        if (current.Actions != null)
        {
            availableActions.AddRange(current.Actions);
        }
        OptionsMenu.Instance.ShowOptions(availableActions);
        OptionsMenu.Instance.ChangeFocus(GetCharacterView(current).transform);
    }

    public Character GetCurrentCharacter()
    {
        return current;
    }
}

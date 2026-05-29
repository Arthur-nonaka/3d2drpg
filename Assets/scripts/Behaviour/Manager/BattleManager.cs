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

        foreach (var c in playerStats)
        {
            Character character;
            if (GameStateManager.Instance.HasSaveData())
            {
                var savedChar = GameStateManager.Instance.CurrentData.Characters.FirstOrDefault(s =>
                    s.name == c.Name
                );
                if (savedChar != null)
                {
                    character = CharacterFactory.CreateFromSaveData(savedChar);
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
            playerHealthUI.Initialize(characters[characters.Count - 1]);
        }

        enemyCharacters = new List<Character>
        {
            new Character(
                name: "WOLF",
                isPlayerControlled: false,
                maxHP: 100,
                hp: 100,
                energy: 10,
                speed: 14,
                defense: 6,
                specialDefense: 4,
                attack: 4,
                specialAttack: 8,
                level: 1,
                experience: 50
            ),
        };

        SpecialBarManager.Instance.ResetBar();

        var allCharacters = characters.Concat(enemyCharacters).ToList();
        turnSystem = new TurnSystem(allCharacters);

        turnSystem.OnTurnStarted += HandleTurnStart;
        turnSystem.OnTurnEnded += HandleTurnEnd;
        turnSystem.StartTurn();
    }

    void HandleTurnStart(Character character)
    {
        current = character;
        Debug.Log($"{character.Name} started their turn.");

        if (current.IsDead)
        {
            Debug.Log($"{current.Name} is dead.");
            turnSystem.EndTurn();
            BattleEnded();
            return;
        }
        // Process status effects that trigger at the start of the turn
        current.ProcessStartOfTurnEffects();

        if (current.IsDead)
        {
            Debug.Log($"{current.Name} died from a status effect.");
            turnSystem.EndTurn();
            BattleEnded();
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

    void ExecuteEnemyTurn()
    {
        if (current == null)
            return;

        var alivePlayers = characters.Where(c => c.IsPlayerControlled && !c.IsDead).ToList();

        if (alivePlayers.Count == 0)
        {
            Debug.Log("All players are dead. Game Over.");
            return;
        }

        var aliveEnemies = enemyCharacters.Where(e => !e.IsDead).ToList();
        if (aliveEnemies.Count == 0)
        {
            BattleEnded();
            return;
        }

        var target = alivePlayers[Random.Range(0, alivePlayers.Count)];
        int damage = current.Attack;

        var currentView = GetCharacterView(current);
        var targetView = GetCharacterView(target);

        Vector3 cameraDirection = (
            Camera.main.transform.position - targetView.transform.position
        ).normalized;
        Vector3 attackPos = targetView.transform.position + cameraDirection * 1f;

        currentView.PerformAttackMove(
            currentView.transform.position,
            attackPosition.transform.position,
            onAttack: () =>
            {
                target.TakeDamage(damage);
                if (currentView != null)
                {
                    VisualEffectManager.Instance.ShowFloatingText(
                        damage.ToString(),
                        attackPos,
                        Color.red
                    );
                    VisualEffectManager.Instance.PlayAnimation(currentView.Animator, "Attack");
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
    }

    public void PlayerChoseAction()
    {
        turnSystem.EndTurn();
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

        Vector3 cameraDirection = (
            Camera.main.transform.position - targetView.transform.position
        ).normalized;
        Vector3 attackPos = targetView.transform.position + cameraDirection * 1f;

        currentView.PerformAttackMove(
            currentView.transform.position,
            attackPosition.transform.position,
            onAttack: () =>
            {
                target.TakeDamage(damage);
                if (currentView != null)
                {
                    VisualEffectManager.Instance.ShowFloatingText(
                        damage.ToString(),
                        attackPos,
                        Color.red
                    );
                    VisualEffectManager.Instance.PlayAnimation(currentView.Animator, "Attack");
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

    public void PlayerChoseItem(ItemAction item)
    {
        if (current == null || !current.IsPlayerControlled)
            return;

        if (item.targetType == ItemTargetType.Self)
        {
            item.Apply(current);
            turnSystem.EndTurn();
            return;
        }

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class TurnSystem
{
    private Queue<Character> turnOrder = new Queue<Character>();
    public event Action<Character> OnTurnStarted;
    public event Action<Character[]> OnTurnOrderUpdated;
    public event Action<Character> OnTurnEnded;

    public TurnSystem(List<Character> characters)
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var c in characters)
            {
                turnOrder.Enqueue(c);
            }
        }
    }

    public Character GetCurrentTurn() => turnOrder.Peek();

    public void EndTurn()
    {
        var c = turnOrder.Dequeue();
        OnTurnEnded?.Invoke(c);
        turnOrder.Enqueue(c);
        StartTurn();
    }

    public Character[] GetTurnOrder() => turnOrder.ToArray().Take(5).ToArray();

    public void StartTurn()
    {
        var current = GetCurrentTurn();
        OnTurnStarted?.Invoke(current);
        OnTurnOrderUpdated?.Invoke(GetTurnOrder());
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TurnSystem
{
    private Queue<Character> turnOrder = new Queue<Character>();
    public event Action<Character> OnTurnStarted;
    public event Action<Character> OnTurnEnded;

    public TurnSystem(List<Character> characters)
    {
        foreach (var c in characters)
        {
            turnOrder.Enqueue(c);
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

    public void StartTurn()
    {
        var current = GetCurrentTurn();
        OnTurnStarted?.Invoke(current);
    }
}

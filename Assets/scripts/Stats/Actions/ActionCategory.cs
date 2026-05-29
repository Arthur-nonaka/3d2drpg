using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Options/Action Category", fileName = "NewActionCategory")]
public class ActionCategory : ScriptableObject, IAction
{
    [SerializeField]
    private string categoryName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private List<ScriptableObject> actionObjects;
    public List<IAction> Actions
    {
        get
        {
            var list = new List<IAction>();
            foreach (var obj in actionObjects)
            {
                if (obj is IAction action)
                    list.Add(action);
            }
            return list;
        }
    }

    public string Name => categoryName;
    public Sprite Icon => icon;
    public int RequiredLevel => 1;

    public bool CanUse()
    {
        return true;
    }

    public List<IAction> GetActions() => Actions;

    public void Execute() { }
}

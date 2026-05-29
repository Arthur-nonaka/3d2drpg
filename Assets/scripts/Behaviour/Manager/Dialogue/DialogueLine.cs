using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string name;
    [TextArea(2, 5)] public string text;
}

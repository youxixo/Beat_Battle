using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    private Girl_Data CurrentCharacterData;
    public Girl_Data GetCurrentCharacterData => CurrentCharacterData;

    public void SetCurrentCharacterData(Girl_Data data)
    {
        CurrentCharacterData = data;
    }
}

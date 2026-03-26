using UnityEngine;

public class TestEnemyController : MonoBehaviour
{
    private Girl_Data girlData => CharacterManager.Instance.GetCurrentCharacterData;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            bool isSpecialAttack = girlData.GetCurrentLandAttackType == LandAttackType.SpecialAttack_Part1 || girlData.GetCurrentLandAttackType == LandAttackType.SpecialAttack_Part2;
            if(isSpecialAttack)
            {
                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;

public class TestEnemyController : MonoBehaviour
{
    private Girl_Data girlData => CharacterManager.Instance.GetCurrentCharacterData;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitBox"))
        {
            bool isAttack3 = girlData.GetCurrentLandAttackType == LandAttackType.LandAttack3_Attack;
            if(isAttack3)
            {
                Destroy(gameObject);
            }
        }
    }
}

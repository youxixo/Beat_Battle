using UnityEngine;

public class ZombieDate : MonoBehaviour
{
    [Header("基础数据")]
    [SerializeField] private int currentHP = 10;
    public int CurrentHP
    {
        set
        {
            currentHP = value;
            if (currentHP <= 0)
            {
                currentHP = 0;
                isDead = true;
            }
        }
        get => currentHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
    }

    [SerializeField] private bool isDead = false;
    public bool IsDead => isDead;

    [Header("移动速度")]
    [SerializeField] private bool canMove = false;
    public bool CanMove => canMove;
    public void SetCanMove(bool CanMove)
    {
        canMove = CanMove;
    }

    [SerializeField, ChineseLabel("初始移动速度")] private float initialMoveSpeed = 2f;

    /// <summary>
    /// 初始移动速度
    /// </summary>
    public float InitialMoveSpeed => initialMoveSpeed;

    [SerializeField, ChineseLabel("当前移动速度")] private float currentMoveSpeed = 2f;

    /// <summary>
    /// 当前移动速度
    /// </summary>
    public float CurrentMoveSpeed => currentMoveSpeed;

    /// <summary>
    /// 设置当前移动速度
    /// </summary>
    public void SetCurrentMoveSpeed(float MoveSpeed)
    {
        currentMoveSpeed = MoveSpeed;
    }

    [Header("攻击冷却")]
    [SerializeField] private float attackCooldown = 2f;
    public float AttackCooldown => attackCooldown;

    [Header("攻击范围")]
    [SerializeField] private SpherePainter attackRange;
    public float AttackRange => attackRange.GetRadius;
}

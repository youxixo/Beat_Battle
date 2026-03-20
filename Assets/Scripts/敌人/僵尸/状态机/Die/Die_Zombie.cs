using System.Collections;
using UnityEngine;

public class Die_Zombie : BaseState<ZombieType>
{
    private Animator animator;
    private int DeathHash;
    private ZombieDate zombieDate;

    private EnemyManager enemyManager => EnemyManager.Instance;
    private CoroutineManager coroutineManager => CoroutineManager.Instance;
    public Die_Zombie(Animator animator, string deathAnimName, ZombieDate zombieDate) : base(needsExitTime: false)
    {
        this.animator = animator;
        DeathHash = Animator.StringToHash(deathAnimName);
        this.zombieDate = zombieDate;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    
        enemyManager.UnregisterEnemy(zombieDate);
        animator.Play(DeathHash);
        coroutineManager.Run("Zombie_Death" + animator.gameObject.GetInstanceID(), Zombie_Death());
    }

    public override void OnExit()
    {
        base.OnExit();

        coroutineManager?.Stop("Zombie_Death" + animator.gameObject.GetInstanceID());
    }   

    private IEnumerator Zombie_Death()
    {
        yield return null; // 等待一帧，确保动画状态已切换
        float animationLength = AnimatorTool.GetRealAnimationLength(animator, DeathHash);
        // 等待死亡动画播放完成
        yield return new WaitForSeconds(animationLength);
        
        // 销毁僵尸对象
        zombieDate.gameObject.SetActive(false);
    }
}

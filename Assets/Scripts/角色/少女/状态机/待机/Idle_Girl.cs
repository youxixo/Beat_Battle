using UnityEngine;

public class Idle_Girl : CharacterState<GirlStateType>
{
    private Animator animator;
    private int IdleHash;
    public Idle_Girl(Animator animator, int idleHash): base()
    {
        this.animator = animator;
        this.IdleHash = idleHash;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        cameraManager.SwitchCamera(CameraType.DefaultCamera);
        
        animator.Play(IdleHash);
    }
}

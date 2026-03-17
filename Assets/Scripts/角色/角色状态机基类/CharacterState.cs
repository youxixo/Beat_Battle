using System;
using UnityHFSM;

public class CharacterState<TStateId> : BaseState<TStateId>
{
    protected InputManager inputManager => InputManager.Instance;
    protected CoroutineManager coroutineManager => CoroutineManager.Instance;
    protected EnemyManager enemyManager => EnemyManager.Instance;
    protected CameraManager cameraManager => CameraManager.Instance;
    public CharacterState(bool needsExitTime = false, bool isGhostState = false, Func<State<TStateId>, bool> canExit = null) : base(needsExitTime: needsExitTime, isGhostState: isGhostState, canExit: canExit)
    {
    }   
}

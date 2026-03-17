using UnityEngine;
using UnityHFSM;

public enum GroundState
{
    GroundInitialState,
    isGround,
    isNotGround,
}

public class GroundCheckBaseController : MonoBehaviour
{
    [Header("Ground Check")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundMask;

     private RaycastHit groundHit;
    [SerializeField] private CharacterController characterController;
    private StateMachine<GroundState> stateMachine = new();

    private void Awake()
    {
        characterController = GetComponentInParent<CharacterController>();
        buildStateMachine();
    }

    void OnEnable()
    {
        stateMachine.Init();
    }

    private void Update()
    {
        stateMachine.OnLogic();
    }

    private void OnDisable()
    {
        stateMachine.OnExit();
    }

    private void buildStateMachine()
    {
        stateMachine.AddState(GroundState.GroundInitialState, new GroundInitialState());
        stateMachine.AddState(GroundState.isGround, new isGround(this));
        stateMachine.AddState(GroundState.isNotGround, new isNotGround(this));

        stateMachine.AddTransition(GroundState.GroundInitialState, GroundState.isGround, t=>CheckIsGround());
        stateMachine.AddTransition(GroundState.GroundInitialState, GroundState.isNotGround, t=>!CheckIsGround());
        stateMachine.AddTwoWayTransition(GroundState.isGround, GroundState.isNotGround, t=>!CheckIsGround());

        stateMachine.SetStartState(GroundState.GroundInitialState);
    }

    private bool CheckIsGround()
    {
        Vector3 origin = characterController.bounds.center;

        bool hit = Physics.SphereCast(
            origin,
            characterController.radius * 0.9f,
            Vector3.down,
            out groundHit,
            groundCheckDistance,
            groundMask
        );

        if (!hit)
        {
            return false;
        }

        float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);

        if (slopeAngle <= characterController.slopeLimit)
        {
            return true;
        }

        return false;
    }

    public virtual void WhenIsGround()
    {
        Debug.Log("进入了接地状态");
    }

    public virtual void WhenIsNotGround()
    {
        Debug.Log("进入了非接地状态");
    }
    /// <summary>
    /// 对外接口，获取当前是否在地面
    /// </summary>
    [SerializeField]private bool CharacterIsGround = true;

    /// <summary>
    /// 获取当前是否在地面
    /// </summary>
    public bool GetIsGround => CharacterIsGround;

    public void SetIsGround(bool isGround)
    {
        CharacterIsGround = isGround;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (characterController == null)
        {
            characterController = GetComponentInParent<CharacterController>();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (characterController == null) return;

        Vector3 origin = characterController.bounds.center;
        float radius = characterController.radius * 0.9f;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(origin, radius);
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, radius);
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
    #endif
}

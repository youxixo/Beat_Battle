#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public enum BeatPointType
{
    Good,
    Bad,
    Hited
}

public class PointBeatController : MonoBehaviour
{
    [SerializeField] private BeatPointType BeatPointState;
    public BeatPointType GetBeatPointState => BeatPointState;

    public Transform HitedPointEffect;
    public Transform BadPointEffect;
    public FixPointController UnHitedPointEffect;


    public void FixBadPoint()
    {
        UnHitedPointEffect.transform.localScale = BadPointEffect.transform.localScale;
        UnHitedPointEffect.transform.position = BadPointEffect.position;
        ChangeState(BeatPointType.Good);
    }

    public void HitPoint()
    {
        ChangeState(BeatPointType.Hited);
    }

    public void ChangeState(BeatPointType newState)
    {
        BeatPointState = newState;
        switch (newState)
        {
            case BeatPointType.Good:
                HitedPointEffect.gameObject.SetActive(false);
                BadPointEffect.gameObject.SetActive(false);
                UnHitedPointEffect.gameObject.SetActive(true);
                UnHitedPointEffect.SetPointOnTheWall(true);
                UnHitedPointEffect.transform.localScale = HitedPointEffect.transform.localScale;
                UnHitedPointEffect.transform.position = HitedPointEffect.position;
                break;
            case BeatPointType.Bad:
                HitedPointEffect.gameObject.SetActive(false);
                BadPointEffect.gameObject.SetActive(true);
                UnHitedPointEffect.gameObject.SetActive(true);
                UnHitedPointEffect.SetPointOnTheWall(false);
                break;
            case BeatPointType.Hited:
                HitedPointEffect.gameObject.SetActive(true);
                BadPointEffect.gameObject.SetActive(false);
                UnHitedPointEffect.gameObject.SetActive(false);
                break;
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;

            ChangeState(BeatPointState);
        };
    }
    #endif

}

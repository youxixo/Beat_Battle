using System;
using UnityEngine;

public class GirlCheckGround : GroundCheckBaseController
{
    [SerializeField] private Girl_Data girlData;
    public override void WhenIsGround()
    {
        girlData.SetCurrentJumpCount(0);
        girlData.SetCurrentYVelocity(0f);
    }

    public override void WhenIsNotGround()
    {
        Debug.Log("进入了非接地状态");
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolKingFollow : StateMachineBehaviour
{
    private FoolKing fk;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fk = GameObject.FindGameObjectWithTag("FoolKing").GetComponent<FoolKing>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fk.FK_Follow();
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}

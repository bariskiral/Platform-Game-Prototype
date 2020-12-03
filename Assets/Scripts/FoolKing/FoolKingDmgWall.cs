using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolKingDmgWall : StateMachineBehaviour
{
    private FoolKing fk;
    private float _waveTime;
    [SerializeField] private float waveTime = 1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fk = GameObject.FindGameObjectWithTag("FoolKing").GetComponent<FoolKing>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_waveTime <= 0)
        {
            fk.FK_DamageWall();
            _waveTime = waveTime;
        }
        else
        {
            _waveTime -= Time.deltaTime;
        }
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}

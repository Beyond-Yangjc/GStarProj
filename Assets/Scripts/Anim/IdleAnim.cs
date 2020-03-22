using UnityEngine;

namespace GStar.Prepare
{
    public class IdleAnim : BaseAnim
    {
        public IdleAnim(Animator _animator) : base(_animator)
        {
        }
        
        public override AnimState animState => AnimState.Idle;
        public override bool loop => true;
        
        public override void OnEnter(AnimState _fromState)
        {
            SetTrigger(IdleTrigger);
        }

        public override void OnExit(AnimState _toState)
        {
        }

        
    }
}
using UnityEngine;

namespace GStar.Prepare
{
    public class DieAnim : BaseAnim
    {
        public DieAnim(Animator _animator) : base(_animator)
        {
        }
        
        public override AnimState animState => AnimState.Die;
        public override bool loop => false;
        
        public override void OnEnter(AnimState _fromState)
        {
            SetTrigger(DieTrigger);
        }

        public override void OnExit(AnimState _toState)
        {
        }

        
    }
}
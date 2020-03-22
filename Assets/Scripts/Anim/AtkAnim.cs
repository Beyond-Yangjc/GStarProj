using UnityEngine;

namespace GStar.Prepare
{
    public class AtkAnim : BaseAnim
    {
        public AtkAnim(Animator _animator) : base(_animator)
        {
        }

        public override AnimState animState => AnimState.Atk;
        public override bool loop => false;

        public override void OnEnter(AnimState _fromState)
        {
            
            SetAnimTrigger(AtkTrigger);
        }

        public override void OnExit(AnimState _toState)
        {
        }

        
    }
}
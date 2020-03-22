using UnityEngine;

namespace GStar.Prepare
{
    public class MoveAnim : BaseAnim
    {  
        public MoveAnim(Animator _animator) : base(_animator)
        {
        }
        
        public override AnimState animState => AnimState.Move;
        public override bool loop => true;
        
        public override void OnEnter(AnimState _fromState)
        {
            SetTrigger(MoveTrigger);
        }

        public override void OnExit(AnimState _toState)
        { 
        }
    }
}
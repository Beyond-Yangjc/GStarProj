using UnityEngine;

namespace GStar.Prepare
{
    public abstract class BaseAnim
    {
        public const string IdleTrigger = "_idle";
        public const string AtkTrigger = "_atk";
        public const string MoveTrigger = "_move";
        public const string MoveOverTrigger = "_moveOver";
        public const string DieTrigger = "_die";

        public enum AnimState
        {
            Idle = 0,
            Atk,
            Move,
            Die,
        }

        protected Animator animator;

        public abstract AnimState animState { get; }

        public abstract bool loop { get; }

        public BaseAnim(Animator _animator)
        {
            animator = _animator;
        }

        public abstract void OnEnter(AnimState _fromState);

        public abstract void OnExit(AnimState _toState);

        protected virtual void SetAnimTrigger(string _trigger)
        {
            if (string.IsNullOrEmpty(_trigger) || animator == null) return;
            
            for (int i = 0; i < animator.parameterCount; i++)
            {
                animator.ResetTrigger(animator.GetParameter(i).name);
            }

            SetTrigger(_trigger);
        }

        protected virtual void SetTrigger(string _trigger)
        {
            if (animator == null) return;
            animator.SetTrigger(_trigger);
        }
    }
}
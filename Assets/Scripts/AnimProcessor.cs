using System;
using System.Collections.Generic;
using UnityEngine;

namespace GStar.Prepare
{
    public struct AnimEvent
    {
        public BaseAnim.AnimState animState;
        public int eventCode;
        public float duration;

        public AnimEvent(BaseAnim.AnimState _animState, int _eventCode, float _duration = -1)
        {
            animState = _animState;
            eventCode = _eventCode;
            duration = _duration;
        }
    }

    [RequireComponent(typeof(Animator))]
    public class AnimProcessor : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private Dictionary<BaseAnim.AnimState, BaseAnim> animDic;
        private BaseAnim lastAnim, curAnim;
        private int curEventCode;

        private BaseAnim.AnimState curState;
        private int curCode;
        private float curDuration;
        public int CurEventCode => curEventCode;
        public BaseAnim.AnimState CurAnimState => curState;

        public delegate void OneAnimEndHandler(int _eventCode);

        public OneAnimEndHandler onOneAnimEnd;

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            animDic = new Dictionary<BaseAnim.AnimState, BaseAnim>
            {
                [BaseAnim.AnimState.Idle] = new IdleAnim(animator),
                [BaseAnim.AnimState.Atk] = new AtkAnim(animator),
                [BaseAnim.AnimState.Move] = new MoveAnim(animator),
                [BaseAnim.AnimState.Die] = new DieAnim(animator),
            };
        }

        private void Start()
        {
            lastAnim = curAnim = animDic[BaseAnim.AnimState.Idle];
        }


        public void ChangeState(AnimEvent _event)
        {
            if (curAnim.animState == _event.animState) return;
            curEventCode = _event.eventCode;
            curState = _event.animState;
            curDuration = _event.duration;
        }

        private void Update()
        {
            if (curAnim.animState == BaseAnim.AnimState.Die) return;

            ProcessAnimChange();

            ProcessAnimEnd();
        }

        void ProcessAnimChange()
        {
            if (curAnim.animState != curState)
            {
                if (!animDic.TryGetValue(curState, out var _anim)) _anim = animDic[BaseAnim.AnimState.Idle];
                var _lastState = lastAnim.animState;
                lastAnim.OnExit(curState);
                lastAnim = curAnim;
                curAnim = _anim;
                curAnim.OnEnter(_lastState);
            }
        }

        void ProcessAnimEnd()
        {
            if (curAnim.loop) return;
            if (animator == null) return;
            curDuration -= Time.deltaTime;
            if (curDuration <= 0)
            {
                Debug.Log("Anim结束");
                onOneAnimEnd?.Invoke(curEventCode);
                ChangeState(new AnimEvent(BaseAnim.AnimState.Idle, Utility.GetEventCode()));
            }
        }
    }
}
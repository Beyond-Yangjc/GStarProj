using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GStar.Prepare
{
    public class EnemyController : BaseController
    {
        public override E_Type eType => E_Type.Enemy;

        private Vector3 home;
        private float actRng;
        private System.Random rand;

        private float DotLockAngle = 0.707f;

        public void Initial(Vector3 _source, float _rng)
        {
            home = _source;
            actRng = _rng;
            rand = new System.Random(markID);
            DotLockAngle = Mathf.Cos(curProperty.LockAngle * Mathf.Deg2Rad);
            if (curTarget == null)
                NoTarget_Action();
        }

        void NoTarget_Action()
        {
            if (isAlive == false) return;
            if (curTarget != null)
            {
                eStatue = E_Statue.Atk;
                return;
            }

            using (var _rc = new RandomController<int>(new[] {1, 2}, new ushort[] {2, 1}, 1))
            {
//                int[]rands=_rc.RandomExtract(rand);//普通随机
                int[] rands = _rc.WeightRandomExtract(rand); //加权随机 
                if (rands.Length > 0)
                {
                    switch ((E_Statue) rands[0] - 1)
                    {
                        case E_Statue.Idle:
                            Idle();
                            break;
                        case E_Statue.MoveAround:
                            MoveAround();
                            break;
                        default: return;
                    }
                }
            }
        }


        async void Idle()
        {
            if (isAlive == false) return;
            var _idleTime = UnityEngine.Random.Range(1.0f, 5.0f);

            await Task.Delay(TimeSpan.FromSeconds(_idleTime));

            NoTarget_Action();
        }

        void MoveAround(bool _enterPos = false, Vector3 _nextPos = default)
        {
            if (isAlive == false) return;
            if (actRng > 0 && _enterPos == false)
            {
                var _randomPos = UnityEngine.Random.insideUnitCircle * actRng;
                _nextPos = home + new Vector3(_randomPos.x, transform.position.y, _randomPos.y);
            }

            var _moveCmd = new MoveCommand(agent, animProcessor, _nextPos, 0.01f);
            _moveCmd.AddEndEvent(NoTarget_Action);
            cQueue.Enqueue(_moveCmd);
        }

        protected override void Update()
        {
            base.Update();
            if (this.isAlive == false) return;
            if (curTarget != null) return;
            var _player = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseController>();
            if (_player.isAlive == false) return;
            var _dist = (transform.position - _player.transform.position).magnitude;
            if (_dist < curProperty.LockDist &&
                Vector3.Dot(transform.forward, (_player.transform.position - transform.position)) > DotLockAngle)
            {
                //看到你啦~~~~    
                if (curTarget == null || curTarget != null && curTarget != _player) //&& animProcessor.curAnimState != BaseAnim.AnimState.Atk
                {
                    curTarget = _player;
                    ClearCmds();
                }
                else if (animProcessor.CurAnimState == BaseAnim.AnimState.Atk)
                    return;

                //移动并靠近后攻击  

                BaseCommand.CommandEndHandler _handler = null;
                _handler = () =>
                {
                    if (this.isAlive == false) return;
                    var _peek = cQueue.Peek();
                    var _b1 = _peek.cType == BaseCommand.E_Command.Atk && _peek.eStatus == BaseCommand.E_Status.Over;
                    if (cQueue.Count <= 1 && _b1)
                    {
                        if (curTarget.isAlive)
                            MoveAndAtk(_handler, curTarget);
                        else
                        {
                            curTarget = null;
                            MoveAround(true, this.home);
                        }
                    }
                };

                MoveAndAtk(_handler, curTarget);
            }
        }

        /// <summary>
        /// 接近目标并攻击
        /// </summary>
        /// <param name="_handler"></param>
        private void MoveAndAtk(BaseCommand.CommandEndHandler _handler, BaseController _target)
        {
            if ((_target.transform.position - transform.position).magnitude - 1f > curProperty.AtkRng)
            {
                //当不在攻击射程内时，先移动
                cQueue.Enqueue(new MoveCommand(agent, animProcessor, _target, curProperty.AtkRng));
            }
            else
            {
                var _dot = Vector3.Dot(_target.transform.position, transform.position);
                if (_dot > 0.2f || _dot < -0.2f)
                {
                    var _rotCmd = new RotCommand(agent, animProcessor, _target, 1f);
                    cQueue.Enqueue(_rotCmd);
                }
            }

            var _atkCmd = new AtkCommand(animProcessor, curProperty, _target);
            _atkCmd.AddEndEvent(_handler);
            cQueue.Enqueue(_atkCmd);
        }
    }
}
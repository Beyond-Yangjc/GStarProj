using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GStar.Prepare
{
    public class PlayerController : BaseController
    {
        private Camera cam;

        public override E_Type eType => E_Type.Player;

        protected override void Awake()
        {
            base.Awake();
            if (cam == null) cam = Camera.main;
        }

        protected override void Start()
        {
            base.Start();
            InputManager.OnMouse0 += OnMouse_0;
            InputManager.OnMouse1 += OnMouse_1;
            InputManager.onKeyS += OnKey_S;
        }


        void OnMouse_0(InputManager.E_EventType _event, Vector3 _pos)
        {
//            if (_event == InputManager.E_EventType.Up)
        }

        void OnMouse_1(InputManager.E_EventType _event, Vector3 _pos)
        {
            if (this.isAlive == false) return;
            if (_event == InputManager.E_EventType.Up)
            {
                RaycastHit _hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(_pos), out _hit)) //layerMask:1 << LayerMask.NameToLayer("Ground")
                {
                    var _go = _hit.collider?.gameObject;
                    if (_go != null)
                        switch (_go.tag)
                        {
                            case "Ground":
                                if (animProcessor.CurAnimState != BaseAnim.AnimState.Atk)
                                {
                                    ClearCmds();
                                }

                                //向目标点转向并移动  
//                                if (cQueue.Count > 0 && cQueue.Peek().cType == BaseCommand.E_Command.Move)
//                                    cQueue.Dequeue();
                                cQueue.Enqueue(new MoveCommand(agent, animProcessor, _hit.point, 0.1f));
                                curTarget = null;
                                break;
                            case "Enemy":
                                var _target = _go.GetComponent<BaseController>();
                                if (_target == null || _target.eType != E_Type.Enemy) break;
                                if (_target.isAlive == false) break;

                                if (curTarget == null || curTarget != null && curTarget != _target) //&& animProcessor.curAnimState != BaseAnim.AnimState.Atk
                                {
                                    curTarget = _target;
                                    ClearCmds();
                                }
                                else if (animProcessor.CurAnimState == BaseAnim.AnimState.Atk)
                                    return;

                                //移动并靠近后攻击  

                                BaseCommand.CommandEndHandler _handler = null;
                                _handler = () =>
                                {
                                    var _peek = cQueue.Peek();
                                    var _b1 = _peek.cType == BaseCommand.E_Command.Atk && _peek.eStatus == BaseCommand.E_Status.Over;
                                    if (cQueue.Count <= 1 && _b1 && curTarget.isAlive)
                                    {
                                        MoveAndAtk(_handler, curTarget);
                                    }
                                };

                                MoveAndAtk(_handler, curTarget);
                                break;
                        }
                }
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

        void OnKey_S(InputManager.E_EventType _event)
        {
            if (this.isAlive == false) return;
            if (_event == InputManager.E_EventType.Up)
            {
                ClearCmds();
                cQueue.Enqueue(new StopCommand(animProcessor, agent));
            }
        }

        public override void GetHit(int _hitValue)
        {
            base.GetHit(_hitValue);
            Debug.Log($"玩家受到伤害： {_hitValue}");
        }

        private Vector2 _vec;

        void OnGUI()
        { 
            if (agent != null)
                GUILayout.Label($"RemainingDist: {agent.remainingDistance}");
            _vec = GUILayout.BeginScrollView(_vec);
            if (cQueue != null)
                foreach (var _command in cQueue)
                {
                    GUILayout.Label($"type: {_command.cType} == status: {_command.eStatus.ToString()}");
                }

            GUILayout.EndScrollView();
        }
    }
}
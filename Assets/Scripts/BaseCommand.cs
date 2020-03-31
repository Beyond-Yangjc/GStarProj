using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GStar.Prepare
{
    public abstract class BaseCommand : IDisposable
    {
        public enum E_Command
        {
            eNull = 0,
            Move,
            Atk,
            Stop,
            Rot,
        }

        public enum E_Status
        {
            Blocked = 0, //堵塞
            Running,
            Over,
        }

        public E_Status eStatus { get; protected set; }

        public abstract E_Command cType { get; }

        public delegate void CommandEndHandler();

        protected CommandEndHandler OnCommandEnd;

        protected bool disposed = false;

        public BaseCommand()
        {
            eStatus = E_Status.Blocked;
        }

        public virtual void Execute()
        {
            eStatus = E_Status.Running;
        }

        public virtual void Tick(float _deltaTime)
        {
        }


        public virtual void AddEndEvent(CommandEndHandler _handler = null)
        {
            if (_handler != null)
                this.OnCommandEnd += _handler;
        }

        public virtual void Dispose(bool _interrupt)
        {
            if (!disposed)
            {
                disposed = !disposed;
                ((IDisposable) this).Dispose();
                if (_interrupt) return;
                OnCommandEnd?.Invoke();
                OnCommandEnd = null;
            }
        }

        void IDisposable.Dispose()
        {
            eStatus = E_Status.Over;
        }
    }

    public class MoveCommand : BaseCommand
    {
        public override E_Command cType => E_Command.Move;

        private NavMeshAgent agent;
        private AnimProcessor animProcessor;
        private BaseController target;
        private Vector3 pos;
        private float stopDist;

        public MoveCommand(NavMeshAgent _agent, AnimProcessor _animProcessor, Vector3 _tgtPos, float _stopDist)
        {
            agent = _agent;
            animProcessor = _animProcessor;
            pos = _tgtPos;
            stopDist = _stopDist;
        }

        public MoveCommand(NavMeshAgent _agent, AnimProcessor _animProcessor, BaseController _tgt, float _stopDist)
        {
            agent = _agent;
            animProcessor = _animProcessor;
            target = _tgt;
            stopDist = _stopDist;
        }

        public override void Execute()
        {
            base.Execute();
            Move(pos);
        }

        public override void Tick(float _deltaTime)
        {
            base.Tick(_deltaTime);
            if (animProcessor.CurAnimState == BaseAnim.AnimState.Move)
            {
                if (target != null)
                    agent.SetDestination(target.transform.position);
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
//                    if (!agent.hasPath || Math.Abs(agent.velocity.sqrMagnitude) < 0.01f)
//                    {
                    animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Idle, Utility.GetEventCode()));
                    Dispose(false);
//                    }
                }
            }
        }

        void Move(Vector3 _tgtPos)
        {
            if (agent == null || animProcessor == null) return;
            var _b = agent.hasPath;
            var _lastVec = agent.velocity;
            if (agent.isStopped)
                agent.isStopped = !agent.isStopped;
            agent.stoppingDistance = stopDist;
            agent.SetDestination(_tgtPos);
            if (_b && Vector3.Dot(_lastVec, agent.velocity) <= 0)
                agent.velocity = agent.velocity.normalized * agent.velocity.magnitude;
            animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Move, Utility.GetEventCode()));
        }
    }

    public class RotCommand : BaseCommand
    {
        private NavMeshAgent agent;
        private AnimProcessor animProcessor;
        private BaseController target;
        private Vector3 pos;
        private float finishTolerance;

        public override E_Command cType
        {
            get => E_Command.Rot;
        }

        public RotCommand(NavMeshAgent _agent, AnimProcessor _animProcessor, BaseController _target, float _finishTolerance)
        {
            agent = _agent;
            animProcessor = _animProcessor;
            target = _target;
            finishTolerance = _finishTolerance;
        }

        public RotCommand(NavMeshAgent _agent, AnimProcessor _animProcessor, Vector3 _tgtpos, float _finishTolerance)
        {
            agent = _agent;
            animProcessor = _animProcessor;
            pos = _tgtpos;
            finishTolerance = _finishTolerance;
        }

        public override void Execute()
        {
            base.Execute();
            if (target != null)
                pos = target.transform.position;
            animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Move, Utility.GetEventCode()));
        }

        private Quaternion _targetRotation, _lastRotation;

        public override void Tick(float _deltaTime)
        {
            base.Tick(_deltaTime);
            var targetDir = pos - agent.transform.position;
            var angle = Vector3.Angle(targetDir, agent.transform.forward);
            if (Mathf.Abs(angle) > finishTolerance)
            {
                if (targetDir != Vector3.zero && targetDir.sqrMagnitude > 0)
                {
                    _targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                }

                _lastRotation = Quaternion.Slerp(_lastRotation, _targetRotation, agent.angularSpeed * Time.fixedDeltaTime);
                agent.transform.rotation = _lastRotation;
            }
            else
            {
                Dispose(false);
            }
        }
    }

    public class Utility
    {
        public static int CODE_OFFSET = 10000;

        public static int GetEventCode()
        {
            ++CODE_OFFSET;
            if (CODE_OFFSET > 20000)
                CODE_OFFSET -= 10000;
            return CODE_OFFSET;
        }
    }

    public class AtkCommand : BaseCommand
    {
        private AnimProcessor animProcessor;
        private BaseController target;
        private CharacterProperty property;
        public override E_Command cType => E_Command.Atk;
        private int eventCode;

        public AtkCommand(AnimProcessor _animProcessor, CharacterProperty _property, BaseController _target)
        {
            animProcessor = _animProcessor;
            animProcessor.onOneAnimEnd += OnThisEnd;
            property = _property;
            target = _target;
        }

        public override void Execute()
        {
            base.Execute();
            eventCode = Utility.GetEventCode();
            animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Atk, eventCode, 0.833f)); 
        }

        void OnThisEnd(int _endCode)
        {
            if (_endCode == eventCode)
            {
                if (target.isAlive)
                    target.GetHit(property.Atk); 
                target = null;
                Dispose(false);
            }
        }

        public override void Dispose(bool _interrupt)
        {
            base.Dispose(_interrupt);
            animProcessor.onOneAnimEnd -= OnThisEnd;
        }
    }


    public class StopCommand : BaseCommand
    {
        public override E_Command cType => E_Command.Stop;
        private AnimProcessor animProcessor;

        public StopCommand(AnimProcessor _animProcessor, NavMeshAgent _agent)
        {
            animProcessor = _animProcessor;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        public override void Execute()
        {
            if (animProcessor != null && animProcessor.CurAnimState != BaseAnim.AnimState.Die)
                animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Idle, Utility.GetEventCode()));
            Dispose(false);
        }
    }
}
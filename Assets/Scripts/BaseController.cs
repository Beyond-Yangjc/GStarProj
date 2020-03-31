using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GStar.Prepare
{
    public abstract class BaseController : MonoBehaviour
    {
        public int MARKID_SOURCE = 10000;
        public int markID { get; private set; }

        public enum E_Type
        {
            eNull = 0,
            Player = 1,
            Enemy = 2,
            Ally = 3,
        }

        public enum E_Statue
        {
            Idle = 0,
            MoveAround,
            Atk,
            Die,
        }


        public virtual E_Statue eStatue { protected set; get; }
        public abstract E_Type eType { get; }

        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected AnimProcessor animProcessor;
        [SerializeField] protected CharacterPropertyObject baseProperty; 
        protected CharacterProperty curProperty;

        protected Queue<BaseCommand> cQueue;

        protected BaseController curTarget;

        public bool isAlive
        {
            get => (curProperty?.Hp ?? 0) > 0;
        }


        protected virtual void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (animProcessor == null) animProcessor = gameObject.AddComponent<AnimProcessor>();
            if (baseProperty != null)
            {
                if (curProperty == null)
                    curProperty = new CharacterProperty().CopyFrom(baseProperty.property);
            }

            cQueue = new Queue<BaseCommand>();
            markID = MARKID_SOURCE++;
        }

        protected virtual void Start()
        {
            agent.speed = curProperty.MoveSpeed;
            agent.angularSpeed = curProperty.RotSpeed;
            agent.acceleration = curProperty.Acceleration;
        }

        protected virtual void Update()
        {
            if (cQueue.Count > 0)
            {
                var _curCommand = cQueue.Peek();
                switch (_curCommand.eStatus)
                {
                    case BaseCommand.E_Status.Blocked:
                        _curCommand.Execute();
                        break;
                    case BaseCommand.E_Status.Running:
                        _curCommand.Tick(Time.deltaTime);
                        break;
                    case BaseCommand.E_Status.Over:
                        cQueue.Dequeue();
                        break;
                }
            }
        }

        protected void ClearCmds()
        {
            foreach (var _cmd in cQueue)
            {
                _cmd.Dispose(true);
            }
        }

        public virtual void GetHit(int _hitValue)
        {
            Debug.Log($"{transform.name} 受到伤害：{_hitValue}, 剩余：{curProperty.Hp - _hitValue}");
            curProperty.Hp -= _hitValue;
            if (curProperty.Hp <= 0)
            {
                ClearCmds();
                cQueue.Enqueue(new StopCommand(animProcessor, agent));
                animProcessor.ChangeState(new AnimEvent(BaseAnim.AnimState.Die, Utility.GetEventCode()));
            }
        }

        private void OnApplicationQuit()
        {
            ClearCmds();
        }
    }
}
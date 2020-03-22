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


        void AI_Action()
        {
            if (isAlive == false) return;
            if (curTarget != null)
            {
                eStatue = E_Statue.Atk;
                return;
            }

            var _actType = UnityEngine.Random.Range(0, 2);
            switch ((E_Statue) _actType)
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

        async void Idle()
        {
            if (isAlive == false) return;
            var _idleTime = UnityEngine.Random.Range(1.0f, 5.0f);

            await Task.Delay(TimeSpan.FromSeconds(_idleTime));

            AI_Action();
        }

        void MoveAround()
        {
            if (isAlive == false) return;
            if (actRng > 0)
            {
                var _randomPos = UnityEngine.Random.insideUnitCircle * actRng;
                var _nextPos = home + new Vector3(_randomPos.x, transform.position.y, _randomPos.y);

                var _moveCmd = new MoveCommand(agent, animProcessor, _nextPos, 0.01f);
                _moveCmd.AddEndEvent(MoveAround);
                cQueue.Enqueue(_moveCmd);
            }
        }

        public void Initial(Vector3 _source, float _rng)
        {
            home = _source;
            actRng = _rng;
            AI_Action();
        }
    }
}
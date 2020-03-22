using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GStar.Prepare
{
    public class GameStartUp : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private List<MonsterSourceObject> sourceObjs = new List<MonsterSourceObject>();
        [SerializeField] private List<Transform> monsterPoints = new List<Transform>();

        [SerializeField] private Transform MainHero;

        private List<MonsterSource> sources;

        private void Awake()
        {
            if (startPoint == null) startPoint = GameObject.FindWithTag("StartPoint").transform;
        }

        void Start()
        {
            if (MainHero == null)
                MainHero = ((GameObject) Object.Instantiate(Resources.Load("Prefabs/Characters/Knight"), startPoint.position, Quaternion.identity)).transform;

            sources = new List<MonsterSource>(monsterPoints.Count);
            for (var i = 0; i < monsterPoints.Count; i++)
            {
                if (sourceObjs.Count <= i) break;
                var _s = new MonsterSource().CopyFrom(sourceObjs[i].monsterSource);
                _s.pos = monsterPoints[i].position;
                sources.Add(_s);
            }

            for (var i = 0; i < sources.Count; i++)
            {
                if (sourceObjs.Count <= i) break;
                for (int j = 0; j < sources[i].initCnt; j++)
                {
                    var _monster = (GameObject.Instantiate(sourceObjs[i].prefab, sources[i].pos, Quaternion.identity)).GetComponent<BaseController>();
                    if (_monster is EnemyController _enemyController)
                        _enemyController.Initial(sources[i].pos, sources[i].ActRng);
                    sources[i].enemyCtrls.Add(_monster);
                }
            } 
        }
    }
}
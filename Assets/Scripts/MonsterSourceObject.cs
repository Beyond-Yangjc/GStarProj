using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GStar.Prepare
{
    [System.Serializable]
    public class MonsterSource
    {
        /// <summary>
        /// 刷新间隔时间
        /// </summary>
        public float refreshInternal;

        /// <summary>
        /// 可刷新数量
        /// </summary>
        public int refreshCnt;

        /// <summary>
        /// 初始数量
        /// </summary>
        public int initCnt;

        /// <summary>
        /// 最大同时存活数量
        /// </summary>
        public int maxCnt;

        /// <summary>
        /// 当前存活数量
        /// </summary>
        public int curCnt;

        /// <summary>
        /// 活动半径
        /// </summary>
        public float ActRng;

        [HideInInspector]
        public Vector3 pos;
        
        public List<BaseController> enemyCtrls = new List<BaseController>();

    }
    
    public static class MonsterSourceExtension
    {
        public static MonsterSource CopyFrom(this MonsterSource _property, MonsterSource _fromProperty)
        {
            _property.refreshInternal = _fromProperty.refreshInternal;
            _property.refreshCnt = _fromProperty.refreshCnt;
            _property.initCnt = _fromProperty.initCnt;
            _property.refreshCnt = _fromProperty.refreshCnt;
            _property.curCnt = _fromProperty.curCnt;
            _property.maxCnt = _fromProperty.maxCnt;
            _property.pos = _fromProperty.pos;
            _property.ActRng = _fromProperty.ActRng;
            return _property;
        }
    }
    
    [CreateAssetMenu(menuName = "Assets/CreateMonsterSource", fileName = "MonsterSource")]
    public class MonsterSourceObject : ScriptableObject
    {
        public Transform prefab;
        public MonsterSource monsterSource;
    }


}


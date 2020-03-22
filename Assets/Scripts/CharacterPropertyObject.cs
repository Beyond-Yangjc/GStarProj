using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GStar.Prepare
{
    [System.Serializable]
    public class CharacterProperty
    {
        /// <summary>
        /// 血量
        /// </summary>
        public int Hp;

        /// <summary>
        /// 能量
        /// </summary>
        public int Ep;

        /// <summary>
        /// 攻击力
        /// </summary>
        public int Atk;

        /// <summary>
        /// 防御力
        /// </summary>
        public int Def;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float AtkRng;

        /// <summary>
        /// 移动速度
        /// </summary>
        [Range(0, 100)] public float MoveSpeed; 

        /// <summary>
        /// 加速度
        /// </summary>
        [Range(0, 100)] public float Acceleration;

        /// <summary>
        /// 转身速度
        /// </summary>
        [Range(0, 1800)] public float RotSpeed;
    }

    public static class CharacterPropertyExtension
    {
        public static CharacterProperty CopyFrom(this CharacterProperty _property, CharacterProperty _fromProperty)
        {
            _property.Hp = _fromProperty.Hp;
            _property.Ep = _fromProperty.Ep;
            _property.Def = _fromProperty.Def;
            _property.Atk = _fromProperty.Atk;
            _property.AtkRng = _fromProperty.AtkRng;
            _property.MoveSpeed = _fromProperty.MoveSpeed;
            _property.RotSpeed = _fromProperty.RotSpeed;
            _property.Acceleration = _fromProperty.Acceleration;
            return _property;
        }
    }

    [CreateAssetMenu(menuName = "Assets/CreateCharacterProperty", fileName = "CharacterProperty")]
    public class CharacterPropertyObject : ScriptableObject
    {
        [SerializeField] public CharacterProperty property;
    }
}
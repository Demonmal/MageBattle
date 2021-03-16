using MageBattle.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Spells
{
    public class MeteorsPool : SingletonObject<MeteorsPool>
    {
        [SerializeField] private GameObject _prefab;
        private Stack<Meteor> _stack = new Stack<Meteor>();
        private Vector3 _idlePosition = new Vector3(1000, 1000, 1000);
        private int _defaultCount = 5;

        private void Awake()
        {
            _instance = this;
            CreateObjects(_defaultCount);
        }

        private void CreateObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var meteor = Instantiate(_prefab, transform).GetComponent<Meteor>();
                meteor.Initialize(this);
                Push(meteor);
            }
        }

        public void Push(Meteor meteor)
        {
            meteor.transform.position = _idlePosition;
            _stack.Push(meteor);
        }

        public Meteor Pop()
        {
            if (_stack.Count == 0)
            {
                CreateObjects(1);
            }
            return _stack.Pop();
        }
    }
}
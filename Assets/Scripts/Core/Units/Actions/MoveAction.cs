using MageBattle.Core.Level;
using System.Collections.Generic;
using UnityEngine;

namespace MageBattle.Core.Units.Actions
{
    public class MoveAction : UnitsAction
    {
        private const float _speed = 2;
        private Queue<Tile> _pathQueue;
        private Vector3 _positionMoveTo;
        private float _moveStep;

        public MoveAction(Tile target, Unit unit) : base(target, unit)
        {
            _moveStep = _speed * Time.deltaTime;
        }

        public override void Start()
        {
            if (LevelBuilder.instance.pathHelper.TryGetPathFromTo(_unit.currentTile, target, out var path))
            {
                if (path.Count == 0)
                {
                    OnActionCompleted();
                }
                else
                {
                    _pathQueue = new Queue<Tile>(path);
                    SetNextPosition();
                }
            }
            else
            {
                OnActionCompleted();
            }
        }

        private void SetNextPosition()
        {
            Tile tile = _pathQueue.Dequeue();
            _positionMoveTo = new Vector3(tile.transform.position.x, _unit.transform.position.y, tile.transform.position.z);
        }

        protected override void OnActionCompleted()
        {
            _unit.SetCurrentTile(target);
            base.OnActionCompleted();
        }

        protected override void SetId()
        {
            id = -1;
        }

        public override void Update()
        {
            if (_completed)
                return;
            MoveToNextTile();
        }

        private void MoveToNextTile()
        {
            Vector3 direction = _positionMoveTo - _unit.transform.position;
            Vector3 directionNormalized = direction.normalized;
            float distance = direction.magnitude;
            if(distance <= _moveStep)
            {
                _unit.transform.position = _positionMoveTo;
                ArrivedToPosition();
            }
            else
            {
                _unit.transform.position += directionNormalized * _moveStep;
            }
        }

        private void ArrivedToPosition()
        {
            if(_pathQueue.Count == 0)
            {
                OnActionCompleted();
                return;
            }
            SetNextPosition();
        }
    }
}
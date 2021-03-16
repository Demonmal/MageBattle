using System;
using UnityEngine;

namespace MageBattle.Core.Units.Spells
{
    public class Meteor : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private float _moveStep;
        private Vector3 _to;
        private Vector3 _direction;
        private Vector3 _directionN;
        private MeshRenderer _meshRenderer;
        private MeteorsPool _pool;
        private bool _isActive;

        public bool isActive => _isActive;

        public Action<Meteor> onArrived;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            SetAlphaState(false);
        }

        public void Initialize(MeteorsPool pool)
        {
            _pool = pool;
        }

        public void MoveTo(Vector3 to)
        {
            _isActive = true;
            _to = to;
            _direction = to - transform.position;
            _directionN = _direction.normalized;
            _moveStep = _speed * Time.deltaTime;
            SetAlphaState(true);
        }

        private void Update()
        {
            if (!_isActive)
                return;
            Move();
        }

        private void Move()
        {
            _direction = _to - transform.position;
            float distance = _direction.magnitude;
            if (distance <= _moveStep)
            {
                transform.position = _to;
                ArrivedToPosition();
            }
            else
            {
                transform.position += _directionN * _moveStep;
            }
        }

        private void SetAlphaState(bool state)
        {
            Color color = _meshRenderer.material.color;
            color.a = state ? 1 : 0;
            _meshRenderer.material.color = color;
        }

        private void ArrivedToPosition()
        {
            onArrived?.Invoke(this);
            _isActive = false;
            SetAlphaState(false);
            _pool.Push(this);
        }
    }
}
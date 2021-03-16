using MageBattle.Core.Level;
using System;
using UnityEngine;
namespace MageBattle.Core.Units.Actions
{
    public abstract class UnitsAction
    {
        protected Unit _unit;
        protected bool _completed;

        public int id { get; protected set; }
        public Tile target { get; protected set; }

        public event Action<UnitsAction> onActionCompleted;

        public UnitsAction(Tile target, Unit unit)
        {
            this.target = target;
            _unit = unit;
            SetId();
        }

        public virtual void Start() { }

        protected abstract void SetId();

        protected virtual void OnActionCompleted()
        {
            _completed = true;
            onActionCompleted?.Invoke(this);
        }

        public abstract void Update();
    }
}
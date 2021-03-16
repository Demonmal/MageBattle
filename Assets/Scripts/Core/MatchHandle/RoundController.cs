using MageBattle.Core.Units;
using System;
using System.Collections.Generic;

namespace MageBattle.Core.MatchHandle
{
    public class RoundController
    {
        private GameHandler _gameHandler;
        private List<Fraction> _fractionStepsOrder = new List<Fraction>();
        private int _orderIndex;
        private int _stepsCount;

        public RoundController(GameHandler gameHandler)
        {
            _gameHandler = gameHandler;
            Initialize();
        }

        private void Initialize()
        {
            foreach (var fractionObj in Enum.GetValues(typeof(Fraction)))
            {
                _fractionStepsOrder.Add((Fraction)fractionObj);
            }
            _stepsCount = _fractionStepsOrder.Count;
        }

        public void StartNewRound()
        {
            _orderIndex = -1;
            NextStep();
        }

        public void NextStep()
        {
            _orderIndex++;
            if(_orderIndex >= _stepsCount)
            {
                RoundEnd();
                return;
            }
            Fraction fraction = _fractionStepsOrder[_orderIndex];
            UnityEngine.Debug.Log($"NextStep {_orderIndex}, fraction {fraction}");
            if (UnitsManager.instance.unitsByFraction.TryGetValue(fraction, out var unit))
            {
                _gameHandler.OnNextRoundStep(unit);
            }
            else
            {
                RoundEnd();
            }            
        }

        private void RoundEnd()
        {
            var firstOrderFraction = _fractionStepsOrder[0];
            _fractionStepsOrder.RemoveAt(0);
            _fractionStepsOrder.Add(firstOrderFraction);
            _gameHandler.OnRoundEnd();
        }
    }
}
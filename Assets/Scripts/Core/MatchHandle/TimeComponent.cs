using System;
using System.Collections;
using UnityEngine;

namespace MageBattle.Core.MatchHandle
{
    public class TimeComponent : MonoBehaviour
    {
        private float _matchStartTime;
        private float _roundTime;

        public event Action onMatchStarted;

        public bool matchStarted { get; private set; }
        public float timeBeforeMatchStart => Time.timeSinceLevelLoad - _matchStartTime;

        public void Initialize(float timeBeforeMatchStart, float roundTime)
        {
            _roundTime = roundTime;
            _matchStartTime = Time.timeSinceLevelLoad + timeBeforeMatchStart;
            StartCoroutine(StartMatchCoroutine());
        }

        private IEnumerator StartMatchCoroutine()
        {
            while(!matchStarted)
            {
                if(Time.timeSinceLevelLoad >= _matchStartTime)
                {
                    StartMatch();
                }
                yield return null;
            }
        }

        private void StartMatch()
        {
            matchStarted = true;
            onMatchStarted?.Invoke();
        }
    }
}
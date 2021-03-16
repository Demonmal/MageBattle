using MageBattle.Core.Data;
using MageBattle.Core.Level;
using MageBattle.Core.Units;
using MageBattle.Core.Units.Bots;
using MageBattle.Profile;
using MageBattle.Utility;
using System;
using UnityEngine;

namespace MageBattle.Core.MatchHandle
{
    public class GameHandler : SingletonObject<GameHandler>
    {
        private const string _defaultMatchInfoDataPath = "Data/default_match_info";

        private TimeComponent _timeComponent;
        private MatchInfo _matchInfo;
        private Unit _unitToAction;
        private RoundController _roundController;
        private BotsDecisionMaker _botsDecisionMaker;
        private int _roundNumber = 1;
        private float _delayBetweenRounds = 1;
        private float _timeBeforeMatchStart = 3;
        private float _roundTime = 30;

        public MatchInfo matchInfo => _matchInfo;
        public TimeComponent timeComponent => _timeComponent;
        public BotsDecisionMaker botsDecisionMaker => _botsDecisionMaker;
        public int maxPlayersCount => 4;

        public event Action<Unit> onNextStep;
        public event Action onRoundEnd;

        private void Awake()
        {
            _instance = this;
            UserProfile.Initialize();
            InitializeDefaulMatch();
            LevelBuilder.instance.GenerateLevel();
            LoadDatas();
            CreateComponents();
            SubscribeEvents();
        }

        private void LoadDatas()
        {
            LayerMaskHelper.Load();
            SpellsInfoLoader.Load();
            DamageValuesContainer.Load();
        }

        private void CreateComponents()
        {
            _roundController = new RoundController(this);
            _timeComponent = gameObject.AddComponent<TimeComponent>();
            _timeComponent.Initialize(_timeBeforeMatchStart, _roundTime);
            _botsDecisionMaker = new BotsDecisionMaker();
        }

        private void SubscribeEvents()
        {
            _timeComponent.onMatchStarted += OnMatchStarted;
        }

        private void OnMatchStarted()
        {
            DebugUtility.Log(Color.yellow, "OnMatchStarted");
            StartNewRound();
        }

        private void InitializeDefaulMatch()
        {
            string jData = Resources.Load<TextAsset>(_defaultMatchInfoDataPath).text;
            _matchInfo = JsonSerializationHelper.DeserializeObject<MatchInfo>(jData);
        }

        public void OnRoundEnd()
        {
            _roundNumber++;
            onRoundEnd?.Invoke();
            DebugUtility.Log(Color.yellow, "OnRoundEnd");
            if (_roundNumber > _matchInfo.roundsCount)
            {
                GameEnd();
            }
            else
            {
                Invoke(nameof(StartNewRound), _delayBetweenRounds);
            }
        }

        private void StartNewRound()
        {
            DebugUtility.Log(Color.yellow, "StartNewRound");
            _roundController.StartNewRound();
        }

        public void OnNextRoundStep(Unit unit)
        {
            DebugUtility.Log(Color.yellow, $"OnNextRoundStep {unit.data.userId}");
            _unitToAction = unit;
            onNextStep?.Invoke(unit);
        }

        public void FinishStep()
        {
            _roundController.NextStep();
        }

        private void GameEnd()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                FinishStep();
            }
        }

        private void UnsubscribeEvents()
        {
            _timeComponent.onMatchStarted -= OnMatchStarted;
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
    }
}
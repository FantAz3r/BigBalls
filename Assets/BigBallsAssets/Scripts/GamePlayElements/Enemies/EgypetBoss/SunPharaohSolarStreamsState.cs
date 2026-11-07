using System.Collections.Generic;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohSolarStreamsState : BossStateBase
{
    private readonly SunPharaoh _boss;
    private readonly List<SolarBeam> _beams = new();
    private IObjectResolverProvider _resolverProvider;
    private IPlayerProvider _playerProvider;

    public SunPharaohSolarStreamsState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IObjectResolverProvider resolverProvider, IPlayerProvider playerProvider)
    {
        _resolverProvider = resolverProvider;
        _playerProvider = playerProvider;
    }

    protected override void OnEnter ()
    {
        var cfg = _boss.BossConfig;

        // ÷елимс€ вокруг игрока: 1/3 пол€ = 75 клеток, луч = 4 клетки (2x2) -> 19 лучей.
        // –азбрасываем лучи по полю 15x15, центриру€ на игроке (не выход€ за границы).
        Vector3 fieldOrigin = _boss.FieldOrigin.position;
        Vector3 fieldCenter = BattleGrid.GridCenter(fieldOrigin, cfg.GridWidth, cfg.GridHeight, cfg.CellSize);

        Vector3 playerPos = _playerProvider.Player.Transform.position;
        int px = Mathf.RoundToInt((playerPos.x - fieldOrigin.x) / cfg.CellSize);
        int py = Mathf.RoundToInt((playerPos.z - fieldOrigin.z) / cfg.CellSize);

        // —обираем все 2x2-квадраты, отбрасываем выход€щие за поле.
        var candidates = new List<Vector2Int>();
        for (int y = 0; y <= cfg.GridHeight - 2; y++)
            for (int x = 0; x <= cfg.GridWidth - 2; x++)
                candidates.Add(new Vector2Int(x, y));

        // —ортируем по близости к игроку и берЄм первые BeamCount Ч "удар по игроку" ощущаетс€.
        candidates.Sort((a, b) =>
        {
            float da = (a.x - px) * (a.x - px) + (a.y - py) * (a.y - py);
            float db = (b.x - px) * (b.x - px) + (b.y - py) * (b.y - py);
            return da.CompareTo(db);
        });

        int count = Mathf.Min(cfg.BeamCount, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            var c = candidates[i];
            Vector3 world = fieldOrigin + new Vector3((c.x + 1f) * cfg.CellSize, 0f, (c.y + 1f) * cfg.CellSize); // центр 2x2

            var go = Object.Instantiate(cfg.SolarBeamPrefab, world, Quaternion.identity);
            go.transform.localScale = new Vector3(2f * cfg.CellSize, 20f, 2f * cfg.CellSize); // 2x2 клетки, высота 20

            var beam = go.GetComponent<SolarBeam>();
            if (beam != null)
            {
                _resolverProvider.CurrentResolver.Inject(beam);
                beam.Init(
                    windup: cfg.SolarStreamsWindup,
                    active: cfg.BeamCastDuration,
                    damage: cfg.SunOrbBeamDamage, // или отдельное поле SolarBeamDamage
                    mask: cfg.SunOrbBeamMask);
                _beams.Add(beam);
            }
        }
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<SunPharaohIdleState>();
    }

    protected override void OnExit ()
    {
        for (int i = 0; i < _beams.Count; i++)
            if (_beams[i] != null)
                Object.Destroy(_beams[i].gameObject);
        _beams.Clear();
    }
}
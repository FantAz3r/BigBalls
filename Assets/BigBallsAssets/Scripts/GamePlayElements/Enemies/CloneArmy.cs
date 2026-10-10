using System.Collections;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;

public class ClonArmy
{
    private const float MinDistanceBetweenClones = 2f;
    private const float HealTime = 1;
    private const float JumpDelay = 1f;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IDamageService _damageService;
    private readonly IEnemyFactory _enemyFactory;

    private readonly List<StatuePharaoh> _allMembers = new List<StatuePharaoh>();
    private int _zoneWidth;
    private int _zoneDepth;
    private int _cloneCount;
    private Vector3 _originPoint;
    private float _moveSpeed;
    private WaitForSeconds _wait;

    public ClonArmy (ICoroutineRunner coroutineRunner, IDamageService damageService, IEnemyFactory enemyFactory)
    {
        _coroutineRunner = coroutineRunner;
        _damageService = damageService;
        _enemyFactory = enemyFactory;
    }

    public void Init (int zoneWidth, int zoneDepth, int cloneCount, Vector3 originPoint, float moveSpeed)
    {
        _wait = new WaitForSeconds(JumpDelay);
        _zoneWidth = zoneWidth;
        _zoneDepth = zoneDepth;
        _cloneCount = cloneCount;
        _originPoint = originPoint;
        _moveSpeed = moveSpeed;
    }

    public void UpdateOrigin (float deltaTime)
    {
        _originPoint += -Vector3.forward * _moveSpeed * deltaTime;
    }

    public void SpawnClones (StatuePharaoh original)
    {
        _originPoint = original.transform.position;
        AddMember(original);

        List<Vector3> targets = GetRandomPositions(_cloneCount);

        for (int i = 0; i < _cloneCount; i++)
        {
            StatuePharaoh cloneStatue = _enemyFactory.Create(original.Config, original.transform.position) as StatuePharaoh;
            cloneStatue.InitClone();
            AddMember(cloneStatue);
            cloneStatue.JumpAnimation(targets[i]);
        }

        original.JumpAnimation(GetPoint());
    }

    private void AddMember (StatuePharaoh statue)
    {
        statue.StatueDied += OnMemberDied;
        _allMembers.Add(statue);
    }

    private void RemoveMember (StatuePharaoh statue)
    {
        statue.StatueDied -= OnMemberDied;
        _allMembers.Remove(statue);
    }

    private void KillAll ()
    {
        var toDestroy = new List<StatuePharaoh>(_allMembers);

        foreach (var member in toDestroy)
        {
            if (member != null)
                member.EventHandler.Die(member);
        }

        _allMembers.Clear();
    }

    private void OnMemberDied (StatuePharaoh deadMember)
    {
        if (deadMember.IsOriginal)
        {
            KillAll();
        }
        else
        {
            HealAll(deadMember);
        }
    }

    private void HealAll (StatuePharaoh deadClone)
    {
        RemoveMember(deadClone);

        foreach (var member in _allMembers)
        {
            _damageService.ApplyHeal(member, member.Health.MaxValue - member.Health.CurrentValue, Color.green);
            member.EffectViewer.EnableEffect(EffectType.Heal, HealTime);
        }

        ShuffleRoutine();
    }

    private void ShuffleRoutine ()
    {
        if (_allMembers.Count == 0)
            return;

        _coroutineRunner.StartCoroutine(JumpToNewPositions());
    }

    private IEnumerator JumpToNewPositions ()
    {
        yield return _wait;

        foreach (var member in _allMembers)
        {
            member.JumpAnimation(_originPoint);
        }

        List<Vector3> newPositions = GetRandomPositions(_allMembers.Count);
        ShuffleList(newPositions);

        yield return _wait;

        for (int i = 0; i < _allMembers.Count && i < newPositions.Count; i++)
        {
            var member = _allMembers[i];
            if (member != null)
            {
                member.JumpAnimation(newPositions[i]);
            }
        }
    }

    private List<Vector3> GetRandomPositions (int count)
    {
        List<Vector3> positions = new List<Vector3>();
        HashSet<Vector3> uniquePoints = new HashSet<Vector3>();

        int maxAttempts = count * 10;
        int attempts = 0;

        while (uniquePoints.Count < count && attempts < maxAttempts)
        {
            attempts++;
            Vector3 newPoint = GetPoint();

            bool tooClose = false;

            foreach (var existingPoint in uniquePoints)
            {
                if (Vector2.Distance(new Vector2(newPoint.x, newPoint.z),
                                   new Vector2(existingPoint.x, existingPoint.z)) <= MinDistanceBetweenClones)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose == false)
            {
                uniquePoints.Add(newPoint);
            }
        }

        positions.AddRange(uniquePoints);
        return positions;
    }



    private Vector3 GetPoint()
    {
        int rx = (int) Random.Range(-_zoneWidth / 2f, _zoneWidth / 2f);
        int rz = (int) Random.Range(0, -_zoneDepth);
        return _originPoint + new Vector3(rx, 0, rz);
    }

    private void ShuffleList<T> (List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
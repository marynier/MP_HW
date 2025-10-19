using Colyseus.Schema;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    private struct States
    {
        public double Time;
        public Vector3 Position;
        public Vector3 Velocity;
    }
    [Header("Interpolation/Extrapolation")]
    [SerializeField] private float _interpolationDelay = 0.15f;   // 100�200 �� ������
    [SerializeField] private float _extrapolationLimit = 0.35f;   // 250�500 �� ������ �������������
    [SerializeField] private int _maxSnapshots = 20;

    [Header("Visual correction")]
    [SerializeField] private float _maxCorrectionSpeed = 10f;     // �/� ����������� ����������� ������
    [SerializeField] private float _snapDistance = 0.35f;         // ���������� ���� ��� ������� ������
    [SerializeField] private float _velocityDamping = 4f;         // ��������� �������� ��� ��������

    [Header("Timeouts")]
    [SerializeField] private double _staleTimeout = 0.3;          // ����� ������� ����� ����������


    private readonly List<States> _states = new List<States>(32);

    private Vector3 _visualPosition;          // ��, ��� ������� ����������
    private double _lastPacketLocalTime; // ��������� ����� ������� ���������� ������
    private Vector3 _lastServerPosition;

    private double _now => Time.realtimeSinceStartupAsDouble;

    private void Start()
    {
        _visualPosition = transform.position;
        _lastServerPosition = _visualPosition;
        _lastPacketLocalTime = _now;
    }

    internal void OnChange(List<DataChange> changes)
    {
        Vector3 serverPosition = GetServerPosition(changes);
        _lastServerPosition = serverPosition;
        _lastPacketLocalTime = _now;


        PushSnapshot(serverPosition, _lastPacketLocalTime);

        //������ � �����
        //Vector3 position = transform.position;
        //foreach (var dataChange in changes)
        //{
        //    switch (dataChange.Field)
        //    {
        //        case "x":
        //            position.x = (float)dataChange.Value;
        //            break;
        //        case "y":
        //            position.z = (float)dataChange.Value;
        //            break;
        //        default:
        //            Debug.LogWarning("�� �������������� ��������� ���� " + dataChange.Field);
        //            break;
        //    }
        //}
        //transform.position = position;
    }
    private Vector3 GetServerPosition(List<DataChange> changes)
    {
        Vector3 position = _lastServerPosition;
        foreach (var data�hange in changes)
        {
            switch (data�hange.Field)
            {
                case "x":
                    position.x = (float)data�hange.Value;
                    break;
                case "y":
                    position.z = (float)data�hange.Value;
                    break;
            }
        }
        return position;
    }

    private void PushSnapshot(Vector3 position, double time)
    {
        // ��������� �������� �� ����������� ��������� ��������
        Vector3 velocity = Vector3.zero;
        int n = _states.Count;
        if (n > 0)
        {
            var prev = _states[n - 1];
            double delta = time - prev.Time;
            if (delta > 0.0005)
                velocity = (position - prev.Position) / (float)delta;
            else
                velocity = prev.Velocity; // ������� ����� delta � ��������� ���������� ��������
        }

        _states.Add(new States { Time = time, Position = position, Velocity = velocity });

        // ��������
        while (_states.Count > _maxSnapshots)
            _states.RemoveAt(0);        
    }
    private bool TryGetInterpolatedTarget(double playbackTime, out Vector3 target)
    {
        target = default;
        int count = _states.Count;
        if (count == 0) return false;

        // ���� playback ������ ������ ������� � ���� ���������
        if (playbackTime <= _states[0].Time)
        {
            target = _states[0].Position;
            return true;
        }

        // ���� ��� �������� �������� ��� ������������
        for (int i = 0; i < count - 1; i++)
        {
            var a = _states[i];
            var b = _states[i + 1];
            if (playbackTime >= a.Time && playbackTime <= b.Time)
            {
                double span = b.Time - a.Time;
                float t = span > 0.0005 ? (float)((playbackTime - a.Time) / span) : 1f;
                target = Vector3.Lerp(a.Position, b.Position, Mathf.Clamp01(t));
                return true;
            }
        }

        // ���� playback ����� ���������� � ����������� �������������
        return false;
    }

    private Vector3 GetExtrapolatedTarget(double playbackTime)
    {
        // �������������� �� ���������� �������� � ������������
        var last = _states[_states.Count - 1];
        double delta = Mathf.Min((float)(playbackTime - last.Time), _extrapolationLimit);
        if (delta <= 0.0)
            return last.Position;

        // ��������� �������� ��� ���������� ���������� ��������
        float damp = Mathf.Exp(-_velocityDamping * (float)(playbackTime - last.Time));
        Vector3 v = last.Velocity * damp;
        return last.Position + v * (float)delta;
    }
    
    private void FixedUpdate()
    {
        if (_states.Count == 0)
        {
            // ��� ������ � ������� ��� ����
            transform.position = _visualPosition;
            return;
        }

        // �������� ������� �� interpolationDelay
        double playbackTime = _now - _interpolationDelay;

        Vector3 logicalTarget;
        bool hasInterp = TryGetInterpolatedTarget(playbackTime, out logicalTarget);
        if (!hasInterp)
        {
            // ��������� � ������������� � ������������
            logicalTarget = GetExtrapolatedTarget(playbackTime);
        }

        // ����������, �� �������� �� ������
        bool stale = (_now - _lastPacketLocalTime) > _staleTimeout;

        // ���������� ��������: ������������ �������� + ���� �����
        Vector3 toTarget = logicalTarget - _visualPosition;
        float dist = toTarget.magnitude;

        if (dist > _snapDistance)
        {
            // ������� ������� ������ � ��������� ����
            _visualPosition = logicalTarget;
        }
        else
        {
            // �������� � ������������ ���������; ��� stale �������� �������� ������
            float corrSpeed = stale ? Mathf.Max(2f, _maxCorrectionSpeed * 0.6f) : _maxCorrectionSpeed;
            Vector3 step = Vector3.ClampMagnitude(toTarget, corrSpeed * Time.fixedDeltaTime);
            _visualPosition += step;
        }

        transform.position = _visualPosition;
    }
}
 
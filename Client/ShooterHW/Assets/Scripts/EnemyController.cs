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
    [SerializeField] private float _interpolationDelay = 0.15f;   // 100–200 мс буфера
    [SerializeField] private float _extrapolationLimit = 0.35f;   // 250–500 мс предел экстраполяции
    [SerializeField] private int _maxSnapshots = 20;

    [Header("Visual correction")]
    [SerializeField] private float _maxCorrectionSpeed = 10f;     // м/с ограничение визуального догона
    [SerializeField] private float _snapDistance = 0.35f;         // мгновенный снап при большой ошибке
    [SerializeField] private float _velocityDamping = 4f;         // затухание скорости при таймауте

    [Header("Timeouts")]
    [SerializeField] private double _staleTimeout = 0.3;          // когда считаем поток устаревшим


    private readonly List<States> _states = new List<States>(32);

    private Vector3 _visualPosition;          // то, что реально отображаем
    private double _lastPacketLocalTime; // локальное время прихода последнего пакета
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

        //Логика в уроке
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
        //            Debug.LogWarning("Не обрабатывается изменение поля " + dataChange.Field);
        //            break;
        //    }
        //}
        //transform.position = position;
    }
    private Vector3 GetServerPosition(List<DataChange> changes)
    {
        Vector3 position = _lastServerPosition;
        foreach (var dataСhange in changes)
        {
            switch (dataСhange.Field)
            {
                case "x":
                    position.x = (float)dataСhange.Value;
                    break;
                case "y":
                    position.z = (float)dataСhange.Value;
                    break;
            }
        }
        return position;
    }

    private void PushSnapshot(Vector3 position, double time)
    {
        // Вычисляем скорость от предыдущего валидного снапшота
        Vector3 velocity = Vector3.zero;
        int n = _states.Count;
        if (n > 0)
        {
            var prev = _states[n - 1];
            double delta = time - prev.Time;
            if (delta > 0.0005)
                velocity = (position - prev.Position) / (float)delta;
            else
                velocity = prev.Velocity; // слишком малая delta — сохраняем предыдущую скорость
        }

        _states.Add(new States { Time = time, Position = position, Velocity = velocity });

        // Усечение
        while (_states.Count > _maxSnapshots)
            _states.RemoveAt(0);        
    }
    private bool TryGetInterpolatedTarget(double playbackTime, out Vector3 target)
    {
        target = default;
        int count = _states.Count;
        if (count == 0) return false;

        // Если playback раньше самого старого — берём старейший
        if (playbackTime <= _states[0].Time)
        {
            target = _states[0].Position;
            return true;
        }

        // Ищем два соседних снапшота для интерполяции
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

        // Если playback позже последнего — потребуется экстраполяция
        return false;
    }

    private Vector3 GetExtrapolatedTarget(double playbackTime)
    {
        // Экстраполируем от последнего снапшота с ограничением
        var last = _states[_states.Count - 1];
        double delta = Mathf.Min((float)(playbackTime - last.Time), _extrapolationLimit);
        if (delta <= 0.0)
            return last.Position;

        // Затухание скорости при длительном отсутствии апдейтов
        float damp = Mathf.Exp(-_velocityDamping * (float)(playbackTime - last.Time));
        Vector3 v = last.Velocity * damp;
        return last.Position + v * (float)delta;
    }
    
    private void FixedUpdate()
    {
        if (_states.Count == 0)
        {
            // Нет данных — остаёмся где были
            transform.position = _visualPosition;
            return;
        }

        // Рендерим “позади” на interpolationDelay
        double playbackTime = _now - _interpolationDelay;

        Vector3 logicalTarget;
        bool hasInterp = TryGetInterpolatedTarget(playbackTime, out logicalTarget);
        if (!hasInterp)
        {
            // Переходим к экстраполяции с ограничением
            logicalTarget = GetExtrapolatedTarget(playbackTime);
        }

        // Определяем, не устарели ли данные
        bool stale = (_now - _lastPacketLocalTime) > _staleTimeout;

        // Визуальное движение: ограниченная скорость + снап порог
        Vector3 toTarget = logicalTarget - _visualPosition;
        float dist = toTarget.magnitude;

        if (dist > _snapDistance)
        {
            // Слишком большая ошибка — единичный снап
            _visualPosition = logicalTarget;
        }
        else
        {
            // Догоняем с ограниченной скоростью; при stale уменьшим скорость слегка
            float corrSpeed = stale ? Mathf.Max(2f, _maxCorrectionSpeed * 0.6f) : _maxCorrectionSpeed;
            Vector3 step = Vector3.ClampMagnitude(toTarget, corrSpeed * Time.fixedDeltaTime);
            _visualPosition += step;
        }

        transform.position = _visualPosition;
    }
}
 
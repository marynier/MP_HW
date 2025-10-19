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
    private List<States> _states = new List<States>();
    private Vector3 _currentPosition;
    private Vector3 _lastServerPosition;
    private bool _needsSmoothCorrection;
    private double _lastUpdateTime;

    [SerializeField] private double _timeoutDuration = 0.001; // время, после которого считаем данные устаревшими
    

    private void Start()
    {
        _currentPosition = transform.position;
        _lastServerPosition = _currentPosition;
        _lastUpdateTime = Time.realtimeSinceStartupAsDouble;
    }

    internal void OnChange(List<DataChange> changes)
    {
        Vector3 serverPosition = GetServerPosition(changes);
        _lastServerPosition = serverPosition;
        _lastUpdateTime = Time.realtimeSinceStartupAsDouble;


        SaveState(serverPosition, _lastUpdateTime);
        RecalculateVelocities();

        _needsSmoothCorrection = true;

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

    private void SaveState(Vector3 position, double time)
    {
        _states.Add(new States
        {
            Time = time,
            Position = position,
            Velocity = Vector3.zero
        });

        // Ограничиваем размер буфера
        if (_states.Count > 2)
            _states.RemoveAt(0);
        ;
    }
    private void RecalculateVelocities()
    {
        if (_states.Count < 2) return;

        States prevState = _states[0];
        States currentState = _states[1];

        double deltaTime = currentState.Time - prevState.Time;

        if (deltaTime > 0.001f)
        {
            currentState.Velocity = (currentState.Position - prevState.Position) / (float)deltaTime;
            _states[1] = currentState;
        }
    }
    private void FixedUpdate()
    {
        double currentTime = Time.realtimeSinceStartupAsDouble;
        double timeSinceLastUpdate = currentTime - _lastUpdateTime;
        // Проверяем, не устарели ли данные от сервера
        if (timeSinceLastUpdate > _timeoutDuration)
        {
            // Данные устарели - останавливаем объект
            if (_states.Count > 0)
            {
                // Используем последнюю известную позицию от сервера
                States latestState = _states[_states.Count - 1];
                float correctionSpeed = CalculateCorrectionSpeed();
                _currentPosition = Vector3.Lerp(
                    _currentPosition,
                    latestState.Position,
                    correctionSpeed * Time.deltaTime
                    );

                // Останавливаем движение (обнуляем скорость во всех состояниях)
                for (int i = 0; i < _states.Count; i++)
                {
                    States state = _states[i];
                    state.Velocity = Vector3.zero;
                    _states[i] = state;
                }
            }
        }

        else if (_states.Count > 0)
        {
            States latestState = _states[_states.Count - 1];

            if (_states.Count == 1)
            {
                // Если есть только одно состояние - просто применяем позицию
                _currentPosition = latestState.Position;
            }
            else
            {
                // Экстраполируем позицию на основе скорости                
                _currentPosition = latestState.Position + latestState.Velocity * (float)timeSinceLastUpdate;
            }
        }

        // Плавная коррекция позиции при получении новых данных
        if (_needsSmoothCorrection)
        {
            float correctionSpeed = CalculateCorrectionSpeed();
            transform.position = Vector3.Lerp(
                transform.position,
                _currentPosition,
                correctionSpeed * Time.deltaTime
            );

            // Отключаем коррекцию когда близко к целевой позиции
            if (Vector3.Distance(transform.position, _currentPosition) < 0.1f)
            {
                _needsSmoothCorrection = false;
            }
        }
        else
        {
            // Обычное движение с экстраполяцией
            transform.position = _currentPosition;
        }
    }
    private float CalculateCorrectionSpeed()
    {
        if (_states.Count < 2) return 5f;

        // Рассчитываем скорость коррекции на основе реальной скорости объекта
        float currentSpeed = _states[1].Velocity.magnitude;
        return Mathf.Clamp(currentSpeed * 2f, 2f, 10f);
    }
}

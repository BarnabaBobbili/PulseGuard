using PulseGuard.Models;

namespace PulseGuard.Services
{
    public class VitalSimulatorService
    {
        private readonly Random _random = new Random();

        private double _timeElapsed = 0.0;
        private double _currentSpO2 = 97.0;
        private int _currentBpm = 75;

        private double _nextTachycardiaAt = 20.0;
        private double _tachycardiaEndAt = 0.0;
        private double _nextDesatAt = 35.0;
        private double _desatEndAt = 0.0;

        private const double DeltaTime = 1.0 / 60.0;

        public VitalRecord GenerateNext()
        {
            _timeElapsed += DeltaTime;

            UpdateHeartRate();
            UpdateSpO2();
            double ecgValue = ComputeEcg();

            return new VitalRecord
            {
                Timestamp = DateTime.UtcNow,
                EcgValue = ecgValue,
                SpO2Value = Math.Round(_currentSpO2, 1),
                HeartRateBpm = _currentBpm
            };
        }

        private void UpdateHeartRate()
        {
            if (_timeElapsed >= _nextTachycardiaAt && _timeElapsed > _tachycardiaEndAt)
            {
                _currentBpm = _random.Next(125, 145);
                _tachycardiaEndAt = _timeElapsed + _random.Next(5, 10);
                _nextTachycardiaAt = _tachycardiaEndAt + _random.Next(15, 30);
            }
            else if (_timeElapsed > _tachycardiaEndAt && _currentBpm > 100)
            {
                _currentBpm = _random.Next(65, 85);
            }
            else
            {
                int drift = _random.Next(-1, 2);
                _currentBpm = Math.Clamp(_currentBpm + drift, 60, 100);
            }
        }

        private void UpdateSpO2()
        {
            if (_timeElapsed >= _nextDesatAt && _timeElapsed > _desatEndAt)
            {
                _currentSpO2 = _random.Next(85, 90);
                _desatEndAt = _timeElapsed + _random.Next(4, 8);
                _nextDesatAt = _desatEndAt + _random.Next(20, 40);
            }
            else if (_timeElapsed > _desatEndAt && _currentSpO2 < 93.0)
            {
                _currentSpO2 = _random.NextDouble() * (99.0 - 95.0) + 95.0;
            }
            else
            {
                double drift = (_random.NextDouble() - 0.5) * 0.2;
                _currentSpO2 = Math.Clamp(_currentSpO2 + drift, 94.0, 99.0);
            }
        }

        private double ComputeEcg()
        {
            double frequency = _currentBpm / 60.0;
            double sineWave = Math.Sin(2 * Math.PI * frequency * _timeElapsed);

            double beatPhase = (_timeElapsed * frequency) % 1.0;
            double qrsSpike = 0.0;
            if (beatPhase > 0.45 && beatPhase < 0.55)
            {
                double x = (beatPhase - 0.5) / 0.02;
                qrsSpike = 2.0 * Math.Exp(-x * x);
            }

            double u1 = 1.0 - _random.NextDouble();
            double u2 = 1.0 - _random.NextDouble();
            double noise = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2) * 0.08;

            return sineWave + qrsSpike + noise;
        }
    }
}

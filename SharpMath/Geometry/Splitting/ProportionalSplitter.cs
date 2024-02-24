﻿namespace SharpMath.Geometry.Splitting;

public readonly struct ProportionalSplitter : IIntervalSplitter
{
    public double DischargeRatio { get; }

    private readonly int _steps;
    private readonly double _lengthCoefficient;

    // Todo добавить возможность ограничивать минимальную длину элемента
    public ProportionalSplitter(int steps, double dischargeRatio)
    {
        if (Math.Abs(DischargeRatio - 1d) < 1e-15)
            throw new NotSupportedException();

        _steps = steps;
        DischargeRatio = dischargeRatio;
        _lengthCoefficient = (DischargeRatio - 1d) / (Math.Pow(DischargeRatio, _steps) - 1d);
    }

    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length * _lengthCoefficient;

        var stepNumber = 0;
        var value = interval.Begin;

        while (interval.Has(value))
        {
            yield return value;
            var nextValue = interval.Begin + step * (Math.Pow(DischargeRatio, stepNumber + 1) - 1d) / (DischargeRatio - 1d);

            if (value == nextValue)
            {
                throw new NotImplementedException($"Следующий шаг разбиения привел к тому же значению {value}");
            }
            
            value = nextValue;
            stepNumber++;
        }
    }
}
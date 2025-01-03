﻿using Skadi.FEM.Core.BasisFunctions;

namespace Skadi.FEM._1D.BasisFunctions;

public class ExplicitFunction1D : IBasisFunction<double>
{
    private readonly Func<double, double> _function;

    public ExplicitFunction1D(Func<double, double> function)
    {
        _function = function;
    }
    
    public double Evaluate(double point) => _function(point);
}
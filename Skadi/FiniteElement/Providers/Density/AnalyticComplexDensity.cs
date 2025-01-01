﻿using System.Numerics;
using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.Assembling.Params;
using Skadi.Matrices.Sparse;

namespace Skadi.FiniteElement.Providers.Density;

public class AnalyticComplexDensity : INodeDefinedParameter<Complex>
{
    private readonly Context<Point2D, IElement, SparseMatrix> _context;
    private readonly Func<Point2D, Complex> _func;

    public AnalyticComplexDensity(Context<Point2D, IElement, SparseMatrix> context, Func<Point2D, Complex> func)
    {
        _context = context;
        _func = func;
    }

    public Complex Get(int nodeIndex)
    {
        var node = _context.Grid.Nodes[nodeIndex];
        return _func(node);
    }
}
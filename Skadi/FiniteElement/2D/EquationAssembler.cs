﻿using Skadi.FEM.Core;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.Assembling;
using Skadi.FiniteElement.Core.Assembling.Boundary.First;
using Skadi.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;

namespace Skadi.FiniteElement._2D;

public class EquationAssembler
{
    public Equation<SparseMatrix> FinalEquation => _context.Equation;

    private readonly Context<Point2D, IElement, SparseMatrix> _context;
    private readonly IStackLocalAssembler<IElement> _localAssembler;
    private readonly IStackInserter<SparseMatrix> _inserter;
    private readonly IFirstBoundaryApplier<SparseMatrix> _firstBoundaryApplier;
    private readonly IHarmonicSecondBoundaryApplier<SparseMatrix> _harmonicSecondBoundaryApplier;

    public EquationAssembler(
        Context<Point2D, IElement, SparseMatrix> context,
        IStackLocalAssembler<IElement> localAssembler,
        IStackInserter<SparseMatrix> inserter,
        IFirstBoundaryApplier<SparseMatrix> firstBoundaryApplier,
        IHarmonicSecondBoundaryApplier<SparseMatrix> harmonicSecondBoundaryApplier
    )
    {
        _context = context;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _firstBoundaryApplier = firstBoundaryApplier;
        _harmonicSecondBoundaryApplier = harmonicSecondBoundaryApplier;
    }

    public EquationAssembler BuildEquation(Context<Point2D, IElement, SparseMatrix> context)
    {
        var equation = context.Equation;

        var matrix = new StackMatrix(stackalloc double[8 * 8], 8);
        Span<double> vector = stackalloc double[8];
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        foreach (var element in context.Grid.Elements)
        {
            _localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);
            
            _localAssembler.AssembleRightSide(element, vector, indexes);
             var localRightSide = new StackLocalVector(vector, indexes);
            _inserter.InsertVector(equation.RightSide, localRightSide);
        }

        return this;
    }

    public EquationAssembler ApplyFirstBoundary(Context<Point2D, IElement, SparseMatrix> context)
    {
        var equation = context.Equation;

        foreach (var condition in _context.FirstConditions)
        {
            _firstBoundaryApplier.Apply(equation, condition);
        }

        return this;
    }

    public EquationAssembler ApplySecondConditions(Context<Point2D, IElement, SparseMatrix> context)
    {
        foreach (var condition in context.SecondConditions)
        {
            _harmonicSecondBoundaryApplier.Apply(context.Equation, condition);
        }

        return this;
    }
}
﻿using Skadi.Vectors;

namespace Skadi.Matrices.Transformation;

public class InverseMatrix : IMatrixTransformation
{
    public Vector SolveEqualTo(Vector rightSide)
    {
        // (A^-1) * x = b
        // => A*b
        throw new NotImplementedException();
    }
}
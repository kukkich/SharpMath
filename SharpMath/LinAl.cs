using SharpMath.Matrices;
using SharpMath.Vectors;

namespace SharpMath;

public static class LinAl
{
    public static Vector Sum(Vector v, Vector u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, 1.0, resultMemory);
    }
    public static Vector Subtract(Vector v, Vector u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, -1.0, resultMemory);
    }
    public static Vector LinearCombination(
        Vector v, Vector u,
        double vCoefficient, double uCoefficient,
        Vector? resultMemory = null
    )
    {
        AssertSameSize(v, u);
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = v[i] * vCoefficient + u[i] * uCoefficient;

        return resultMemory;
    }

    public static Vector Multiply(double coefficient, Vector v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = coefficient * v[i];

        return resultMemory;
    }
    public static Matrix Multiply(double coefficient, MatrixBase a, Matrix? resultMemory = null)
    {
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }
    public static ImmutableMatrix Multiply(double coefficient, ImmutableMatrix a)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
    }
    public static ImmutableMatrix Multiply(ImmutableMatrix a, double coefficient)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
    }

    public static Vector Multiply(MatrixBase a, Vector v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        var result = new double[v.Length];

        for (var i = 0; i < v.Length; i++)
            for (var j = 0; j < v.Length; j++)
                result[i] += a[i, j] * v[j];

        return new Vector(result);
    }

    //Todo refactor (?)
    public static Vector Multiply(SymmetricSparseMatrix matrix, Vector x, Vector? resultMemory = null)
    {
        if (resultMemory == null)
        {
            resultMemory = new Vector(new double[x.Length]);
        }
        else
        {
            AssertSameSize(x, resultMemory);
        }
        if (matrix.RowIndexes.Length != x.Length)
        {
            throw new ArgumentOutOfRangeException($"{nameof(matrix.RowIndexes)} and {nameof(x)}", "must have the same length");
        }

        for (var i = 0; i < x.Length; i++)
            resultMemory[i] = x[i] * matrix.Diagonal[i];

        for (var i = 0; i < x.Length; i++)
        {
            foreach (var indexValue in matrix[i])
            {
                resultMemory[i] += indexValue.Value * x[indexValue.ColumnIndex];
                resultMemory[indexValue.ColumnIndex] += indexValue.Value * x[i];
            }
        }

        return resultMemory;
    }
    public static Matrix Multiply(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertSameSize(a, b);
        if (a == resultMemory || b == resultMemory) 
            throw new ArgumentOutOfRangeException($"{nameof(resultMemory)}", "can't be equal to one of the arguments");
        resultMemory = ValidateOrAllocateIfNullForMultiplying(a, b, resultMemory).AsMutable();

        for (var aRow = 0; aRow < a.Rows; aRow++)
        {
            for (var bColumn = 0; bColumn < b.Columns; bColumn++)
            {
                double sum = 0;
                for (var i = 0; i < a.Columns; i++)
                    sum += a[aRow, i] * b[i, bColumn];

                resultMemory[aRow, bColumn] = sum;
            }
        }

        return resultMemory;
    }
    public static MatrixBase Sum(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertCanBeMultiplied(a, b);
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Rows; j++)
                resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }


    // TODO вынести функционал по валидации и выделению памяти в отдельный класс
    public static MatrixBase ValidateOrAllocateIfNull(MatrixBase a, MatrixBase? b)
    {
        if (b is null)
            b = new Matrix(new double[a.Rows, a.Columns]);
        else AssertSameSize(a, b);

        return b;
    }
    public static void AssertSameSize(MatrixBase a, MatrixBase b)
    {
        if (a.Rows != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same rows");
        if (a.Columns != b.Columns)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same columns");
    }

    public static MatrixBase ValidateOrAllocateIfNullForMultiplying(MatrixBase a, MatrixBase b, MatrixBase? c)
    {
        AssertCanBeMultiplied(a, b);
        if (c is null)
            c = new Matrix(new double[a.Rows, b.Columns]);
        else if (a.Rows != c.Rows || b.Columns != c.Columns)
            throw new ArgumentOutOfRangeException(nameof(c), "сan't be the result of a multiplication");

        return c;
    }
    public static void AssertCanBeMultiplied(MatrixBase a, MatrixBase b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "can't be multiplied");
    }
    
    public static void ValidateOrAllocateIfNull(Vector v, ref Vector? u)
    {
        if (u is null)
            u = Vector.Create(v.Length);
        else AssertSameSize(v, u);
    }
    private static void AssertSameSize(Vector v, Vector u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)}", "must have the same length");
    }
}
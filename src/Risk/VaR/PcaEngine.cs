#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace FixedIncomePricingLibrary.Risk.VaR;

/// <summary>
/// Performs Principal Component Analysis (PCA) on interest rate changes.
/// </summary>
public class PcaEngine
{
    /// <summary>
    /// Executes PCA on a matrix of rate changes (rows = days, cols = tenors).
    /// </summary>
    public PcaResult RunPca(double[,] rateChanges)
    {
        var matrix = Matrix<double>.Build.DenseOfArray(rateChanges);

        // Calculate Covariance Matrix (X^T * X) / (n - 1)
        var cov = matrix.Transpose().Multiply(matrix).Divide(matrix.RowCount - 1);

        // Eigen decomposition
        // Covariance matrices are symmetric, so we can expect real eigenvalues
        Evd<double> evd = cov.Evd();

        // evd.EigenValues is Vector<Complex<double>>
        var eigenvalues = evd.EigenValues.Select(c => c.Real).ToArray();
        var eigenvectors = evd.EigenVectors;

        return new PcaResult
        {
            EigenValues = eigenvalues,
            EigenVectors = eigenvectors.ToArray()
        };
    }
}

public class PcaResult
{
    public double[] EigenValues { get; set; } = Array.Empty<double>();
    public double[,] EigenVectors { get; set; } = new double[0, 0];
}

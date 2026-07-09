namespace EmissorNotaFiscal.Domain.Interfaces;

public interface ICaptchaSolverService
{
    Task<string> SolveAsync(string base64Image, CancellationToken cancellationToken = default);
}

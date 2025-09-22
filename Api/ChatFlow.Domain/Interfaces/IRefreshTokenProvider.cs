namespace ChatFlow.Domain.Interfaces
{
    public interface IRefreshTokenProvider
    {
        string GeneratePlainToken(int bytes = 64);
        string ComputeSha256(string plain);
    }
}

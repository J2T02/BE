namespace SWP.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(string accName, int accId, string roleName);
    }
}

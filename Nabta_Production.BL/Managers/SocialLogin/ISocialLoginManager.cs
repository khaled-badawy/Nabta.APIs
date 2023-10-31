namespace Nabta_Production.BL
{
    public interface ISocialLoginManager
    {
       Task<AuthModel> RegisterUserByGoogleAuth(string token);
    }
}

namespace Inventory.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Core.Entities.User> RegisterAsync(string username, string password, string role);
        Task<string?> LoginAsync(string username, string password);
    }
}

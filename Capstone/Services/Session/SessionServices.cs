namespace Capstone.Services.Session
{
    public interface ISessionService
    {
        int? GetUserId();
        void SetUserId(int userId);
        string? GetString(string key);
        void SetString(string key, string value);
        void Remove(string key);
        bool? IsAdmin();
        void ClearSession();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId() => _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

        public void SetUserId(int userId) =>
            _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId);

        public string? GetString(string key) =>
            _httpContextAccessor.HttpContext?.Session.GetString(key);

        public void SetString(string key, string value) =>
            _httpContextAccessor.HttpContext?.Session.SetString(key, value);

        public void Remove(string key) =>
            _httpContextAccessor.HttpContext?.Session.Remove(key);

        public bool? IsAdmin() =>
            _httpContextAccessor.HttpContext?.Session.GetString("IsAdmin") == "true" ? true : false;

        public void ClearSession()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }



    }
}

using System.Threading.Tasks;

namespace EventManagement.Api.Features.Users.LogOff
{
    public class LogOffHandler
    {
        public Task<LogOffResponse> HandleAsync(LogOffRequest request)
        {
            // In a stateless API, there's no session to invalidate
            // The client will be responsible for removing any stored tokens
            
            var response = new LogOffResponse
            {
                Success = true
            };
            
            return Task.FromResult(response);
        }
    }
}

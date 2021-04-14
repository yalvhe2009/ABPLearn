using System.Threading.Tasks;
using Abp.Application.Services;
using Blog.Sessions.Dto;

namespace Blog.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

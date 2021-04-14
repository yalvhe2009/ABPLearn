using System.Threading.Tasks;
using Blog.Configuration.Dto;

namespace Blog.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}

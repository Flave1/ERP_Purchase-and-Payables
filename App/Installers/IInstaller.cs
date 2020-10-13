using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Puchase_and_payables.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}

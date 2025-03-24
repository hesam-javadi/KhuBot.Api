using KhuBot.Application.IRepositories;
using KhuBot.Application.IServices;
using KhuBot.Application.Services;
using KhuBot.Infrastructure.Repositories;

namespace KhuBot.Api.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services


            services.AddScoped<IAuthServices, AuthServices>();

            services.AddScoped<IChatServices, ChatServices>();

            #endregion

            #region repositories

            services.AddScoped<IChatBotRepository, DeepSeekChatBotRepository>();

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            #endregion

            services.AddHttpContextAccessor();

            services.AddHttpClient();
        }
    }
}

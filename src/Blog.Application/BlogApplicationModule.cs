using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Blog.Authorization;
using Blog.Features;

namespace Blog
{
    [DependsOn(
        typeof(BlogCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class BlogApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BlogAuthorizationProvider>();
            Configuration.Features.Providers.Add<BlogFeatureProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(BlogApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}

using System.Reflection;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core;
using Castle.MicroKernel;

namespace Abp.Runtime.Validation.Interception
{
    internal static class ValidationInterceptorRegistrar
    {
        public static void Initialize(IIocManager iocManager)
        {
            iocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private static void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            System.Console.WriteLine($"又有新的实例被注册到容器里了：{key}");
            if (typeof(IApplicationService).GetTypeInfo().IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(AbpAsyncDeterminationInterceptor<ValidationInterceptor>)));
            }
        }
    }
}
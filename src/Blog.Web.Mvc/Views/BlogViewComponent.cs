using Abp.AspNetCore.Mvc.ViewComponents;

namespace Blog.Web.Views
{
    public abstract class BlogViewComponent : AbpViewComponent
    {
        protected BlogViewComponent()
        {
            LocalizationSourceName = BlogConsts.LocalizationSourceName;
        }
    }
}

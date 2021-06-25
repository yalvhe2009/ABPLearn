using System;
using Abp.DynamicEntityProperties;
using Abp.UI.Inputs;
using Blog.Contracts;

namespace Blog.DynamicPropertys
{
    public class BlogDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            //动态属性支持的输入类型
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            
            //支持动态属性的实体
            context.Manager.AddEntity<Contract, Guid>();
        }
    }
}
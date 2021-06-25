using Abp.Application.Features;
using Abp.Localization;
using Abp.UI.Inputs;

namespace Blog.Features
{
    public class BlogFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var 合同管理 = context.Create(
                BlogFeatureNames.合同管理,
                defaultValue: "true",
                displayName: L(BlogFeatureNames.合同管理),
                inputType: new CheckboxInputType()
            );

            合同管理.CreateChildFeature(
                BlogFeatureNames.合同管理_读取,
                defaultValue: "true",
                displayName: L(BlogFeatureNames.合同管理_读取),
                inputType: new CheckboxInputType()
            );
            
            合同管理.CreateChildFeature(
                BlogFeatureNames.合同管理_创建,
                defaultValue: "false",
                displayName: L(BlogFeatureNames.合同管理_创建),
                inputType: new CheckboxInputType()
            );

            context.Create(
                "Blog模块的Feature",
                defaultValue: "B",
                displayName: L("测试显示名称"),
                inputType: new ComboboxInputType(
                    new StaticLocalizableComboboxItemSource(
                        new LocalizableComboboxItem("A", L("Selection A")),
                        new LocalizableComboboxItem("B", L("Selection B")),
                        new LocalizableComboboxItem("C", L("Selection C"))
                    )
                )
            );
        }
        
        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AppConsts.LocalizationSourceName);
        }
    }
}
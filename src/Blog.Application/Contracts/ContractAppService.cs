using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.DynamicEntityProperties;
using Abp.UI.Inputs;
using Blog.Features;
using Newtonsoft.Json;

namespace Blog.Contracts
{
    public class ContractAppService : BlogAppServiceBase, IContractAppService
    {
        private readonly IRepository<Contract, Guid> _contractRepository;
        private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;
        private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;
        private readonly IDynamicPropertyValueManager _dynamicPropertyValueManagerManager;
        private readonly IDynamicPropertyManager _dynamicPropertyManager;

        public ContractAppService(
            IRepository<Contract, Guid> contractRepository,
            IDynamicEntityPropertyManager dynamicEntityPropertyManager,
            IDynamicEntityPropertyValueManager dynamicEntityPropertyValueManager,
            IDynamicPropertyValueManager dynamicPropertyValueManagerManager,//动态属性值管理器
            IDynamicPropertyManager dynamicPropertyManager//动态属性管理器
                )
        {
            _contractRepository = contractRepository;
            _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
            _dynamicEntityPropertyValueManager = dynamicEntityPropertyValueManager;
            _dynamicPropertyValueManagerManager = dynamicPropertyValueManagerManager;
            _dynamicPropertyManager = dynamicPropertyManager;
        }

        [RequiresFeature(BlogFeatureNames.合同管理_读取)]
        public async Task<List<Contract>> GetAllContractListAsync()
        {
            var list = await _contractRepository.GetAllListAsync();

            return list;
        }


        [RequiresFeature(BlogFeatureNames.合同管理_创建)]
        public async Task CreateContract(Contract contract)
        {
            await _contractRepository.InsertAsync(contract);
        }

        /// <summary>
        /// 创建Property
        /// </summary>
        public async Task DynamicPropertyTest()
        {
            var contractProperty = new DynamicProperty
            {
                PropertyName = "CompanyName", //我方公司名称
                InputType = InputTypeBase.GetName<SingleLineStringInputType>(),
                Permission = "",
                TenantId = 1
            };
            await _dynamicPropertyManager.AddAsync(contractProperty);
        }
        
        /// <summary>
        /// 创建值
        /// </summary>
        public async Task DynamicPropertyTest2()
        {
            var contractProperty = await _dynamicPropertyManager.GetAsync("CompanyName");

            var yitai = new DynamicPropertyValue(contractProperty, "伊泰集团", 1);

            var zhujiang = new DynamicPropertyValue(contractProperty, "珠江投资管理集团", 1);

            await _dynamicPropertyValueManagerManager.AddAsync(yitai);
            await _dynamicPropertyValueManagerManager.AddAsync(zhujiang);

        }

        /// <summary>
        /// 查询值
        /// </summary>
        public void DynamicPropertyTest3()
        {
            var contractProperty = _dynamicPropertyManager.Get("CompanyName");
            var dynamicPropertyValue = _dynamicPropertyValueManagerManager.Get(contractProperty.Id);
            var list = _dynamicPropertyValueManagerManager.GetAllValuesOfDynamicProperty(contractProperty.Id);
            Console.WriteLine(dynamicPropertyValue);
        }

        /// <summary>
        /// 实体-创建property
        /// </summary>
        public void DynamicPropertyTest4()
        {
            var contractProperty = _dynamicPropertyManager.Get("CompanyName");
            var dynamicEntityProperty = new DynamicEntityProperty
            {
                DynamicPropertyId = contractProperty.Id,
                EntityFullName = typeof(Contract).FullName
            };
            _dynamicEntityPropertyManager.Add(dynamicEntityProperty);
            
        }

        /// <summary>
        /// 实体-创建value
        /// </summary>
        public void DynamicPropertyTest5()
        {
            var companyNameProperty = _dynamicPropertyManager.Get("CompanyName");
            List<DynamicEntityProperty> propertyList = _dynamicEntityPropertyManager.GetAll(typeof(Contract).FullName);
            DynamicEntityProperty dynamicEntity = propertyList.FirstOrDefault(x => x.DynamicPropertyId == companyNameProperty.Id);

            DynamicEntityPropertyValue value1 = new DynamicEntityPropertyValue(dynamicEntity, "08d936e2-88fa-4ca0-823b-b7f454685c8d",
                "伊泰集团", 1);
            
            _dynamicEntityPropertyValueManager.Add(value1);
        }

        /// <summary>
        /// 实体-查询
        /// </summary>
        public string DynamicPropertyTest6()
        {
            var companyNameProperty = _dynamicPropertyManager.Get("CompanyName");

            List<DynamicEntityPropertyValue> dynamicEntityPropertyValues = _dynamicEntityPropertyValueManager.GetValues(typeof(Contract).FullName, "08d936e2-88fa-4ca0-823b-b7f454685c8d", companyNameProperty.Id);

            return JsonConvert.SerializeObject(dynamicEntityPropertyValues);
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Blog.Contracts
{
    [Table("Contract")]
    public class Contract : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 合同数量
        /// </summary>
        public string ContractName { get; set; }

        /// <summary>
        /// 相对方名称
        /// </summary>
        public string OppositePartyName { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradeAmount { get; set; }
    }
}
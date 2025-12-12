using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public class ContractModel
    {
        private readonly Contracts contract;
        public ContractModel(Contracts contract)
        {
            this.contract = contract;
        }

        public string ContractNumber
        {
            get { return contract.ContractNumber; }
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "ContractNumber", contract?.ContractNumber ?? "" },
                { "StartDate", contract?.StartDate.ToString("dd.MM.yyyy") ?? "" },
                { "EndDate", contract?.EndDate.ToString("dd.MM.yyyy") ?? "" },
                { "MonthlyRent", contract?.MonthlyRent.ToString("F2") ?? "0,00" },
                { "Deposit", contract.Deposit.HasValue? contract.Deposit.Value.ToString("F2") : "" },
                { "PaymentDueDay", contract?.PaymentDueDay.ToString() ?? "" },
                { "TenantName", contract?.Tenants?.FullName ?? "" },
                { "TenantPhone", contract?.Tenants?.PhoneNumber ?? "" },
                { "RealtyAddress", contract?.Realty?.Address ?? "" },
                { "ContractStatus", contract?.ContractStatuses?.StatusName ?? "" }
            };
        }
    }
}
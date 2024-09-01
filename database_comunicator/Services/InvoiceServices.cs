using database_comunicator.Data;
using database_comunicator.Models;
using database_comunicator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace database_comunicator.Services
{
    public interface IInvoiceServices
    {
        public Task<GetOrgsForInvocie> GetOrgsForInvocie(int userId);
        public Task<IEnumerable<GetTaxes>> GetTaxes();
        public Task<IEnumerable<GetPaymentMethods>> GetPaymentMethods();
        public Task<IEnumerable<GetPaymentStatuses>> GetPaymentStatuses();
    }
    public class InvoiceServices : IInvoiceServices
    {
        private readonly HandlerContext _handlerContext;
        public InvoiceServices(HandlerContext handlerContext)
        {
            _handlerContext = handlerContext;
        }
        public async Task<GetOrgsForInvocie> GetOrgsForInvocie(int userId)
        {
            var result = await _handlerContext.Organizations
                .Where(e => e.OrganizationId != userId)
                .Select(e => new RestOrgs
                {
                    OrgId = e.OrganizationId,
                    OrgName = e.OrgName
                }).ToListAsync();
            var userOrg = await _handlerContext.Organizations
                .Where(e => e.OrganizationId == userId)
                .Select(e => new RestOrgs
                {
                    OrgId = e.OrganizationId,
                    OrgName = e.OrgName
                }).FirstAsync();
            return new GetOrgsForInvocie
            {
                UserOrgId = userOrg.OrgId,
                OrgName = userOrg.OrgName,
                RestOrgs = result
            };
        }
        public async Task<IEnumerable<GetTaxes>> GetTaxes()
        {
            return await _handlerContext.Taxes.Select(e => new GetTaxes
            {
                TaxesId = e.TaxesId,
                TaxesValue = e.TaxValue
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetPaymentMethods>> GetPaymentMethods()
        {
            return await _handlerContext.PaymentMethods.Select(e => new GetPaymentMethods
            {
                PaymentMethodId = e.PaymentMethodId,
                MethodName = e.MethodName,
            }).ToListAsync();
        }
        public async Task<IEnumerable<GetPaymentStatuses>> GetPaymentStatuses()
        {
            return await _handlerContext.PaymentStatuses.Select(e => new GetPaymentStatuses
            { 
                PaymentStatusId = e.PaymentStatusId,
                StatusName = e.StatusName,
            }).ToListAsync();
        }
    }
}

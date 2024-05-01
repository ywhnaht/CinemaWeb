using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Models.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContextBase context ,invoice invoiceItem);
        //string CreatePaymentUrl(HttpContextBase httpContext, invoice newInvoice);
        VnPaymentResponseModel PaymentExecute(FormCollection collection);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CinemaWeb.Models.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(HttpContent content, VnPaymentRequestModel model);

    }
}

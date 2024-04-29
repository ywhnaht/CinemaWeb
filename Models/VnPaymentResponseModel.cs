using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaWeb.Models
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }

    }
    public class VnPaymentRequestModel
    {
        public string full_name { get; set; }
        public string total_money { get; set; }
    }
}
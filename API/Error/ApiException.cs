using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Error
{
    public class ApiException
    {
        public ApiException(int StatusCode , string Message, string Details)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Details = Details;
        }

        public int StatusCode { get; }
        public string Message { get; }
        public string Details { get; }
    }
}
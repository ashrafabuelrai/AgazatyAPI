﻿namespace Agazaty.Domain.Entities
{
    public class ForgetPassResponse
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

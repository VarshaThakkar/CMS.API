﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Vaan.CMS.API.Helpers
{
    public class AppException : Exception
    {
        public AppException() : base()
        {
            
        }
        public AppException(string message) : base(message) { }
        public AppException(string message, params object[] args)
          : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProductoFwkTest.App_Start
{
    public  class StringCon
    {

        public StringCon(string str)
        {
            String = str;
        }

        public  string String { get; set; } 
    }
}
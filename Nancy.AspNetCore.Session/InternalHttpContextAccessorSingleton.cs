using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nancy.AspNetCore.Session
{
    internal class InternalHttpContextAccessorSingleton
    {
        private static InternalHttpContextAccessorSingleton _instance;

        public static InternalHttpContextAccessorSingleton Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new InternalHttpContextAccessorSingleton();
                }
                return _instance;
            }
        }
        public IHttpContextAccessor HttpContextAccessor { get; private set; }
        private InternalHttpContextAccessorSingleton()
        {
            HttpContextAccessor = new NancyHttpContextAccessor();
        }
    }
}
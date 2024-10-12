using Castle.DynamicProxy;
using Serilog;
using System;

namespace Core.API.AdditionalService
{
    public class ServiceActionLogger : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                Log.Debug($"Calling method - {invocation.Method.Name}");
                invocation.Proceed();
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}

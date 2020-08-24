using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Baza_Visual_WPF;

namespace Payroll_Web_Service
{
    [ServiceContract]
    public interface PayrollService
    {
        [OperationContract]
        Stream examineFile(Stream logFile);
    }
}

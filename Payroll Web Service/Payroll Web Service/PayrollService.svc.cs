using Baza_Visual_WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Payroll_Web_Service
{
    [Serializable]
    public class Service1 : PayrollService
    {
        public Stream examineFile(Stream logFile)
        {
            List<UserData> temp = new LineProcessing().processFile(logFile);
            return makeExportFile(temp);
        }

        private Stream makeExportFile(List<UserData> users)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\tempLog\temp.txt"))
            {
                List<string> lines = new List<string>();
                foreach (UserData user in users)
                {
                    sw.WriteLine(user.id + "ids");
                    sw.WriteLine(user.name + "name");
                    foreach (int a in user.days)
                        sw.WriteLine(a + "days");
                    foreach (var a in user.normalMovement)
                        sw.WriteLine(a.first + "`" + a.second + "normalMovement");
                    foreach (var a in user.invalidMovement)
                        sw.WriteLine(a.first + "`" + a.second + "invalidMovement");
                    foreach (var a in user.breakMovement)
                        sw.WriteLine(a.first + "`" + a.second + "breakMovement");
                    foreach (var a in user.officialMovement)
                        sw.WriteLine(a.first + "`" + a.second + "officialMovement");
                    foreach (var a in user.privateMovement)
                        sw.WriteLine(a.first + "`" + a.second + "privateMovement");
                    foreach (var a in user.normalTime)
                        sw.WriteLine(a.day + "`" + a.time.h + "`" + a.time.min + "`" + a.time.s + "normalTime");
                    foreach (var a in user.breakTime)
                        sw.WriteLine(a.day + "`" + a.time.h + "`" + a.time.min + "`" + a.time.s + "breakTime");
                    foreach (var a in user.officalTime)
                        sw.WriteLine(a.day + "`" + a.time.h + "`" + a.time.min + "`" + a.time.s + "officalTime");
                    foreach (var a in user.privateTime)
                        sw.WriteLine(a.day + "`" + a.time.h + "`" + a.time.min + "`" + a.time.s + "privateTime");
                    sw.WriteLine(user.normalSum + "normalSum");
                    sw.WriteLine(user.breakSum + "breakSum");
                    sw.WriteLine(user.officialSum + "officialSum");
                    sw.WriteLine(user.privateSum + "privateSum");
                }
                sw.WriteLine(users[0].month + "month");
                return sw.BaseStream;
            }
        }
    }


}

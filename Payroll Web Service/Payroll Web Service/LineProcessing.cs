using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Baza_Visual_WPF
{
    public class triPair
    {
        public string value;
        public string enter, exit;

        public triPair(string a, string b, string c)
        {
            value = a;
            enter = b;
            exit = c;
        }
    }

    public class LineProcessing
    {
        //List<>
        triPair[] PGNormal = { new triPair("PG 4:", "Prijava UK", "-"), new triPair("PG 5:", "-", "Odjava UK") }; //uso u zgradu normalno --- izaso iz zgrade normalno
        triPair[] PGBreak = { new triPair("PG 10:", "Pauza UK", "-"), new triPair("PG 11:","-", "Kraj pauze UK") }; //ide na pauzu --- vraca se sa pauzu
        triPair[] PGofficial = { new triPair("PG 6:", "Službeni izlazak UK", "-") , new triPair("PG 7:","-", "Službeni povratak UK") }; //izaso sluzbeno napolje --- vraca se sa sluzbenog u zgradu
        triPair[] PGPrivate = { new triPair("PG 8:", "Privatni izlazak UK", "-"), new triPair("PG 9:","-", "Privatni povratak UK") }; //izaso privatno napolje ---vraca se sa privatnog u zgradu
        int whichPG = -1; //0-normal  1-break  2-official  3-private //Dok citam linije iz Log-a, ovde upisujem koji PG sam procitao
        string dayInLog = "1";

        List<UserData> user = new List<UserData>();


        public List<UserData> processFile(Stream logFile)
        {
            using (StreamReader sr = new StreamReader(logFile))
            {
                //string line="";

                //var reversedLines = File.ReadAllLines(logFile).Reverse();
                //string[] reversedLines = File.ReadAllLines(logFile);

                //while ((line = log.ReadLine())!="" && line!=null)
                //{
                //int brcPom = 0;
                bool flag = true;
                //foreach (var line in reversedLines)
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    //Console.OutputEncoding = Encoding.Unicode;
                    //Console.WriteLine(line);
                    // if (line.Contains("+"))
                    //brcPom++;

                    int userIndex = isUserLine(line);
                    int pgIndex = isPGLine(line);
                    //Console.WriteLine(whichPG);
                    if (userIndex != -1 && pgIndex != -1)
                    {
                        //Console.WriteLine("Uso  " + line);
                        /*
                        pom = pom.Substring(0, 2);
                        if (pom[1] == '/')
                            pom = pom[0].ToString();
                        */
                        string pom = getTime(line);
                        List<int> idx = getAllSpaceIndexes(line, '/');
                        pom = line.Substring(idx[0] + 1, idx[1] - idx[0] - 1);
                        //Console.WriteLine(pom);
                        if (pom != dayInLog)
                        {
                            resetLastPGs(last: false);
                            dayInLog = pom;
                        }
                        string id = getId(userIndex, line);
                        int index_of_user = userExist(id);
                        //Console.WriteLine(line);
                        if (index_of_user != -1)
                        {
                            examineLine(line, pgIndex, index_of_user);
                        }
                        else
                        {
                            user.Add(new UserData(id));
                            //byte[] iso88591data = Encoding.GetEncoding("ISO-8859-1").GetBytes(getName(userIndex, line).Trim());
                            //user[user.Count - 1].name = Encoding.UTF8.GetString(iso88591data);;
                            user[user.Count - 1].name = getName(userIndex, line).Trim();
                            //Console.WriteLine("Dodajem novog: " + user[user.Count - 1].name);
                            index_of_user = user.Count - 1;
                            if (flag)
                            {
                                List<int> idx5 = getAllSpaceIndexes(line, '|');
                                string mnth = line.Substring(idx5[1] + 1, idx[0] - idx5[1] - 1);
                                if (mnth == "1") user[index_of_user].month = "January";
                                else if (mnth == "2") user[index_of_user].month = "February";
                                else if (mnth == "3") user[index_of_user].month = "March";
                                else if (mnth == "4") user[index_of_user].month = "April";
                                else if (mnth == "5") user[index_of_user].month = "May";
                                else if (mnth == "6") user[index_of_user].month = "June";
                                else if (mnth == "7") user[index_of_user].month = "July";
                                else if (mnth == "8") user[index_of_user].month = "August";
                                else if (mnth == "9") user[index_of_user].month = "September";
                                else if (mnth == "10") user[index_of_user].month = "October";
                                else if (mnth == "11") user[index_of_user].month = "November";
                                else if (mnth == "12") user[index_of_user].month = "December";

                                flag = !flag;
                            }
                            examineLine(line, pgIndex, index_of_user);
                        }
                    }
                    // Console.WriteLine("Nisam uso  " + line);
                    //if (brcPom == 3)
                    //resetLastPGs();
                }
                resetLastPGs(last: true);
                return user;
            }
        }

        //string name = getName(userIndex, line);

        private void resetLastPGs(bool last = false)
        {
            if (user.Count > 0)
            {
                if (!last)
                {
                    foreach (var a in user)
                    {
                        //if (a.wentOfficiallyOutside == true && a.officialMovement.Count > 0 && a.officialMovement[a.officialMovement.Count - 1].second == null)
                        //{
                        //    a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                        //    a.invalidMovement[a.invalidMovement.Count - 1].second = a.officialMovement[a.officialMovement.Count - 1].first;

                        //    List<int> pom = getAllSpaceIndexes(a.officialMovement[a.officialMovement.Count - 1].first, '/');
                        //    string poms = a.officialMovement[a.officialMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                        //    int pomi = Convert.ToInt32(poms);
                        //    a.days[pomi - 1] = -1;

                        //    a.officialMovement.RemoveAt(a.officialMovement.Count - 1);
                        //    a.wentOfficiallyOutside = false;
                        //    a.lastUsedPG = -1;
                        //    break;
                        //}
                        if (a.officialMovement.Count > 0 && a.officialMovement[a.officialMovement.Count - 1].second == null)
                        {
                            a.wentOfficiallyOutside = true;
                            a.lastUsedPG = -1;
                        }
                        if (a.normalMovement.Count > 0 && a.normalMovement[a.normalMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.normalMovement[a.normalMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.normalMovement[a.normalMovement.Count - 1].first, '/');
                            string poms = a.normalMovement[a.normalMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.normalMovement.RemoveAt(a.normalMovement.Count - 1);
                            a.lastUsedPG = -1;
                        }
                        if (a.breakMovement.Count > 0 && a.breakMovement[a.breakMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.breakMovement[a.breakMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.breakMovement[a.breakMovement.Count - 1].first, '/');
                            string poms = a.breakMovement[a.breakMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.breakMovement.RemoveAt(a.breakMovement.Count - 1);
                            a.lastUsedPG = -1;
                        }
                        if (a.privateMovement.Count > 0 && a.privateMovement[a.privateMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.privateMovement[a.privateMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.privateMovement[a.privateMovement.Count - 1].first, '/');
                            string poms = a.privateMovement[a.privateMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.privateMovement.RemoveAt(a.privateMovement.Count - 1);
                            a.lastUsedPG = -1;
                        }
                        a.lastUsedPG = -1;
                    }
                }
                else if(last)          ///////////////////////////////////////////////////
                {
                    foreach (var a in user)
                    {
                        if (a.normalMovement.Count > 0 && a.normalMovement[a.normalMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.normalMovement[a.normalMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.normalMovement[a.normalMovement.Count - 1].first, '/');
                            string poms = a.normalMovement[a.normalMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.normalMovement.RemoveAt(a.normalMovement.Count - 1);
                        }
                        if (a.breakMovement.Count > 0 && a.breakMovement[a.breakMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.breakMovement[a.breakMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.breakMovement[a.breakMovement.Count - 1].first, '/');
                            string poms = a.breakMovement[a.breakMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.breakMovement.RemoveAt(a.breakMovement.Count - 1);
                        }
                        if (a.officialMovement.Count > 0 && a.officialMovement[a.officialMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.officialMovement[a.officialMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.officialMovement[a.officialMovement.Count - 1].first, '/');
                            string poms = a.officialMovement[a.officialMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.officialMovement.RemoveAt(a.officialMovement.Count - 1);
                        }
                        if (a.privateMovement.Count > 0 && a.privateMovement[a.privateMovement.Count - 1].second == null)
                        {
                            a.invalidMovement.Add(new pair("No exit event on the end of the day"));
                            a.invalidMovement[a.invalidMovement.Count - 1].second = a.privateMovement[a.privateMovement.Count - 1].first;

                            List<int> pom = getAllSpaceIndexes(a.privateMovement[a.privateMovement.Count - 1].first, '/');
                            string poms = a.privateMovement[a.privateMovement.Count - 1].first.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                            int pomi = Convert.ToInt32(poms);
                            a.days[pomi - 1] = -1;

                            a.privateMovement.RemoveAt(a.privateMovement.Count - 1);
                        }
                        a.lastUsedPG = -1;
                    }
                }
            }
        }

        private string getName(int index, string line)
        {
            index += 5;
            while (Char.IsDigit(line[index]))
                index++;
            //Ovde sam prosao id od Usera
            index++;
            List<int> idx = getAllIndexes(line);
            return line.Substring(index, idx[3] - index - 1);
        }
        private void examineLine(string line, int pgIndex, int index_of_user)
        {
            //Console.WriteLine("examine line");
            Console.WriteLine(line);
            if (user[index_of_user].lastUsedPG == -1) //Prvi put danas dobijamo neki event
            {
                //Console.WriteLine("prvi event dns");
                if (whichPG != 0 || (whichPG == 0 && line.Contains(PGNormal[pgIndex].exit))) //Nije prvi event da udje GRESKA
                {
                    //Console.WriteLine("ispred invalid first entry");
                    invalidFirstEntryToday(line, pgIndex, index_of_user);
                }
                else if (whichPG == 0) //uso danas normalno, sve regularno
                {
                    if(user[index_of_user].wentOfficiallyOutside==true)
                    {
                        user[index_of_user].invalidMovement.Add(new pair("No exit event yesterday"));
                        user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                        user[index_of_user].wentOfficiallyOutside = false;
                        user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);
                        List<int> pom = getAllSpaceIndexes(line, '/');
                        string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                        int pomi = Convert.ToInt32(poms);
                        user[index_of_user].days[pomi - 2] = -1;
                    }
                    user[index_of_user].normalMovement.Add(new pair(getTime(line)));
                    user[index_of_user].lastUsedPG = 0;
                }
            }
            else //Ako nije prvi event danas
            {
                //Console.WriteLine("Nije prvi event danas");

                //ispitujem da li je neki ulaz sad
                if (line.Contains(PGNormal[pgIndex].enter) || line.Contains(PGBreak[pgIndex].exit) || line.Contains(PGofficial[pgIndex].exit) || line.Contains(PGPrivate[pgIndex].exit))
                {
                    //Console.WriteLine("neki ulaz sad");
                    //Ako je prethodni neki ulaz onda GRESKA
                    if (user[index_of_user].normalMovement.Count>0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second == null)
                    {
                        //Console.WriteLine("ispred two entry");
                        twoEntryEvents(line, pgIndex, index_of_user);
                    }
                    else //Ako je prethodni neki izlaz onda je mozda dobro
                    {
                        //Console.WriteLine("ispred  exit then entry");
                        exitThenEntry(line, pgIndex, index_of_user);
                    }
                }
                else //Ako je neki izlaz sad
                {
                    //Console.WriteLine("neki izlaz sad");
                    //Console.WriteLine(user[index_of_user].name);
                    //Ako je prethodni neki izlaz onda GRESKA
                    //
                    bool pom1 = (user[index_of_user].normalMovement.Count > 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null) ? true : false;
                    bool pom2 = (user[index_of_user].breakMovement.Count>0 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null) ? true : false;
                    bool pom3 = (user[index_of_user].officialMovement.Count > 0 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null) ? true : false;
                    bool pom4 = (user[index_of_user].privateMovement.Count > 0 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null) ? true : false;
                    if (pom1 || pom2 || pom3 || pom4)
                    {
                        //Console.WriteLine("ispredtwo exit");
                        twoExitEvents(line, pgIndex, index_of_user);
                    }
                    else //Ako je prethodni neki ulaz, mozda dobro
                    {
                        //Console.WriteLine("uso sam 1");
                        //Console.WriteLine("ispred entry then exit");
                        entryThenExit(line, pgIndex, index_of_user);
                    }
                }
            }
        }

        //Nepravilan prvi ulaz danas
        public void invalidFirstEntryToday(string line, int pgIndex, int index_of_user)
        {
            //Console.WriteLine("uso invalid first entry");
            if(whichPG==0) //znaci izaso na normalan
            {
                user[index_of_user].invalidMovement.Add(new pair("First event normal exit"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = getTime(line);
                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;
                user[index_of_user].lastUsedPG = -1;
                user[index_of_user].wentOfficiallyOutside = false;
            }
            else if(whichPG==1) //znaci ulaz ili izlaz na pauzu
            {
                if(line.Contains(PGBreak[pgIndex].enter)) //prvi event izlazi na pauzu
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event went on break "));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].breakMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 1;
                }
                else //Prvi event povratak sa pauze
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event came back from break"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].normalMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 0;
                }
                user[index_of_user].wentOfficiallyOutside = false;
            }
            else if(whichPG==2) //znaci ulaz ili izlaz sluzbeno
            {
                if(user[index_of_user].wentOfficiallyOutside==true && line.Contains(PGofficial[pgIndex].enter)) //izasao sluzbeno a prethodni dan isto izasao
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event went officially outside"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first = time;
                    user[index_of_user].lastUsedPG = 2;
                    user[index_of_user].wentOfficiallyOutside = false;
                }
                else if (user[index_of_user].wentOfficiallyOutside == true && line.Contains(PGofficial[pgIndex].exit)) //prethodnog dana izaso na sluzbeni put, i danas se vraca
                {
                    string time = getTime(line);
                    user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second = time;
                    Console.WriteLine(user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first + " " + user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second);
                    user[index_of_user].wentOfficiallyOutside = false;
                    user[index_of_user].lastUsedPG = 0;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 2] = (user[index_of_user].days[pomi - 2] == -1) ? -1 : 1;

                    workingTime calculatedTime = calculateTime(user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1]);
                    user[index_of_user].officalTime.Add(calculatedTime);

                    Int64 pomss = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                    user[index_of_user].officialSum += pomss;

                    user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;
                    user[index_of_user].normalMovement.Add(new pair(time));
                }
                else if (user[index_of_user].wentOfficiallyOutside == false && line.Contains(PGofficial[pgIndex].enter)) //prvi event sluzbeno izlazi napolje
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event went officially outside"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].officialMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 2;
                }
                else if(user[index_of_user].wentOfficiallyOutside == false && line.Contains(PGofficial[pgIndex].exit))//prvi event vraca se sa sluzbenog
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event came back from official"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].normalMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 0;
                }
                user[index_of_user].wentOfficiallyOutside = false;
            }
            else if(whichPG==3) //znaci ulaz ili izlaz privatno
            {
                if(line.Contains(PGPrivate[pgIndex].enter)) //prvi event privatno izlazi napolje
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event went privately outside"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].privateMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 3;
                }
                else //prvi event privatno se vraca od napolje
                {
                    user[index_of_user].invalidMovement.Add(new pair("First event came back from private"));
                    string time = getTime(line);
                    user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                    List<int> pom = getAllSpaceIndexes(line, '/');
                    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                    int pomi = Convert.ToInt32(poms);
                    user[index_of_user].days[pomi - 1] = -1;

                    user[index_of_user].normalMovement.Add(new pair(time));
                    user[index_of_user].lastUsedPG = 0;
                }
                user[index_of_user].wentOfficiallyOutside = false;
            }
        }

        //2 ulaza za redom
        public void twoEntryEvents(string line, int pgIndex, int index_of_user)
        {
            //Console.WriteLine("uso sam  two entry" + user[index_of_user].name);
            //sada ulazi u zgradu i prethodno je usao
            if (line.Contains(PGNormal[pgIndex].enter))
            {
                user[index_of_user].invalidMovement.Add(new pair("No exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first;

                user[index_of_user].invalidMovement.Add(new pair("2 entries in a row"));
                string time = getTime(line);
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 0;
            }
            //prethodno normalan ulaz a sada ulazi sa pauze
            else if (line.Contains(PGBreak[pgIndex].exit))
            {
                user[index_of_user].invalidMovement.Add(new pair("No exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count-1].second = user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first;

                user[index_of_user].invalidMovement.Add(new pair("No info on going to the break"));
                string time = getTime(line);
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 0;
            }
            else if(line.Contains(PGofficial[pgIndex].exit))
            {
                user[index_of_user].invalidMovement.Add(new pair("No exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first;

                user[index_of_user].invalidMovement.Add(new pair("No info on going to the official"));
                string time = getTime(line);
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 0;
            }
            else if(line.Contains(PGPrivate[pgIndex].exit))
            {
                user[index_of_user].invalidMovement.Add(new pair("No exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first;

                user[index_of_user].invalidMovement.Add(new pair("No info on going to the private"));
                string time = getTime(line);
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 0;
            }
        }

        //ulaz onda izlaz UPITI
        public void exitThenEntry(string line, int pgIndex, int index_of_user)
        {
            Console.WriteLine(pgIndex);
            //podrazumeva se da je lastusedpg 0 tj da je izaso prethodno normalno
            //zato sada ispitujemo gde i kako ulazi
            //sada ulazi a prethodno izaso normalno
            //if (line.Contains(PGofficial[pgIndex].enter) && user[index_of_user].wentOfficiallyOutside == true)
            //{
            //    string time = getTime(line);
            //    user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second = time;
            //    user[index_of_user].wentOfficiallyOutside = false;
            //    user[index_of_user].lastUsedPG = 2;

            //    List<int> pom = getAllSpaceIndexes(line, '/');
            //    string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
            //    int pomi = Convert.ToInt32(poms);
            //    user[index_of_user].days[pomi - 2] = (user[index_of_user].days[pomi - 2] == -1) ? -1 : 1;
            //}
            //else
            if (line.Contains(PGNormal[pgIndex].enter)  && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                string time = getTime(line);
                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = (user[index_of_user].days[pomi - 1] == -1) ? -1 : 1;

            }
            //sada ulazi sa pauze a prethodno izaso normalno
            else if (line.Contains(PGBreak[pgIndex].exit) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                //Console.WriteLine("normalno2" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No exit break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGofficial[pgIndex].exit) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                //Console.WriteLine("normalno3" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No exit official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;


                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGPrivate[pgIndex].exit) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                //Console.WriteLine("uso sam normal pa private " + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No exit private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;


                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGNormal[pgIndex].enter) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);


                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGBreak[pgIndex].exit) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1]);
                user[index_of_user].breakTime.Add(calculatedTime);

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].breakSum += pom;

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGofficial[pgIndex].exit) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No official exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if ( line.Contains(PGPrivate[pgIndex].exit) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No private exit event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso sluzbeno a sada normalno ulazi u zgradu
            else if (line.Contains(PGNormal[pgIndex].enter) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso sluzbeno a vraca se sa pauze
            else if (line.Contains(PGBreak[pgIndex].exit) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No going on break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso sluzbeno i vraca se sluzbeno
            else if (line.Contains(PGofficial[pgIndex].exit) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1]);
                user[index_of_user].officalTime.Add(calculatedTime);

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].officialSum += pom;

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            else if (line.Contains(PGPrivate[pgIndex].exit) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No going out private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso privatno a vraca se normalno
            else if (line.Contains(PGNormal[pgIndex].enter) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                //Console.WriteLine("privatno1" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso privatno a vraca se sa pauze
            else if (line.Contains(PGBreak[pgIndex].exit) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                //Console.WriteLine("privatno2" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No going on break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso privatno a vraca se sa sluzbenog
            else if (line.Contains(PGofficial[pgIndex].exit) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                //Console.WriteLine("privatno3" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                user[index_of_user].invalidMovement.Add(new pair("No going on official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //izaso privatno i vraca se privatno
            else if (line.Contains(PGPrivate[pgIndex].exit) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                //Console.WriteLine("privatno4" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1]);
                user[index_of_user].privateTime.Add(calculatedTime);

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].privateSum += pom;

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                user[index_of_user].normalMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 0;
            }
            //Console.WriteLine("izaso sam "+user[index_of_user].name);
        }

        //2 izlaza za redom
        public void twoExitEvents(string line, int pgIndex, int index_of_user)
        {
            //Console.WriteLine("uso sam  two exit" + user[index_of_user].name);
            //Prethodno normalno izaso i sada opet normalno izlazi
            if (line.Contains(PGNormal[pgIndex].exit) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null )
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No entry event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                //user[index_of_user].normalMovement.RemoveAt(user[index_of_user].normalMovement.Count - 1);
                user[index_of_user].lastUsedPG = 0;
            }
            //prethodno normalno izaso a sada izlazi na pauzu
            else if (line.Contains(PGBreak[pgIndex].enter) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No entry event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].breakMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 1;
            }
            //prethodno normlano izaso a sada izlazi sluzbeno
            else if(line.Contains(PGofficial[pgIndex].enter) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count-1].second!= null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No entry event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].officialMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 2;
            }
            //prethodno normlano izaso a sada izlazi privatno
            else if (line.Contains(PGPrivate[pgIndex].enter) && user[index_of_user].lastUsedPG == 0 && user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second != null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No entry event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = time;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].privateMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 3;
            }
            //prethodno izaso na pauzu, a sada izlazi normalno
            else if(line.Contains(PGNormal[pgIndex].exit) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count-1].second== null)
            {
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].lastUsedPG = 0;
            }
            //prethodno izaso na pauzu a sada izlazi na pauzu
            else if (line.Contains(PGBreak[pgIndex].enter) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null )
            {
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;

                string poms = "";
                int pomi=0;
                List<int> pom = getAllSpaceIndexes(line, '/');
                poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                pomi = Convert.ToInt32(poms);
                
                string time = getTime(line);

                user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 1;
               
                Console.WriteLine("aaaaaaaaaaa");
               
                user[index_of_user].days[pomi - 1] = -1;
                //Int32 pomi = Convert.ToInt32(poms);



            }
            //prethodno izaso na pauzu a sada izlazi sluzbeno
            else if (line.Contains(PGofficial[pgIndex].enter) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null )
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].officialMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 2;
            }
            //prethodno izaso na pauzu a sada izlazi privatno
            else if (line.Contains(PGPrivate[pgIndex].enter) && user[index_of_user].lastUsedPG == 1 && user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from break event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].breakMovement[user[index_of_user].breakMovement.Count - 1].first;
                user[index_of_user].breakMovement.RemoveAt(user[index_of_user].breakMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].privateMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 3;
            }
            //prethondo sluzbeno izaso a sada normalno izlazi
            else if(line.Contains(PGNormal[pgIndex].exit) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count-1].second== null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].lastUsedPG = 0;
            }
            //prethondo sluzbeno izaso a sada izlazi na pauzu
            else if (line.Contains(PGBreak[pgIndex].enter) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].breakMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 1;
            }
            //prethodno sluzbeno izaso a sada izlazi sluzbeno
            else if (line.Contains(PGofficial[pgIndex].enter) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null  )
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 2;
            }
            //prethondo sluzbeno izaso a sada izlazi privatno
            else if (line.Contains(PGPrivate[pgIndex].enter) && user[index_of_user].lastUsedPG == 2 && user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from official event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].officialMovement[user[index_of_user].officialMovement.Count - 1].first;
                user[index_of_user].officialMovement.RemoveAt(user[index_of_user].officialMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].privateMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 3;
            }
            //prethodno izaos privatno a sada izlazi normalno
            else if (line.Contains(PGNormal[pgIndex].exit)&&user[index_of_user].lastUsedPG == 3 &&  user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count-1].second== null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].lastUsedPG = 0;
            }
            //prethodno izaso privatno a sada izlazi na pauzu
            else if (line.Contains(PGBreak[pgIndex].enter) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].breakMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 1;
            }
            //prethodno izaso privatno a sada izlazi sluzbeno
            else if (line.Contains(PGofficial[pgIndex].enter) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;
                user[index_of_user].privateMovement.RemoveAt(user[index_of_user].privateMovement.Count - 1);

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;


                user[index_of_user].officialMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 2;
            }
            //prethodno izaso privatno a sada izlazi privatno
            else if (line.Contains(PGPrivate[pgIndex].enter) && user[index_of_user].lastUsedPG == 3 && user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].second == null)
            {
                string time = getTime(line);
                user[index_of_user].invalidMovement.Add(new pair("No coming back from private event"));
                user[index_of_user].invalidMovement[user[index_of_user].invalidMovement.Count - 1].second = user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first;

                List<int> pom = getAllSpaceIndexes(line, '/');
                string poms = line.Substring(pom[0] + 1, pom[1] - pom[0] - 1);
                int pomi = Convert.ToInt32(poms);
                user[index_of_user].days[pomi - 1] = -1;

                user[index_of_user].privateMovement[user[index_of_user].privateMovement.Count - 1].first = time;
                user[index_of_user].lastUsedPG = 3;
            }
        }

        //izlaz onda ulaz UPITI
        //prethodno uso a sad izlazi UPITI
        public void entryThenExit(string line, int pgIndex, int index_of_user)
        {
            //Console.WriteLine("uso sam  entry then exit"+user[index_of_user].name);
            //prethodno normalno uso a sada normalno izlazi
            if (user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count-1].second==null && line.Contains(PGNormal[pgIndex].exit))
            {
                //Console.WriteLine("uso sam 1  " + line);
                //Console.WriteLine(line);
                string time = getTime(line);
                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1]);
                //Console.WriteLine("izaso sam 1  " + line);
                user[index_of_user].normalTime.Add(calculatedTime);

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].normalSum += pom;

                user[index_of_user].lastUsedPG = 0;
            }
            //prethodno normalno uso a sada izlazi na pauzu
            else if (user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second == null && line.Contains(PGBreak[pgIndex].enter))
            {
                string time = getTime(line);
                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1]);
                user[index_of_user].normalTime.Add(calculatedTime);

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].normalSum += pom;

                user[index_of_user].breakMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 1;
            }
            //prethodno normalno uso a sada izlazi sluzbeno
            else if (user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second == null && line.Contains(PGofficial[pgIndex].enter))
            {
                string time = getTime(line);
                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1]);
                user[index_of_user].normalTime.Add(calculatedTime);

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;

                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].normalSum += pom;

                user[index_of_user].officialMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 2;
            }
            //prethodno normalno uso a sada izlazi privatno
            else if (user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second == null && line.Contains(PGPrivate[pgIndex].enter))
            {
               // Console.WriteLine("uso sam normal private" + user[index_of_user].name);
                string time = getTime(line);
                user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1].second = time;

                workingTime calculatedTime = calculateTime(user[index_of_user].normalMovement[user[index_of_user].normalMovement.Count - 1]);
                user[index_of_user].normalTime.Add(calculatedTime);

                user[index_of_user].days[calculatedTime.day - 1] = (user[index_of_user].days[calculatedTime.day - 1] == -1) ? -1 : 1;


                Int64 pom = calculatedTime.time.h * 3600 + calculatedTime.time.min * 60 + calculatedTime.time.s;
                user[index_of_user].normalSum += pom;

                user[index_of_user].privateMovement.Add(new pair(time));
                user[index_of_user].lastUsedPG = 3;
            }
        }


        //public class TimeView
        //{
        //    public int h=0, min=0, s=0;

        //    public TimeView()
        //    {

        //    }

        //    public TimeView(int cas, int minut, int sekund)
        //    {
        //        h = cas;
        //        min = minut;
        //        s = sekund;
        //    }
        //}
        private workingTime calculateTime(pair a) //funkcija koja racuna vreme
        {
            TimeView pom = new TimeView();
            List<int> idx = getAllSpaceIndexes(a.first, ' ');
            List<int> idx1 = getAllSpaceIndexes(a.first, ':');
            int h1 = Convert.ToInt32(a.first.Substring(idx[0], idx1[0] - idx[0]));
            int m1 = Convert.ToInt32(a.first.Substring(idx1[0] + 1, idx1[1] - idx1[0] - 1));
            int s1 = Convert.ToInt32(a.first.Substring(idx1[1] + 1, idx[1] - idx1[1] - 1));
            if (a.first.Contains("PM") && h1 != 12)
                h1 += 12;

            idx = getAllSpaceIndexes(a.second, ' ');
            idx1 = getAllSpaceIndexes(a.second, ':');
            int h2 = Convert.ToInt32(a.second.Substring(idx[0], idx1[0] - idx[0]));
            int m2 = Convert.ToInt32(a.second.Substring(idx1[0] + 1, idx1[1] - idx1[0] - 1));
            int s2 = Convert.ToInt32(a.second.Substring(idx1[1] + 1, idx[1] - idx1[1] - 1));
            if (a.second.Contains("PM") && h2 != 12)
                h2 += 12;


            List<int> idxPom = getAllSpaceIndexes(a.first, '/');
            List<int> idxPom2 = getAllSpaceIndexes(a.second, '/');
            int pom1 = Convert.ToInt32(a.first.Substring(idxPom[0] + 1, idxPom[1] - idxPom[0] - 1));
            int pom2 = Convert.ToInt32(a.second.Substring(idxPom2[0] + 1, idxPom2[1] - idxPom2[0] - 1));
            Console.WriteLine(pom1 + " " + pom2);
            int pp = pom2 - pom1;
            int u1, u2, u;
            if (pp > 0)
            {
                pp--;
                h2 += pp * 24;
                u1 = h1 * 3600 + m1 * 60 + s1;
                u1 = 86400 - u1;
                u2 = h2 * 3600 + m2 * 60 + s2;
                u = u2 + u1;
            }
            else
            {
                u1 = h1 * 3600 + m1 * 60 + s1;
                u2 = h2 * 3600 + m2 * 60 + s2;

                u = u2 - u1;
            }

            pom.h = u / 3600;
            pom.min = (u - pom.h * 3600) / 60;
            pom.s = u - pom.h * 3600 - pom.min * 60;

            //Console.WriteLine(a.first + " " + a.second);
            //Console.WriteLine(pom.h + " " + pom.min + " " + pom.s);

            List<int> idx2 = getAllSpaceIndexes(a.first, '/');
            string poms = a.first.Substring(idx2[0]+1, idx2[1] - idx2[0]-1);
            //Console.WriteLine(poms);
            int ddd = Convert.ToInt32(poms);
            return new workingTime(ddd, pom);
        }

        public List<int> getAllSpaceIndexes(string line, char what)
        {
            List<int> pom = new List<int>();
            int brc = 0;
            foreach (char a in line)
            {
                if (a == what)
                    pom.Add(brc);
                brc++;
            }
            return pom;
        }
        private int exitedOnAnybreakPG(string line)
        {
            int brc = 0;
            foreach(var a in PGBreak)
            {
                if (line.Contains(a.exit))
                    return brc;
                brc++;
            }
            return -1;
        }
        private int exitedOnAnyofficialPG(string line)
        {
            int brc = 0;
            foreach (var a in PGofficial)
            {
                if (line.Contains(a.exit))
                    return brc;
                brc++;
            }
            return -1;
        }

        private int exitedOnAnyPrivatePG(string line)
        {
            int brc = 0;
            foreach (var a in PGPrivate)
            {
                if (line.Contains(a.exit))
                    return brc;
                brc++;
            }
            return -1;
        }

        private int exitedOnAnyNormalPG(string line)
        {
            int brc = 0;
            foreach (var a in PGNormal)
            {
                if (line.Contains(a.exit))
                    return brc;
                brc++;
            }
            return -1;
        }

        private void normalMovementProcess()
        {

        }
        private List<int> getAllIndexes(string line)
        {
            List<int> idx = new List<int>();
            for (int i = 0; i < line.Length; i++)
                if (line[i] == '|') idx.Add(i);
            return idx;
        }
        public string getTime(string line)
        {
            List<int> idx = getAllIndexes(line);
            return line.Substring(idx[1]+1, idx[2] - idx[1]-1);
        }

        private int userExist(string id)
        {
            for(int i=0;i<user.Count;i++)
                if (user[i].id == id)
                    return i;
            return -1;
        }

        public string getId(int index, string line)
        {
            string temp = "";
            index+=5;
            while (Char.IsDigit(line[index]))
            {
                temp += line[index];
                index++;
            }
            return temp;
        }

        public int isUserLine(string line)
        {
            if (line!=null && line!="" && line.Contains("User"))
                return line.IndexOf("User");
            return -1;
        }

        private int isPGLine(string line)
        {
            for(int i=0;i<PGNormal.Count<triPair>();i++)
            {
                if (line.Contains(PGNormal[i].value))
                {
                    whichPG = 0;
                    return i;
                }
            }

            for (int i = 0; i < PGBreak.Count<triPair>(); i++)
            {
                if (line.Contains(PGBreak[i].value))
                {
                    whichPG = 1;
                    return i;
                }
            }

            for (int i = 0; i < PGofficial.Count<triPair>(); i++)
            {
                if (line.Contains(PGofficial[i].value))
                {
                    whichPG = 2;
                    return i;
                }
            }

            for (int i = 0; i < PGPrivate.Count<triPair>(); i++)
            {
                if (line.Contains(PGPrivate[i].value))
                {
                    whichPG = 3;
                    return i;
                }
            }
            return -1;
        }
    }
}

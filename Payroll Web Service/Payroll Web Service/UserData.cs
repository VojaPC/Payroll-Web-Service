using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using static Baza_Visual_WPF.LineProcessing;

namespace Baza_Visual_WPF
{
    [DataContract]
    public class TimeView
    {
        [DataMember]
        public int h = 0, min = 0, s = 0;

        public TimeView()
        {

        }

        public TimeView(int cas, int minut, int sekund)
        {
            h = cas;
            min = minut;
            s = sekund;
        }
    }

    [DataContract]
    public class pair
    {
        [DataMember]
        public string first=null, second=null;
        public pair(string f)
        {
            first = f;
        }
    }

    [DataContract]
    public class workingTime
    {
        [DataMember]
        public int day;
        [DataMember]
        public TimeView time = new TimeView();

        public workingTime()
        {

        }

        public workingTime(int d, TimeView t)
        {
            day = d;
            time = t;
        }
    }

    [DataContract]
    public class UserData
    {
        [DataMember]
        public string id;
        [DataMember]
        public string name;

        [DataMember]
        public int[] days = Enumerable.Repeat(0, 31).ToArray(); //0 prazno  - 1 postoji  - -1 nepravilno
        [DataMember]
        public List<pair> normalMovement = new List<pair>();
        [DataMember]
        public List<pair> invalidMovement = new List<pair>();
        [DataMember]
        public List<pair> breakMovement = new List<pair>();
        [DataMember]
        public List<pair> officialMovement = new List<pair>();
        [DataMember]
        public List<pair> privateMovement = new List<pair>();

        [DataMember]
        public List<workingTime> normalTime = new List<workingTime>();
        [DataMember]
        public List<workingTime> breakTime = new List<workingTime>();
        [DataMember]
        public List<workingTime> officalTime = new List<workingTime>();
        [DataMember]
        public List<workingTime> privateTime = new List<workingTime>();

        [DataMember]
        public Int64 normalSum = 0;
        [DataMember]
        public Int64 breakSum = 0;
        [DataMember]
        public Int64 officialSum = 0;
        [DataMember]
        public Int64 privateSum = 0;

        [DataMember]
        public string month = "";

        [DataMember]
        public bool wentOfficiallyOutside = false; //flag ako izadje dns sluzbeno i vrati se sutra sluzbeno

        public int lastUsedPG = -1; //poslednji koriscen PG

        public UserData()
        {
        }

        public UserData(string i)
        {
            id = i;
        }
    }
}

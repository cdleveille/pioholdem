using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class PreflopLookups
    {
        public string[] BUopen_100 = new string[]
        {"AA", "KK", "QQ", "JJ", "TT", "99", "88", "77", "66", "55", "44", "33", "22", "AKo", "AKs", "AQo", "AQs", "AJo", "AJs", "ATo",
        "ATs", "A9o", "A9s", "A8o", "A8s", "A7o", "A7s", "A6o", "A6s", "A5o", "A5s", "A4o", "A4s", "A3o", "A3s", "A2o", "A2s", "KQo",
        "KQs", "KJo", "KJs", "KTo", "KTs", "K9o", "K9s", "K8o", "K8s", "K7o", "K7s", "K6o", "K6s", "K5o", "K5s", "K4o", "K4s", "K3o",
        "K3s", "K2o", "K2s", "QJo", "QJs", "QTo", "QTs", "Q9o", "Q9s", "Q8o", "Q8s", "Q7o", "Q7s", "Q6o", "Q6s", "Q5o", "Q5s", "Q4o",
        "Q4s", "Q3o", "Q3s", "Q2o", "Q2s", "JTo", "JTs", "J9o", "J9s", "J8o", "J8s", "J7o", "J7s", "J6o", "J6s", "J5o", "J5s", "J4o",
        "J4s", "J3o", "J3s", "J2s", "T9o", "T9s", "T8o", "T8s", "T7o", "T7s", "T6o", "T6s", "T5o", "T5s", "T4s", "T3s", "T2s", "98o",
        "98s", "97o", "97s", "96o", "96s", "95s", "94s", "93s", "92s", "87o", "87s", "86o", "86s", "85o", "85s", "84s", "83s", "82s",
        "76o", "76s", "75o", "75s", "74s", "73s", "72s", "65o", "65s", "64o", "64s", "63s", "62s", "54o", "54s", "53s", "52s", "43s",
        "42s", "32s"};

        public string[] BBcallOpen_100 = new string[]
        {"77", "66", "55", "44", "33", "22", "A9o", "A8o", "A8s", "A7o", "A7s", "A6o", "A6s", "A5o", "A4o", "A3o", "A2o", "KJo", "KTo",
        "K9o", "K9s", "K8o", "K8s", "K7o", "K7s", "K6o", "K6s", "K5o", "K5s", "K4o", "K3o", "K2o", "K2s", "QJo", "QTo", "Q9o", "Q9s",
        "Q8o", "Q8s", "Q7o", "Q7s", "Q6o", "Q6s", "Q5o", "Q5s", "Q4s", "Q3s", "Q2s", "JTo", "J9o", "J8o", "J7o", "J6o", "J5s", "J4s",
        "J3s", "J2s", "T9o", "T8o", "T7o", "T6o", "T5s", "T4s", "T3s", "T2s", "98o", "97o", "96o", "95s", "94s", "93s", "92s", "87o",
        "86o", "84s", "83s", "82s", "76o", "75o", "73s", "72s", "65o", "62s", "54o", "52s", "42s"};

        public string[] BB3bet_100 = new string[]
        {"AA", "KK", "QQ", "JJ", "TT", "99", "88", "AKo", "AKs", "AQo", "AQs", "AJo", "AJs", "ATo", "ATs", "A9s", "A5s", "A4s", "A3s",
         "A2s", "KQo", "KQs", "KJs", "KTs", "K4s", "K3s", "QJs", "QTs", "JTs", "J9s", "J8s", "J7s", "J6s", "T9s", "T8s", "T7s", "T6s",
         "98s", "97s", "96s", "87s", "86s", "85s", "76s", "75s", "74s", "65s", "64s", "63s", "54s", "53s", "43s", "32s"};

        public string[] BUcall3bet_100 = new string[]
        {"99", "88", "77", "66", "55", "44", "33", "22", "AQo", "AJo", "AJs", "ATo", "ATs", "A9o", "A9s", "A8o", "A8s", "A7s", "A6s",
        "A5s", "A4s", "A3s", "A2s", "KQo", "KQs", "KJo", "KJs", "KTo", "KTs", "K9o", "K9s", "K8s", "K7s", "K6s", "K5s", "K4s", "K3s",
        "K2s", "QJo", "QJs", "QTo", "QTs", "Q9o", "Q9s", "Q8s", "Q7s", "Q6s", "Q5s", "Q4s", "Q3s", "Q2s", "JTo", "JTs", "J9o", "J9s",
        "J8s", "J7s", "J6s", "T9o", "T9s", "T8s", "T7s", "T6s", "98s", "97s", "96s", "87s", "86s", "85s", "76s", "75s", "74s", "65s",
        "64s", "63s", "54s", "53s", "43s"};

        public string[] BU4bet_100 = new string[]
        {"AA", "KK", "QQ", "JJ", "TT", "AKo", "AKs", "AQs", "J5s", "J4s", "J3s", "J2s", "T5s", "95s", "84s", "73s"};

        public string[] BB5betShove_100 = new string[]
        {"AA", "KK", "QQ", "JJ", "AKo", "AKs", "A5s", "A4s", "A3s", "A2s", "K4s", "K3s", "96s", "86s", "85s", "76s", "75s", "74s", "65s",
        "64s", "54s"};

        public string[] BUcallShove_100 = new string[]
        {"AA", "KK", "AKo", "AKs", "QQ"};

        public Dictionary<string, double> pushFold_BUshove = new Dictionary<string, double>();

        public Dictionary<string, double> pushFold_BBcall = new Dictionary<string, double>();
    }
}

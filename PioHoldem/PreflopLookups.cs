using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class PreflopLookups
    {
        // Effective stack: 100BB
        public string[] BUopen_100_call;
        public string[] BUopen_100_raise;
        public string[] BBvsBUlimp_100_raise;
        public string[] BBvsBUopen_100_call;
        public string[] BBvsBUopen_100_raise;
        public string[] BUvsBBraiseAfterLimping_100_call;
        public string[] BUvsBBraiseAfterLimping_100_raise;
        public string[] BUvsBBraiseAfterOpening_100_call;
        public string[] BUvsBBraiseAfterOpening_100_raise;
        public string[] BBvsBB4bet_100_call;
        public string[] BBvsBB4bet_100_raise;
    }
}

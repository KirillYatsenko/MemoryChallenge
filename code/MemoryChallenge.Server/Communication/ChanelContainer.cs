using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryChallenge.Common
{
    public class ChanelsContainer
    {
        private Dictionary<int, Chanel> chanels = new Dictionary<int, Chanel>();
        private int count = 0;

        public int Add(Chanel chanel)
        {
            int id = count;

            chanels.Add(count, chanel);
            count++;

            return id;
        }

        public Chanel GetById(int id)
        {
            return chanels[id];
        }

    }
}

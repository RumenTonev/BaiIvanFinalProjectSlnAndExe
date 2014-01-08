using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiIvanProGAMENirane
{
    class Cell
    {
        public bool isVisited;//when recursion not to check a cell twice
        public bool isFree; //is free to place the new figure

        public Cell()
        {
            isVisited = false;
            isFree = true;
        }
    }
}

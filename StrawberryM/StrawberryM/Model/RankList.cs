using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace StrawberryM.Model
{
    public class RankList
    {
        public int rank { get; set; }
        public string singer { get; set; }
        public string song { get; set; }
        public ICommand SearchCommand { get; set; }
    }
}

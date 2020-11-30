using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamagotchi.Exceptions
{
    class DeathException : Exception
    {
        public DeathException(string text) : base(text) { }
    }
}

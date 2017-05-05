using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfToClass
{
    class Program
    {
        static void Main(string[] args)
        {
            var converter = new Converter();
            converter.convert("pics/1.png");
        }
    }
}

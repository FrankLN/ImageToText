using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pdfToClass
{
    public class Converter
    {
        public List<Item> convert(string imageURL)
        {
            var result = new List<Item>();

            //Image image = Image.FromFile(imageURL);
            var text = fromImgToString(imageURL);
            //var text = "I. Margeritter Tomat, ost, oregano. 35,- 2. Elika pizza Tomat, ost, skinke, bacon, oregano. 50,- 3. Bella corsa Tomat, ost, oksekød, peperoni, salat, dressing oregano. 50,- 4. Vesuvio Tomat, ost, skinke, oregano 45,- 5. Capriccioso Tomat, ost, skinke, champignon, oregano. 45,- 6. Salvotora Tomat, ost, løg, ananas, oksekød, oregano. 50,- 7. Mexico i Tomat, ost,løg, ol peperoni, chili, oregano 50,- 8. Vegertariane Tomat, ost, løg, ananas, majs, champignon, oregano. 45,- 9. Fiorantina Tomat, ost, oksekød, bacon, oregano... 10. Qatro staggione Tomat, ost, skinke, champignon, rejer, o  50,- 11. Pizza kebab < /  , Tomat, ost, kebab, rod peber, oregano   sor 12. Pizza kebab 2 Tomat, ost,kebab, løg, salat, dressing oregano   55'-13. Pias bolognese Tomat, ost, kødsovs, oregano. 49,- 14. Ufo Tomat, ost, skinke, oksekød, peperoni, oregano 50,- 15. Bebeto Tomat, ost, skinke, peperoni,bacon, chili, hvidløg, oregano. 50,- 16. Calzone (indbagt) Tomat, ost,skinke,champignon, oregano. 50,- 17. Pereferita Tomat, ost, kylling, champignon, oregano. 50,- 18. Triesta Tomat, ost, kylling, rejer, tun, oregano 55,-";

            Console.WriteLine("MyResult: " + text);

            List<string> splitted = split(text);

            Console.WriteLine();
            foreach (var item in splitted)
            {
                Console.WriteLine(item);
            }

            return result;
        }

        public string fromImgToString(string imageURL)
        {
            string license_code = "E9B5C06B-AD65-4CA3-8EC6-C7D348D677D3";
            string user_name = "FRANKL";

            string result = RestOCR.ProcessDocument(user_name, license_code, imageURL);

            return result;
        }

        public List<string> split(string text)
        {
            List<string> temp = new List<string>();
            List<string> result = new List<string>();

            temp = text.Split('-').ToList();

            for (int i = 0; i < temp.Count(); i++)
            {
                if(temp[i].Count() > 0 && temp[i].First() == ' ')
                {
                    result.Add(temp[i].Substring(1));
                }
                else if(temp[i].Count() > 0)
                {
                    result.Add(temp[i]);
                }
            }

            return result;
        }
    }
}

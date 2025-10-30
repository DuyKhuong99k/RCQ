using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
namespace PrintIt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var filePath = args[0];
                var excelApp = new Application(); 
                var _workbook = excelApp.Workbooks.Open(filePath);
                _workbook.PrintOut();
                _workbook.Close(false);
                excelApp.Quit();
#if DEBUG
                Console.ReadKey();      
#endif
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                //throw;
            }
        }
    }
}

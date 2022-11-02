//Mario Valdez Rico
using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //instancia();
                //GC.Collect();
                
                using (Lenguaje a = new Lenguaje())
                {
                    
                    a.Programa();
                }
                
                
                /* while(!a.FinArchivo())
                 {
                     a.NextToken();
                 }*/
                //a.cerrar();
               // a.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /*static void instancia()
        {
            Lenguaje a = new Lenguaje();
            a.Programa();
        }*/
    }
}
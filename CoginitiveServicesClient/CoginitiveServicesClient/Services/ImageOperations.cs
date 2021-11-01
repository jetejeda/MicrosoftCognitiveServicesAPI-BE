using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoginitiveServicesClient.Services
{
    public static class ImageOperations
    {
        public static string GetKinship(double cognitiveServicesResult)
        {
            if (cognitiveServicesResult <= 20)
            {
                return "ninguno";
            }
            else if (cognitiveServicesResult <= 40)
            {
                return "primos lejanos";
            }
            else if (cognitiveServicesResult <= 60)
            {
                return "primos o tíos";
            }
            else if (cognitiveServicesResult <= 80)
            {
                return "hermanos";
            }
            else if (cognitiveServicesResult <= 90)
            {
                return "papá - mamá / hijo";
            }
            else
            {
                return "la misma persona";
            }
        }
    }
}

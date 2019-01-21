using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LabGoogleMap.Helpers
{
    public static class EnumHelper
    {


        public static string GetEnumDescription<T>(T value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description;
        }


        public static Dictionary<int, string> GetEnumDataSource<T>(T var)
        {
            var enumDatasource = new Dictionary<int, string>();
           var enumList = Enum.GetValues(typeof(T));

            foreach (var item in enumList)
            {
                var enumDescription = GetEnumDescription((T) item);
                enumDatasource.Add((int)item, enumDescription);
            }

            // typeof(Service.RequestModels.TrafficModel)
            return enumDatasource;
        }





    }
}

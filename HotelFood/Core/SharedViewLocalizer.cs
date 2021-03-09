using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace HotelFood.Core
{
    public class SharedViewLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public SharedViewLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResources", assemblyName.Name);
        }

        public LocalizedString this[string key] => _localizer[key];

        public LocalizedString GetLocalizedString(string key)
        {
            return _localizer[key];
        }
    }
}

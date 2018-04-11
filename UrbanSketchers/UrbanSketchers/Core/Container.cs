using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace UrbanSketchers.Core
{
    public static class Container
    {
        public static IContainer Current { get; internal set; }
    }
}

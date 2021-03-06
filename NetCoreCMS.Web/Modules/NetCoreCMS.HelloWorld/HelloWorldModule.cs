﻿using Microsoft.Extensions.DependencyInjection;
using NetCoreCMS.Framework.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreCMS.Modules.HelloWorld
{
    public class HelloWorldModule : IModule
    {
        public HelloWorldModule()
        {
            
        }

        public string Id { get; set; }
        public string ModuleName { get; set; }
        public string SortName { get; set; }
        public bool AntiForgery { get; set ; }
        public string Author { get ; set ; }
        public string Website { get ; set ; }
        public Version Version { get ; set ; }
        public Version NetCoreCMSVersion { get ; set ; }
        public string Description { get ; set ; }

        public List<string> Dependencies { get ; set ; }
        public string Category { get ; set ; }

        [NotMapped]
        public Assembly Assembly { get; set; }
        public string Path { get ; set ; }

        public void Init(IServiceCollection services)
        {
            
        }
    }
}

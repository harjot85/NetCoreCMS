﻿using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace NetCoreCMS.Framework.Modules
{
    public class ModuleManager
    {
        List<IModule> modules = new List<IModule>();
        public List<IModule> LoadModules(IDirectoryContents moduleRootFolder)
        {   
            foreach (var moduleFolder in moduleRootFolder.Where(x => x.IsDirectory))
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.PhysicalPath, "bin"));
                if (!binFolder.Exists)
                {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException ex)
                    { 
                        continue;
                    }
                    
                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        modules.Add(new Module { ModuleName = moduleFolder.Name, Assembly = assembly, Path = moduleFolder.PhysicalPath });
                    }
                }
            }
            return modules;
        }
        
        public void RegisterModules(IMvcBuilder mvcBuilder, IServiceCollection services)
        {
            foreach (var module in modules)
            {
                // Register controller from modules
                mvcBuilder.AddApplicationPart(module.Assembly);

                // Register dependency in modules
                var moduleInitializerType = module.Assembly.GetTypes().Where(x => typeof(IModule).IsAssignableFrom(x)).FirstOrDefault();
                if (moduleInitializerType != null && moduleInitializerType != typeof(IModule))
                {
                    var moduleInitializer = (IModule) Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }
            }

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ModuleViewLocationExpendar());
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace VirtualPet.Core
{
    public class PluginLoader
    {
        public List<IPlugin> LoadedPlugins { get; set; }
        public PluginLoader()
        {
            LoadedPlugins = new List<IPlugin>();
        }

        public IPlugin LoadPlugin(string path, IPetInteractor interactor)
        {
            IPlugin _return = null;
            Type appType = typeof(IPlugin);
            byte[] AppContent;
            using (FileStream app = File.OpenRead(path))
            {
                AppContent = new byte[app.Length];
                app.Read(AppContent, 0, (int)app.Length);
            }
            PluginLoadContext loadContext = new PluginLoadContext(path);
             Assembly a = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
           // Assembly a = AppDomain.CurrentDomain.Load(AppContent);
            if (a != null)
            {
                Type[] types = a.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsInterface || t.IsAbstract)
                        continue;
                    else
                    {
                        if (t.GetInterface(appType.FullName) != null)
                        {
                            IPlugin app = (IPlugin)Activator.CreateInstance(t);
                            _return = app;
                        }
                    }
                }
            }
            if(_return != null)
            {
                _return.SetInteractor(interactor);
                LoadedPlugins.Add(_return);
            }
            return _return;
        }
    }
}

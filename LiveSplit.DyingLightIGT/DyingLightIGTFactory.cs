using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;

namespace LiveSplit.DyingLightIGT
{
    public class DyingLightIGTFactory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Dying Light"; }
        }

        public string Description
        {
            get { return "Dying Light Autosplitter"; }
        }

        public ComponentCategory Category
        {
            get { return ComponentCategory.Control; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new DyingLightIGTComponent(state);
        }

        public string UpdateName
        {
            get { return this.ComponentName; }
        }

        public string UpdateURL
        {
            get { return "https://raw.githubusercontent.com/Dalet/DyingLightIGT/master/"; }
        }

        public Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public string XMLURL
        {
            get { return this.UpdateURL + "Components/update.LiveSplit.DyingLightIGT.xml"; }
        }
    }
}

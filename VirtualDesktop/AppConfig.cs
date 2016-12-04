using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace VirtualDesktop
{
   

    public class AppConfigSection : ConfigurationSection    // 所有配置节点都要选择这个基类
    {
        private static readonly ConfigurationProperty s_property
            = new ConfigurationProperty(string.Empty, typeof(AppKeyValueCollection), null,
                                            ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public AppKeyValueCollection KeyValues
        {
            get
            {
                return (AppKeyValueCollection)base[s_property];
            }
        }

    }


    [ConfigurationCollection(typeof(AppKeyValueSetting))]
    public class AppKeyValueCollection : ConfigurationElementCollection        // 自定义一个集合
    {
        // 基本上，所有的方法都只要简单地调用基类的实现就可以了。

        public AppKeyValueCollection()
            : base(StringComparer.OrdinalIgnoreCase)    // 忽略大小写
        {
        }

        // 其实关键就是这个索引器。但它也是调用基类的实现，只是做下类型转就行了。
        new public AppKeyValueSetting this[string name]
        {
            get
            {
                return (AppKeyValueSetting)base.BaseGet(name);
            }
        }

        // 下面二个方法中抽象类中必须要实现的。
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppKeyValueSetting();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppKeyValueSetting)element).Name;
        }

        // 说明：如果不需要在代码中修改集合，可以不实现Add, Clear, Remove
        public void Add(AppKeyValueSetting setting)
        {
            this.BaseAdd(setting);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class AppKeyValueSetting : ConfigurationElement    // 集合中的每个元素
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return this["path"].ToString(); }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("position", IsRequired = true)]
        public string Position
        {
            get { return this["position"].ToString(); }
            set { this["position"] = value; }
        }

    }
    

}

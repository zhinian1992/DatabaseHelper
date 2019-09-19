using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DatabaseHelper
{
    public class BinderConfig:ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public BusinessCollection BinderSettings
        {
            get
            {
                return (BusinessCollection)base[""];
            }
        }
    }

    public class BusinessCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BusinessElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BusinessElement)element).type;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "business";
            }
        }

        public BusinessElement this[int index]
        {
            get
            {
                return (BusinessElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }

    public class BusinessElement:ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]

        public string type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("conn", IsRequired = true)]

        public string conn
        {
            get { return (string)this["conn"]; }
            set { this["conn"] = value; }
        }

        [ConfigurationProperty("sql_select_for_grid1", IsRequired = true)]

        public string SSFG1
        {
            get { return (string)this["sql_select_for_grid1"]; }
            set { this["sql_select_for_grid1"] = value; }
        }

        [ConfigurationProperty("sql_select_for_grid2", IsRequired = true)]

        public string SSFG2
        {
            get { return (string)this["sql_select_for_grid2"]; }
            set { this["sql_select_for_grid2"] = value; }
        }

        [ConfigurationProperty("sql_select_for_combobox", IsRequired = true)]

        public string SSFC
        {
            get { return (string)this["sql_select_for_combobox"]; }
            set { this["sql_select_for_combobox"] = value; }
        }

        [ConfigurationProperty("sql_update_single", IsRequired = true)]

        public string SUS
        {
            get { return (string)this["sql_update_single"]; }
            set { this["sql_update_single"] = value; }
        }

        [ConfigurationProperty("sql_delete_record", IsRequired = true)]

        public string SDR
        {
            get { return (string)this["sql_delete_record"]; }
            set { this["sql_delete_record"] = value; }
        }

        [ConfigurationProperty("sql_insert_record", IsRequired = true)]

        public string SIR
        {
            get { return (string)this["sql_insert_record"]; }
            set { this["sql_insert_record"] = value; }
        }
    }
}

using System;
using System.ComponentModel;
using System.Configuration;

namespace LibSettings
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class HexSettings : DialogPage
    {
        public string ID { get; set; }

        public HexSettings()
        {
        }
    }
}

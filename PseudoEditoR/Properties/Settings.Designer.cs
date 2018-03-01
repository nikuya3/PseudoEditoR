namespace PseudoEditoR.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Deutsch")]
        public string LastUsedLocalizationLanguage {
            get {
                return ((string)(this["LastUsedLocalizationLanguage"]));
            }
            set {
                this["LastUsedLocalizationLanguage"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Kvic")]
        public string LastUsedProgrammingLanguage {
            get {
                return ((string)(this["LastUsedProgrammingLanguage"]));
            }
            set {
                this["LastUsedProgrammingLanguage"] = value;
            }
        }
    }
}

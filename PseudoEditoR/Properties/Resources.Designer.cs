﻿
namespace PseudoEditoR.Properties {
    using System;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public Resources() {
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PseudoEditoR.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static byte[] Deutsch {
            get {
                object obj = ResourceManager.GetObject("Deutsch", resourceCulture);
                return ((byte[])(obj));
            }
        }

        public static byte[] English {
            get {
                object obj = ResourceManager.GetObject("English", resourceCulture);
                return ((byte[])(obj));
            }
        }

        public static byte[] Kvic {
            get {
                object obj = ResourceManager.GetObject("Kvic", resourceCulture);
                return ((byte[])(obj));
            }
        }

        public static byte[] PseudoCode {
            get {
                object obj = ResourceManager.GetObject("PseudoCode", resourceCulture);
                return ((byte[])(obj));
            }
        }

        public static byte[] Rainbow {
            get {
                object obj = ResourceManager.GetObject("Rainbow", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HttPlaceholder.Application {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ApplicationResources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApplicationResources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("HttPlaceholder.Application.ApplicationResources", typeof(ApplicationResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string ConfigValueNotFound {
            get {
                return ResourceManager.GetString("ConfigValueNotFound", resourceCulture);
            }
        }
        
        internal static string ConfigValueCantBeMutated {
            get {
                return ResourceManager.GetString("ConfigValueCantBeMutated", resourceCulture);
            }
        }
        
        internal static string ConfigValueIncorrectBoolean {
            get {
                return ResourceManager.GetString("ConfigValueIncorrectBoolean", resourceCulture);
            }
        }
        
        internal static string ConfigProviderNotFound {
            get {
                return ResourceManager.GetString("ConfigProviderNotFound", resourceCulture);
            }
        }
        
        internal static string ConflictDetected {
            get {
                return ResourceManager.GetString("ConflictDetected", resourceCulture);
            }
        }
        
        internal static string EntityNotFound {
            get {
                return ResourceManager.GetString("EntityNotFound", resourceCulture);
            }
        }
        
        internal static string ValidationFailed {
            get {
                return ResourceManager.GetString("ValidationFailed", resourceCulture);
            }
        }
        
        internal static string RequestConvertionNotSupported {
            get {
                return ResourceManager.GetString("RequestConvertionNotSupported", resourceCulture);
            }
        }
        
        internal static string FeatureFlagNotSupported {
            get {
                return ResourceManager.GetString("FeatureFlagNotSupported", resourceCulture);
            }
        }
        
        internal static string ScenarioNotFound {
            get {
                return ResourceManager.GetString("ScenarioNotFound", resourceCulture);
            }
        }
        
        internal static string HostedJobNotFound {
            get {
                return ResourceManager.GetString("HostedJobNotFound", resourceCulture);
            }
        }
    }
}

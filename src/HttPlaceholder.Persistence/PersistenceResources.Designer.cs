﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HttPlaceholder.Persistence {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PersistenceResources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PersistenceResources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("HttPlaceholder.Persistence.PersistenceResources", typeof(PersistenceResources).Assembly);
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
        
        internal static string NoDataSourceOrConnection {
            get {
                return ResourceManager.GetString("NoDataSourceOrConnection", resourceCulture);
            }
        }
        
        internal static string MysqlStringNotFound {
            get {
                return ResourceManager.GetString("MysqlStringNotFound", resourceCulture);
            }
        }
        
        internal static string PostgresStringNotFound {
            get {
                return ResourceManager.GetString("PostgresStringNotFound", resourceCulture);
            }
        }
        
        internal static string MigratorCouldntFindFile {
            get {
                return ResourceManager.GetString("MigratorCouldntFindFile", resourceCulture);
            }
        }
        
        internal static string MigratorCouldntDetermineFolder {
            get {
                return ResourceManager.GetString("MigratorCouldntDetermineFolder", resourceCulture);
            }
        }
        
        internal static string StubTypeNotSupported {
            get {
                return ResourceManager.GetString("StubTypeNotSupported", resourceCulture);
            }
        }
        
        internal static string SqliteStringNotFound {
            get {
                return ResourceManager.GetString("SqliteStringNotFound", resourceCulture);
            }
        }
        
        internal static string SqlserverStringNotFound {
            get {
                return ResourceManager.GetString("SqlserverStringNotFound", resourceCulture);
            }
        }
        
        internal static string FileStorageUnexpectedlyNull {
            get {
                return ResourceManager.GetString("FileStorageUnexpectedlyNull", resourceCulture);
            }
        }
        
        internal static string ScenarioWithKeyAlreadyExists {
            get {
                return ResourceManager.GetString("ScenarioWithKeyAlreadyExists", resourceCulture);
            }
        }
        
        internal static string ScenarioWithKeyNotFound {
            get {
                return ResourceManager.GetString("ScenarioWithKeyNotFound", resourceCulture);
            }
        }
        
        internal static string ScenarioStateNotUpdated {
            get {
                return ResourceManager.GetString("ScenarioStateNotUpdated", resourceCulture);
            }
        }
    }
}

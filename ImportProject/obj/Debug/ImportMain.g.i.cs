﻿#pragma checksum "..\..\ImportMain.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "94CD46DD79620F051B78FE7354BB3B8F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ImportProject;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ImportProject {
    
    
    /// <summary>
    /// ImportMain
    /// </summary>
    public partial class ImportMain : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 200 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label FolderAddressLabel;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label usernameLabel;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lastImportDateLabel;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label noTransactionsLabel;
        
        #line default
        #line hidden
        
        
        #line 208 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label urgencyLabel;
        
        #line default
        #line hidden
        
        
        #line 209 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button FileBrowser;
        
        #line default
        #line hidden
        
        
        #line 212 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox descriptionComboBox;
        
        #line default
        #line hidden
        
        
        #line 215 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton alwaysAskCB;
        
        #line default
        #line hidden
        
        
        #line 216 "..\..\ImportMain.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton neverAskCB;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ImportProject;component/importmain.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ImportMain.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.FolderAddressLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.usernameLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lastImportDateLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.noTransactionsLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.urgencyLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.FileBrowser = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.descriptionComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.alwaysAskCB = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 9:
            this.neverAskCB = ((System.Windows.Controls.RadioButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\..\TableList\HomeTableView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "447AE2B151254465B60515CBA469EDCC936F164C"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using EVMESCharts.TableList;
using LiveCharts.Wpf;
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


namespace EVMESCharts.TableList {
    
    
    /// <summary>
    /// HomeTableView
    /// </summary>
    public partial class HomeTableView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 113 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DataGrid;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Hour;
        
        #line default
        #line hidden
        
        
        #line 227 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton day;
        
        #line default
        #line hidden
        
        
        #line 252 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton Month;
        
        #line default
        #line hidden
        
        
        #line 286 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid All;
        
        #line default
        #line hidden
        
        
        #line 301 "..\..\..\TableList\HomeTableView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup allpop;
        
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
            System.Uri resourceLocater = new System.Uri("/EVMESCharts;component/tablelist/hometableview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\TableList\HomeTableView.xaml"
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
            this.DataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 2:
            this.Hour = ((System.Windows.Controls.RadioButton)(target));
            
            #line 206 "..\..\..\TableList\HomeTableView.xaml"
            this.Hour.Checked += new System.Windows.RoutedEventHandler(this.Hour_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.day = ((System.Windows.Controls.RadioButton)(target));
            
            #line 229 "..\..\..\TableList\HomeTableView.xaml"
            this.day.Checked += new System.Windows.RoutedEventHandler(this.Day_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Month = ((System.Windows.Controls.RadioButton)(target));
            
            #line 254 "..\..\..\TableList\HomeTableView.xaml"
            this.Month.Checked += new System.Windows.RoutedEventHandler(this.Month_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.All = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            
            #line 294 "..\..\..\TableList\HomeTableView.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.AllMouseEnter);
            
            #line default
            #line hidden
            
            #line 295 "..\..\..\TableList\HomeTableView.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.AllMouseLeave);
            
            #line default
            #line hidden
            
            #line 296 "..\..\..\TableList\HomeTableView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AllClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.allpop = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


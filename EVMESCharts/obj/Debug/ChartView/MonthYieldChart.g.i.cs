﻿#pragma checksum "..\..\..\ChartView\MonthYieldChart.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A11C0C1A753BCFA4C8DA65638E2C1B5ED4736EB1"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using EVMESCharts.ChartView;
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


namespace EVMESCharts.ChartView {
    
    
    /// <summary>
    /// MonthYieldChart
    /// </summary>
    public partial class MonthYieldChart : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\..\ChartView\MonthYieldChart.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Reset;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\ChartView\MonthYieldChart.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup pop3;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\ChartView\MonthYieldChart.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid All;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\ChartView\MonthYieldChart.xaml"
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
            System.Uri resourceLocater = new System.Uri("/EVMESCharts;component/chartview/monthyieldchart.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ChartView\MonthYieldChart.xaml"
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
            this.Reset = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\..\ChartView\MonthYieldChart.xaml"
            this.Reset.MouseEnter += new System.Windows.Input.MouseEventHandler(this.ResetMouseEnter);
            
            #line default
            #line hidden
            
            #line 46 "..\..\..\ChartView\MonthYieldChart.xaml"
            this.Reset.MouseLeave += new System.Windows.Input.MouseEventHandler(this.ResetMouseLeave);
            
            #line default
            #line hidden
            
            #line 47 "..\..\..\ChartView\MonthYieldChart.xaml"
            this.Reset.Click += new System.Windows.RoutedEventHandler(this.ResetClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.pop3 = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 3:
            this.All = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            
            #line 67 "..\..\..\ChartView\MonthYieldChart.xaml"
            ((System.Windows.Controls.Button)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.AllMouseEnter);
            
            #line default
            #line hidden
            
            #line 68 "..\..\..\ChartView\MonthYieldChart.xaml"
            ((System.Windows.Controls.Button)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.AllMouseLeave);
            
            #line default
            #line hidden
            
            #line 69 "..\..\..\ChartView\MonthYieldChart.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AllClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.allpop = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

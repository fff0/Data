   M S B A M L   `   `   `  ����U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf  5      F BEVMESCharts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:EVMESCharts.TableListEVMESCharts.TableList 5      .   (EVMESCharts.TableList.RunStatusTableView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5      5local#clr-namespace:EVMESCharts.TableList     5        � 	Resources  5
         ContentStyle �� 
TargetType �� Property �� Value��%R  &	        '�� �     '�� �     �� 5   
   " ��6%   .�5      ��  � ��$ Center=�6?   �� 5       � ��$ Center=�6?   �� 5       � ��$
 Wrap=�6?   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   5      �� 5   
   " ��6   .�5      ��  � �� �#dddddd6=   5      5      .��5      � $��5 0 0 0q�6    � RowDefinitions 5   
   �� 5      $��35 �6   �� 5      $��2 �6   �� 5      $��3* �6   �� 5      $��2* �6   .+�5   
   � $
��10 0q�6   .+�5       ��  �� Orientation$ 
Horizontal=�6   .+�5!      �� $	����6   $��22(�5"      $��../Fonts/#iconfont)�5#      ��5$      �� *	FontColor+�� 5%      $��故障信息��6   $��18(�5&      $��10 0 0 0q�5'      ��5(      �� *	FontColor+5)      5*      �� 5+   
   $��1��6   $��	10 0 10 0q�6   62   � 5,   
   $��2��6   .+�5-      �� $��2��6   
�������$��10 0 5 0q�67    J� Effect 5.      3  -System.Windows.Media.Effects.DropShadowEffect  5/         Color$ #ececec��6'      
BlurRadius$ 5Q�69   	   ShadowDepth$	 1Q�6I   
   Opacity$
 1Q�6Z   ._�51      &   System.Windows.Controls.DataGrid  -   5          Name$ DataGrid��51      ���#303153   HeadersVisibility$ Column=�53         AutoGenerateColumns .     CanUserAddRows .     VerticalGridLinesBrush
 ����    HorizontalGridLinesBrush
 �����$��0q�58      ��59      �� *	FaultList+   ColumnHeaderStyle 5:      �� 5;      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6    .�5<      ��  � ��$ 0q�6T   �� 5=       � ��$ Center=�6T   �� 5>       � ��$ Center=�6T   �� 5?       � �� �#3c3d596T   �� 5@       � �� �#fff6T   �� 5A       � ��$
 Bold"�6T   �� 5B       � ��$ 35��6T   5C         RowStyle 5F      �� 5G      )  #System.Windows.Controls.DataGridRow"  6    .�5H      ��  � ��$ 40��6T   �� 5I       � �� �#fff6T   �� 5J       � �� �#3031536T   �� 5K       � ��$ Center=�6T   �� 5L       � ��$ Center=�6T   5M         Columns 5P      0  *System.Windows.Controls.DataGridTextColumn  5Q      ,  &System.Windows.Controls.DataGridColumn   Header$ 故障信息��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$ 5* 5R   -   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [  5T   -      
IsReadOnly .    SortDirection$ 
DescendingX�5V   -      Binding 5S   -   �� *	FaultName+5V   -     5W      $ 故障次数��6-   $ 3* 5X   -   # [  5Z   -    .  5Y   -   �� *FaultNumber+5[   -     5\      $ 所占比例��6-   $ 3* 5]   -   # [  5_   -    .  5^   -   �� *Ratio+5`   -     5a      $ 时间��6-   $ 3* 5b   -   # [  5d   -    .  5c   -   �� *Time+5e   -   5g      5h      5i      � 5k   
   $��3��6   $��	0 10 5 10q�6   .+�5l      ��  �� CornerRadius$ 10��64   $��
10 10 0 10q�6F   ��6   �� *	BgColor+ 5m        5n      $ #ececec��6'   $ 5Q�67   $	 1Q�6F   $
 1Q�6V   ._�5p      �  5q      �� 5r      $��30 �6(   �� 5s      .+�5w      � .+�5x      �� $��电气消耗图表��6$   $��Center=�5y       $��14(�5z       $��30 5 0 0q�5{       ��5|       �� *	FontColor+5}      � 5�      $��1��6   .+�5�      #	   LiveCharts.Wpf.CartesianChart	  $��10q�6-   &
    LiveCharts.Wpf.Charts.Base.Chart 
  LegendLocation$	 Top=�6:    
  DisableAnimations . ��6h   �� *	FontColor+ 
  Series 5�   "   !   LiveCharts.Wpf.ColumnSeries     LiveCharts.Wpf.Series   Title$ 	用电量��63      Fill �#60acfc    Values  5�   +   �� *ConsumePower+5�   +     5�   "   $ 	用气量��63    �#5bc49f  5�   +   �� *ConsumptionGas+5�   +   ! 
  AxisX! 5�   "      LiveCharts.Wpf.Axis  "   
ShowLabels" . #   Title# 6+   �� *ChartXAxisTitle+$   MinValue$ 5�   #   �� *
AxisXMin+%   MaxValue% 5�   #   �� *
AxisXMax+&   LabelFormatter& 5�   #   �� *
XFormatter+' 
  AxisY' 5�   "     $# 	消耗量��6+   $$ 0Q�5�   #   " . 5�   #   5�      5�      5�      5�      5�      5�      5�      
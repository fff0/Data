   M S B A M L   `   `   `  ����F  BEVMESCharts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:EVMESCharts.ChartViewEVMESCharts.ChartView  5      )    #EVMESCharts.ChartView.HomeDataTable   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      5local#clr-namespace:EVMESCharts.ChartView     5        � 	Resources  5	       �� 
TargetType �� Property �� Value)  #System.Windows.Controls.DataGridRow*  $System.Windows.Controls.DataGridCell �� ScaleX �� ScaleY �� Angle=  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�  '��       '  �     '  �    '  �    �� 5
   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ Center=�64   �� 5       � ��$ 18(�6)   5      �� 5   
   "  6   .�5      ��  � ��$ 70��6'   �� 5       � ��$ Center=�62   �� 5       � ��$ 14(�6)   �� 5       � �� �#3031536+   �� 5       � ��$ 0 20 0 0q�6(   6;   5      �� 5   
   "  6   .�5      ��  � {� .  6*   �� 5       � ��$ Center=�62   �� 5       � ��$ Center=�64   �� 5       �@�LayoutTransform 5      V� 5      .~�5      �� $ 1Q�6)   $ -1Q�64   �� 5      $ 90Q�6*   5      5!      5"      �� 5#   
   "  6   .�5$      ��  � {� .  6*   �� 5%       � ��$	 110��6&   �� 5&       � �� �#3031536+   �� 5'       � ��$ Center=�6;   �� 5(       � ��$ 16(�6)   �� 5)       � �� �#fff6+   �� 5*       �@�LayoutTransform 5+      V� 5,      .~�5-      �� $	 -90Q�6*   �� 5.      $ 1Q�6)   $ -1Q�64   5/      51      52      53      .��54      �  � ColumnDefinitions 55   
   �� 56      �� 57      $	��100 �6   .+�5:   
   &   System.Windows.Controls.DataGrid  -   5          Name$ DG��5=      $
��Auto=�5:      $
��Auto=�5;      	   HeadersVisibility$	 Column=�5<      
   AutoGenerateColumns
 .     CanUserAddRows .  ���#303153$	��170��5B         GridLinesVisibility$
 None=�5C      ���#303153��5>      �� *DevNameList+ � LayoutTransform 5E      V� 5F      .~�5G      �� $	 -90Q�6&   �� 5H      $ 1Q�6%   $ -1Q�60   5I         Columns 5K      0  *System.Windows.Controls.DataGridTextColumn  5L      ,  &System.Windows.Controls.DataGridColumn   Header$ 时间��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6K      
Foreground �#fff1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Time+6U     5M      $ 生产效率��6%   $ * 6S    �#fff 63   �� *
Efficiency+6]     5N      $ 良率��6%   $ * 6L    �#fff 62   �� *Good+6V     5O      $ 时间稼动率��6%   $ * 6X    �#fff 64   �� *TimeEfficiency+6b   5Q      � 5S   
   $��1��6   .+�5T        -   5        $ Table��5W      $
��Auto=�5T      $
��Auto=�5U      $
	 None=�5V      
 .   .  ���#303153$	��170��5\      $
 None=�5]      ���#303153��5X      �� *SummaryList+ 5_      V� 5`      .~�5a      �� $	 -90Q�6*   �� 5b      $ 1Q�6)   $ -1Q�64   5c       5e        5f      $ * 6D    �#fff 6)   �� *Title+6N     5g      $ * 6J    �#fff 6*   �� *
Efficiency+6T     5h      $ * 6D    �#fff 6*   �� *Good+6N     5i      $ * 6N    �#fff 6*   �� *TimeEfficiency+6X   5k      5l      5m      5n      
   M S B A M L   `   `   `  ����F  BEVMESCharts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:EVMESCharts.PopupEVMESCharts.Popup  5   	   "    EVMESCharts.Popup.FaultsList   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:EVMESCharts.Popup     5   	     � Title$  告警详细信息��5   	   $	��600��6   $	��600��6%    � 	Resources 5	       �� 
TargetType �� Property �� Value   ContentStyle��%  '��       '�� �     &	  T    �� 5
   
   " ��6   .�5      ��  � ��$ Center=�6;   �� 5       � ��$ Center=�69   �� 5       � ��$ 65��6&   �� 5       � ��$ 14(�6)   �� 5       � ��$ 30��6(   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�64   �� 5       � ��$ Center=�62   5      �� 5   
   " ��6%   .�5      ��  � ��$ Center=�6?   �� 5       � ��$ Center=�6?   �� 5       � ��$
 Wrap=�6?   5      5      .��5      � ��6   �� *	BgColor+ � RowDefinitions 5   
   �� 5      $��60 �6   �� 5      �� 5      $��80 �6   .+�5"   
   �� $��20(�60   ��6   �� *
TableTitle+��6>   �� *	FontColor+&   System.Windows.Controls.DataGrid  5%   
   -   5          Name$ DataGrid��5%      $��1��5&      $��2��5'      
������    HeadersVisibility$ Column=�5)         AutoGenerateColumns .  	   CanUserAddRows	 .  
   VerticalGridLinesBrush

 ����    HorizontalGridLinesBrush
 �����$��0q�5.      $
��50 0q�50      ��5/      �� *	FaultList+   ColumnHeaderStyle 51      �� 52      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�53      ��  � ��$ 3q�6L   �� 54       � ��$ Center=�6L   �� 55       � ��$ Center=�6L   �� 56       � ��
 ���� 6L   �� 57       � ��
 �  ��6L   �� 58       � ��$
 Bold"�6L   �� 59       � ��$ 50��6L   �� 5:       � ��$ 18(�6L   5;         RowStyle 5>      �� 5?      )  #System.Windows.Controls.DataGridRow"  6   .�5@      ��  � ��$ 40��6L   �� 5A       � ��
 �  ��6L   �� 5B       � �� �#3031536L   �� 5C       � ��$ Center=�6L   �� 5D       � ��$ Center=�6L   5E         Columns 5H      0  *System.Windows.Controls.DataGridTextColumn  5I      ,  &System.Windows.Controls.DataGridColumn   Header$ 故障信息��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 2* 5J   %   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [  5L   %      
IsReadOnly .    FontSize$ 16Q�5N   %      Binding 5K   %   �� *	FaultName+5O        5P      $ 故障次数��6%   $ 2* 5Q   %   # [  5S   %    . $ 16Q�5U   %    5R   %   �� *FaultNumber+5U   %     5V      $ 所占比例��6%   $ 2* 5W   %   # [  5Y   %    . $ 16Q�5[   %    5X   %   �� *Ratio+5[   %   5]      � 5`   
   $��2��6   .+�5a      �� -   5        $��	btnAppend��5b      $��3��5c      $��确定��5d      $��20(�5e      $	��250��5f      $��40��5g      ��5h      �� 5i      " ��6   .�5j      ��  � �� �#95c4ff6?   �� 5k       � ��
 �����6?    �� Triggers 5l      P� 5m       P� Property �@J�IsMouseOver P� Value . . �5n   "   ��  � �� �#4395ff6?   5o      5q      ��5s      �� 5t       �� 
TargetType" ��6&   .R�5u      ��  �� CornerRadius$ 10��6!   #��z��63   ._�5v      �� $��Center=�6/   $��Center=�6L   5w      5x      5z      5{      5|      5}      
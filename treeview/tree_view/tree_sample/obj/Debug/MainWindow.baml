   M S B A M L   `   `   `  ����F  Btree_sample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:tree_sampletree_sample  5   	   H DSearchTextBox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null>/clr-namespace:UIControls;assembly=SearchTextBox
UIControls 5   	       tree_sample.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:tree_sample     5   	   F
UIControls/clr-namespace:UIControls;assembly=SearchTextBox     5   	   -   5          � Title$  
MainWindow��5
   	   $	��350��6   $	��525��6)   .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $	��.7* �6    � RowDefinitions 5   
   �� 5      $
��1.5* �6   �� 5      $��1* �6   �� 5      $
��1.1* �6   �� 5      $	��19* �6   .+�5   
   �� $��TreeView��6   $��Stretch=�6"    �� TextAlignment$ Center=�6?   $��2��6W   $��1��6g   
�������$
��Bold"�6�   
���   �6�     UIControls.SearchTextBox  5   
   -   5        $��txtNameToSearch��5   #   $��2��6e   $��2��6u   
���   �6�   R� 5   
   -   5         R�Name$ 
FolderView��5      $��2��6'   $��3��6{   $��Stretch=�6�   
���   � � ContextMenu 5      �� 5      .@�5      m� $
��Copy��6   m� 5      $��Delete��6   m� 5      $��Rename��6   5       � 	Resources 5"       �� 
TargetType �� Property	 �� Value
 �� Orientation �� RelativeSource �� Mode �� AncestorType �� Path(  "tree_sample.HeaderToImageConverter   Instance �� 	Converter��%M  'Q�       �� 5#      " Q�6   .�5$      ��  � ��	 5%      �� 5&      .R�5'   "   �� $
 
Horizontal=�6-   .+�5(   &   �� $��20��6,   $��3q�67   ��5)   &   ��  �� $ FindAncestor=�" Q�$	 Tag�# Z 5-   s   �� 5.   &   $��Center=�60   ��6K   �� 6^   5/   #   50      52      53      54      55      �� 56   
   $��0��6   $��1��6!   $��2��6.    J� Opacity$ .4Q�6B   $
��Name��6O   6`   59      5:      
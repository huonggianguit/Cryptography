   M S B A M L   `   `   `  ����F  BFinance Pro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Finance_Pro.LoginFinance_Pro.Login  5      '    !Finance_Pro.Login.UC_TextboxLogin   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Finance_Pro.Login     5      -   5          >�Name$  Uc_TextboxLogin��5
      $	��250��5      $��55��5	      .��5      � $��0,0,0,0q�6   .+�5   
   �� -   5         ��Name$ txtBeforeUsername��5      $��
Montserrat)�5      $
��Bold"�5      ��5      �� *PlaceHolder+ �� Mode$ TwoWay=� �� ElementName$ Uc_TextboxLogin����5      �� 5       �� Triggers 5      �� 5       �� Binding 6&   �� *Text+$ TwoWay=�$ txtUser�� �� Value  6a    N� EnterActions 5      �� 5   "   .��5   &   ��  g� 
SpeedRatio$ 10Q�61   .��5   *   W� 	 �� TargetProperty$	 FontSize�6:   	
 W� To
 156_   W� 5   *   $	 Opacity�6:   
 0.36^   t� 5   *   $	 Margin�6=   	 t� To 	40,30,0,06`   �� 5   *   $6	 0(TextElement.Foreground).(SolidColorBrush.Color)�69   	 �� To #a3a3a46�    g� Duration$ 0:0:1D�6�   5   '   5   #    N� ExitActions 5      �� 5    "   .��5!   &   �� $ 15Q�61   .��5#   *   W� $	 FontSize�6:   
 186_   W� 5%   *   $	 Opacity�6:   
 16^   t� 5'   *   $	 Margin�6=    40,0,0,06`   �� 5)   *   $6	 0(TextElement.Foreground).(SolidColorBrush.Color)�69    Red6�   $ 0:0:1D�6�   5+   '   5,   #   5.      50      52      �� 53   
   -   5         ��Name$ txtUser��53      $	��250��54      $��30��55      $��20(�56      $��
Montserrat)�57      $��Medium"�58      
������� �� 
CaretBrush
 �����
�������$��0,0,0,1q�5<      $��Bottom=�5=      $��40,0,0,0q�5>      .��6&   ��5?      �� 5@       �� ImageSource$ /assets/UserIcon.png��6    j� Stretch$
 None=�5A       j� 
AlignmentX$
 Left=�5B       j� 
AlignmentY$ Center=�5C      .��5D   "   5E      5F      5G      
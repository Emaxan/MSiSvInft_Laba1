   M S B A M L   `   `   `  ����  
MSiS_Laba1    MSiS_Laba1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  WMain��5   	    � Title$ 
MainWindow��5   	   $	��525��6    � 
ResizeMode$ NoResize=�6(    � WindowStartupLocation$ CenterScreen=�6>   ���	#FF404040 � WindowStyle$
 None=�5   	    � SizeToContent$ Height=�6   .��5      � -   5         �Name$ GMain��5       � RowDefinitions 5   
   �� 5      $��8* �6   �� 5	      $	��92* �6   .+�5   
   �� -   5         ��Name$ LTitle��5      $��0��6    $��MSiSvInfT__Laba1��6-   $��Stretch=�6H   $��Stretch=�6f   $��Center=�6�   $
��Bold"�6�   $��15(�6�   �� 5   
   -   5        $ BClose��5      $��0��6    $��X��6-   $��20��69   $��Right=�6D   $��Stretch=�6`   $��18(�6|   6�   �� 5   
   -   5        $ 	LFilePath��5      $��1��6#   $��
File path:��60   $��18(�6E   �� 5   
   -   5        	 ��Name$	 
TbFilePath��5      $��1��6&   $��Stretch=�63   $	��Top=�6Q   $��20��6i   ���	#FF707070$��
80,10,80,0q�6�   
 �� 
CaretBrush

 �   �6�   �� 5   
   -   5         ��Name$ BOpen��5      $��1��6    $��Open...��6-   $��Right=�6@   $	��Top=�6\   $��20��6t   $��60��6�   ���	#FF707070$��	0,10,10,0q�6�   6�   �� 5   
   -   5        $	 
TbFileText��5      $��1��6&   $��Center=�63   $	��Top=�6P   $	��500��6h   $	��500��6u   ���	#FF707070$��0,40,0,0q�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�   

 �   � �� AcceptsReturn .  �� 
AcceptsTab . 6�   �� 5   
   -	   5        $ LResult��5      $��1��6!   $��Result:��6.   $	��Top=�6@   $
��Left=�6X   $��18(�6s   $��	0,540,0,0q�6�   �� 5   
   -
   5        $ BAnalize��5      $��1��6#   $��Analize��60   $��Right=�6B   $	��Top=�6^   $	��100��6v   $��20��6�   $��
0,545,10,0q�6�   ���	#FF7070706�   �� 5   
   -   5        $	 TbResult��5      $��1��6$   $��Center=�61   $	��Top=�6N   $	��100��6f   $	��500��6s   ���	#FF707070$��
0,570,0,10q�6�    �� 
IsReadOnly . $
 Auto=�6�   

 �   �6�   5      5      
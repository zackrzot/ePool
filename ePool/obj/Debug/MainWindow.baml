   M S B A M L   `   `   `  ����@  <ePool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ePoolePool  5   	       ePool.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:ePool     5   	     � Title$  ePool��5   	   $��591.5��6   $��868.5��6&    � 
ResizeMode$ NoResize=�5	   	    � Icon$ Resources/ePool.ico��6   .��5
      � .+�5   
   (� $��1��6    (� Stretch$ Uniform=�6   $��Center=�61   � 5   
   $
��Left=�6   $��
10,10,0,10q�6*   $	��840��6>   .+�5      �� -   5         ��Name$ Image��5      $	��490��6#   $��10,10,340,139q�6/    J� RenderTransformOrigin$ 0.5,0.5&�6F   �� 5      -   5        $ 
ImageColor��5      $��510,10,10,297q�6(   $ 0.5,0.5&�6?   �� 5      -   5         ��Name$ 	statusBar��5   6   $��Stretch=�6   $��Bottom=�6I   
�������
���   �$��10,0,0,0q�6�   �� 5      -   5         ��Name$ button_saveDepthData��5      $��	Save Data��63   $
��Left=�6G   $��365,471,0,0q�6b   $	��Top=�6w   $��67��6�   6�   �� 5      -   5         ��Name$ textBox_TLX��5      $
��Left=�6+   $��23��6F   $��365,415,0,0q�6R   	 �� TextWrapping$
	 Wrap=�6g   $	��180��6{   $	��Top=�6�   $��52��6�   �� 5      -   5        $ textBox_TLY��5      $
��Left=�6+   $��23��6F   $��433,415,0,0q�6R   $
	 Wrap=�6g   $	��205��6{   $	��Top=�6�   $��52��6�   �� 5      -   5        $ textBox_BRX��5      $
��Left=�6+   $��23��6F   $��365,443,0,0q�6R   $
	 Wrap=�6g   $	��255��6{   $	��Top=�6�   $��52��6�   {�.  6�   �� 5      -   5        $ textBox_BRY��5      $
��Left=�6+   $��23��6F   $��433,443,0,0q�6R   $
	 Wrap=�6g   $	��250��6{   $	��Top=�6�   $��52��6�   {�.  6�   �� 5      -	   5        
 ��Name$
 richTextBox_console��5       �� 
IsReadOnly . $
��Left=�6I   $	��264��6d   $��510,252,0,0q�6q   $	��Top=�6�   $	��320��6�   $��4q�6�   $ �� HorizontalScrollBarVisibility$ Disabled=�6�   " �� VerticalScrollBarVisibility$ Visible=�6�   ��. $��10(�62   �� 
LineHeight$ 6��6@  . �5      /� .T�5      J� .*�6    .*�5      �� $� ��6   5      5      5      �� 5      -
   5         ��Name$ 
slider_pan��5      $
��Left=�6)   $��180,414,0,0q�6D   $	��Top=�6Y   $	��131��6q    � Maximum$	 255Q�6}    � SmallChange$ 1Q�6�   �� 5      -   5         ��Name$ comboBox_light��5      $
��Left=�6/   $��
44,413,0,0q�6J   $	��Top=�6^   $��80��6v    �� SelectedIndex$ 0��6�   �� 5      -   5        $ comboBox_color��5      $
��Left=�6/   $��
44,440,0,0q�6J   $	��Top=�6^   $��80��6v   $ 0��6�   �� 5       -   5        $ comboBox_gobo��5       $
��Left=�6.   $��
44,467,0,0q�6I   $	��Top=�6]   $��80��6u   $ 0��6�   �� 5!      -   5        $ comboBox_strobe��5!      $
��Left=�60   $��44,494,0,-18q�6K   $	��Top=�6a   $��80��6y   $ 0��6�   �� 5"      -   5         ��Name$ label��5"      $��Light��6#   $��Right=�63   $��0,414,796,0q�6O   $	��Top=�6d   �� 5#      -   5        $ 
label_Copy��5#      $��Color��6(   $��Right=�68   $��0,440,796,0q�6T   $	��Top=�6i   �� 5$      -   5        $ label_Copy1��5$      $
��Gobo��6)   $��Right=�68   $��0,467,796,0q�6T   $	��Top=�6i   �� 5%      -   5        $ label_Copy2��5%      $��Strobe��6)   $��Right=�6:   $��0,494,795,-22q�6V   $	��Top=�6m   �� 5&      -   5        $ label_Copy3��5&      $	��Pan��6)   $��Right=�67   $��0,410,660,0q�6S   $	��Top=�6h   �� 5'      -   5        $ 	label_pan��5'      $��0��6'   $��Right=�63   $��0,411,495,0q�6O   $	��Top=�6d   $ 0.517,-0.038&�6|   �� 5(      -   5        $ slider_pan_fine��5(      $
��Left=�6.   $��180,441,0,0q�6I   $	��Top=�6^   $	��131��6v   $	 255Q�6�   $ 1Q�6�   6�   �� 5)      -   5        $ label_Copy5��5)      $��Pan Fine��6)   $��Right=�6<   $��0,438,661,0q�6X   $	��Top=�6m   �� 5*      -   5        $ label_pan_fine��5*      $��0��6,   $��Right=�68   $��0,438,495,0q�6T   $	��Top=�6i   �� 5+      -   5        $ slider_tilt��5+      $
��Left=�6*   $��180,469,0,0q�6E   $	��Top=�6Z   $	��131��6r   $	 255Q�6~   $ 1Q�6�   6�   �� 5,      -   5        $ label_Copy7��5,      $
��Tilt��6)   $��Right=�68   $��0,465,661,0q�6T   $	��Top=�6i   �� 5-      -   5        $ 
label_tilt��5-      $��0��6(   $��Right=�64   $��0,466,495,26q�6P   �� 5.      -   5        $ slider_tilt_fine��5.      $
��Left=�6/   $��180,495,0,0q�6J   $	��Top=�6_   $	��131��6w   $	 255Q�6�   $ 1Q�6�   6�   �� 5/      -   5        $ label_Copy9��5/      $��	Tilt Fine��6)   $��Right=�6=   $��0,491,661,0q�6Y   $	��Top=�6n   �� 50      -   5        $ label_tile_fine��50      $��0��6-   $��Right=�69   $��0,492,495,0q�6U   $	��Top=�6j   �� 51      -   5        $ button_setLight��51      $��	Set Light��6.   $
��Left=�6B   $��365,497,0,0q�6]   $	��Top=�6r   $��67��6�   $��22��6�   6�   �� 52      -   5        $ button_lightOff��52      $	��Off��6.   $
��Left=�6<   $��437,497,0,0q�6W   $	��Top=�6l   $��48��6�   6�   �� 53      -    5        $ button_detect��53      $��Detect��6,   $
��Left=�6=   $��437,471,0,0q�6X   $	��Top=�6m   $��48��6�   6�   54      55      56      
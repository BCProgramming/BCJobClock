   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  E ABCJobClock, Version=1.6.0.1, Culture=neutral, PublicKeyToken=null'clr-namespace:BCJobClock
BCJobClock 'clr-namespace:BCJobClock
BCJobClock U QSystem.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3aG4clr-namespace:System.Drawing;assembly=System.DrawingSystem.Drawing    BCJobClock.wndJobClock   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     9System&clr-namespace:System;assembly=mscorlib     *localclr-namespace:BCJobClock     -controlsclr-namespace:BCJobClock     HDrawing4clr-namespace:System.Drawing;assembly=System.Drawing     -     � Title$  JobClock PIN Entry��$
��1440��$	��900��$��Stretch=� � WindowStyle$
 None=� � WindowState$ 	Maximized=� � WindowStartupLocation$ CenterScreen=� � Topmost .   � Icon$( "/BCJobClock;component/timelist.ico������  � ColorInterpolationMode$ SRgbLinearInterpolation=����  � Color$ White�� � Offset$	 0.1Q�� $
 Gray��$ 1Q�	 � 	Resources	    BCJobClock.BitmapConverter   BitmapConverter  StandardEntryButtonStyle
 �� Property �� Value  WorkOrderEntryButtonStyle �� 
TargetType  MessageItemTemplate �� Path��%  &	        &	      &	 @     '�� �     &	 �       �� .��� 
 � ��$ 48(��� 
 � ��$
 Bold"�-   �� .��� 
 � ��$ 18(��� 
 � ��$
 Bold"��� 
 � |� .  -   �� " ��.�-   �� 
 � |� .  �� .R��� ���� $ Message�.��]� $��Stretch=�.+�(�  J� SnapsToDevicePixels .  $	��Top=�.��� -   $��TopLevelGrid�� � ColumnDefinitions ��  � RowDefinitions �� .+�� -   $��PinEntry��$��0��$��0�� �� $
��Auto ��� $
��Auto ��� $
��Auto � �� .+�� $��0��$��0�� �� $
��Auto �$	��136����  �� $
��Auto �$	��329���� .+��� -   $��label1��$��0��$��0��$	��106��$��4,0,2,0q�$��Center=�$��72(�.��
Enter PIN:�� -   $��lblPIN��$��1��$��5q�$�� ��$	��Top=�$��13,10.233,74,0q�$��72(�$��115.767������  �� Opacity$	 0.4Q�.��� $ 	Turquoise��$ 0Q�� $
 Navy��$ 1Q�����  �� Color$ White���� -	   $��
lblPINpass��$��1��$y�	Collapsed=�$��5q�$�� ��$	��Top=�$��13,10.233,74,0q�$��72(�$��115.767������ $	 0.4Q�.��� $ 	Turquoise��$ 0Q�� $
 Navy��$ 1Q����� $ White����  �� Orientation$ Vertical=�$��0��$��1�� � DataContext ��  �� ElementName$ lstUserNames��$ SelectedItems�.+�9� -
   $��UserListExpander������ $��24(�.��
Users List.���� -   $��lstUserNames�� �� SelectionMode$ Single=�$��24(�$��Stretch=�$��5q�$	��400����. $
��Auto=����� .R��� $��1q�
�����F� �� CornerRadius$ 4��$��Stretch=�$	��200��._��� -   
 � Tag �� .���� $ 
Horizontal=�$��Stretch=�.+�� .+��� ���� $ properUserImage��� $��14(�$
��Left=�$��Bottom=����� $ ActiveJobCountString��� $��Stretch=����� $
 Name�� -   $��EntryButtons��
������� J� Opacity$ 1Q�$��2��$��1��$��4q�$��2��$	��640��$	��480��$��Center=� �� �� �� ��  �� �� �� .+��� -   $��EntryButton7��$��0��$��0��#��[ $��5q�.��7�� -   $��EntryButton8��$��0��$��1��#��[ $��5q�.��8�� -   $��EntryButton9��$��0��$��2��#��[ $��5q�.��9�� -   $��EntryButton4��$��1��$��0��#��[ $��5q�.��4�� -   $��EntryButton5��$��1��$��1��#��[ $��5q�.��5�� -   $��EntryButton6��$��1��$��2��#��[ $��5q�.��6�� -   $��EntryButton1��$��2��$��0��#��[ $��5q�.��1�� -   $��EntryButton2��$��2��$��1��#��[ $��5q�.��2�� -   $��EntryButton3��$��2��$��2��#��[ $��5q�.��3�� -   $��EntryButtonC��$��3��$��0��
���  ��#��[ $��3q�.��C�� -   $��EntryButton0��$��3��$��1��#��[ $��5q�.��0�� -   $��EntryButtonOK��$��3��$��2��
��� � �#��[ $��	WingDings)�$��3q�.��ü� $��0��$��1��$��5q�$	��230��$��Stretch=� �� $ lstLogEntries��$ SelectedItems� �� $��35 ��� .+��� -   $��lblLogEntry��$��18(�$��0��.��Recent Activity�� -   $��lstLogEntries��$��18(�$��1��$	��500��$
��Left=�#��[ ���� $ 
MESSAGELOG��� -   $��	WorkOrder��$��Center=�$��Center=�$��0��$��0��$ Vertical=�$��50q�
��� � �$y�	Collapsed=�.+��� -   $��lblScanPromptLabel��$��10q�$��48(�$��Stretch=�$	��500��.���� -   $��lblScanPrompt��$,��&Hello BLAH, please scan the work order��$
��Wrap=��� -   $��WorkOrderEntry��{�.  !  BCJobClock.TextBoxAllowable   AllowedChars$ 
1234567890��$��18(�$��	Monospace)�$��12q� �� AcceptsReturn .  �� -    $��CancelScanEntry��$��Bottom=�$��Center=�$��70��$��18(�|�.  $��2,2,2,8q�.��Cancel9� -!   $��	Clockedin��$��Stretch=�$ 1Q�$y�	Collapsed=�|�.  ���� $ SRgbLinearInterpolation=���� $
 Gray��$	 0.1Q�� $ White��$ 1Q����� $��Stretch=� �� TextAlignment$ Justify=�$	��480������ $ SRgbLinearInterpolation=���� $
 Gray��$	 0.1Q�� $ White��$ 1Q�Clocked-in orders.���  �� $ lstExistingOrders��$ SelectedItems�.+��� -"   $��lstExistingOrders��|�.  $��14(�$ Single=�9� $��Stretch=�$ 1Q����� $ SRgbLinearInterpolation=���� $
 Gray��$	 0.1Q�� $ White��$ 1Q����� $��Stretch=�$ Justify=�$	��480������ $ SRgbLinearInterpolation=���� $
 Gray��$	 0.1Q�� $ White��$ 1Q�Manual Entry.��� -#   $��EntryButtonsWorkOrder��
�������$ 1Q�$��2��$��1��$��4q�$��2��$	��320��$	��240��$��Right=� �� �� �� ��  �� �� �� .+��� -$   $��	WOButton7��$��0��$��0��#��[ $��5q�.��7�� -%   $��	WOButton8��$��0��$��1��#��[ $��5q�.��8�� -&   $��	WOButton9��$��0��$��2��#��[ $��5q�.��9�� -'   $��	WOButton4��$��1��$��0��#��[ $��5q�.��4�� -(   $��	WOButton5��$��1��$��1��#��[ $��5q�.��5�� -)   $��	WOButton6��$��1��$��2��#��[ $��5q�.��6�� -*   $��	WOButton1��$��2��$��0��#��[ $��5q�.��1�� -+   $��	WOButton2��$��2��$��1��#��[ $��5q�.��2�� -,   $��	WOButton3��$��2��$��2��#��[ $��5q�.��3�� --   $��	WOButtonC��$��3��$��0��
���  ��#��[ $��3q�.��C�� -.   $��	WOButton0��$��3��$��1��#��[ $��5q�.��0�� -/   $��
WOButtonOK��$��3��$��2��
��� � �#��[ $��	WingDings)�$��3q�.��ü� -0   $��AlertWindow��$��Center=�$��0��$��0��$��50q�$	��300��
�������$y�	Collapsed=� J� BitmapEffect O�  O� 	GlowColor$	 Red�� O� GlowSize$ 8Q� �� �� �� ��  �� �� �� �� �� �� �� �� .+��� -1   $��ALabel��$��Center=�$��5,5,5,5q�$��18(�$	��300��$��Stretch=�$��0��$��0��$��8��$��3��$	��400��.���� -2   $��
AlertLabel��$,��&This is where the Message will appear.��$
��Wrap=��� $ 
Horizontal=�$��3��$��1��$��7��.+��� -3   $��AlertOK��$��18(�$��1��$��1��$��9,0,4,20q�.��OK�� -4   $��AlertCancel��$��18(�$��1��$��2��$��9,0,4,20q�.��Cancel�� -5   $��VersionLabel��$��Bottom=�$
��Left=�.��Version 1.0.0�� -6   $��jmtclick��#��[" �$��Bottom=�$��Right=�$	��350��$��83��.���� $��jmt.png���� -7   $
��icon��#��[" �$��Bottom=�$��Right=�$��32��$��32��.���� $��	bcico.png��
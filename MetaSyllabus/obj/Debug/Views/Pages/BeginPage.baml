   M S B A M L   `   `   `  ����G  CMetaSyllabus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:MetaSyllabus.ViewModelsMetaSyllabus.ViewModels  5      A%clr-namespace:MetaSyllabus.AnimationsMetaSyllabus.Animations  5      "    MetaSyllabus.Pages.BeginPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      4vm%clr-namespace:MetaSyllabus.ViewModels     5      3a%clr-namespace:MetaSyllabus.Animations     5        N� Title$
  Home��5       N� 
Background �#008CDC N� 	KeepAlive .  +� FocusedElement 5	      ��  �� ElementName$ 	SearchBox��.��5      �� $	��550��6   ���#0094E4$��Stretch=�5      .+�5!   
   4  .MetaSyllabus.Animations.PetersenGraphAnimation     ScalingFactor$	 350��6#      AnimationDuration$	 3.6��5"   #      EdgeThickness$ 5��5#   #      
EdgeColour$ White��5$   #   $��Center=�5%   #   $��	0,60,0,20q�5&   #   �� 5(   
   $��MetaSyllabus��6   $��30(�5)      $
��Bold"�5*      $��SemiExpanded&�5+      $��Center=�5,      
�������$��0,0,0,10q�5.      �� 50   
   $)��#What would you like to learn about?��6   $��18(�51      $��SemiBold"�52      
�������$��Center=�54      $��0,0,0,20q�55      ]� 57   
   $��40��6   $	��350��58      .+�5:      �� -   5        	 ��Name$	 	SearchBox��5:      $	��300��5;      $��50,0,0,0q�5<      $��Center=�5=      $��Center=�5>      $��Center=�5?      
 �� AcceptsReturn
 .  $
��Thin"�5A      $��20(�5B      ��5C      �� *SearchString+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��5E   K    J� InputBindings 5G      �� 5H       �� CommandParameter$! Views/Pages/SearchPage.xaml��5I   !   
 �� Key$ Return��5J   !    �� Command 5H   !   �� *SearchAndNavCommand+5J   !   .��5K   )   5M      �� 5O      $��Search��6   $!��Views/Pages/SearchPage.xaml��5Q      $��50��5R      ��5P      �� *SearchAndNavCommand+5R      5T      �� 5V   
   $��What is this?��6   $��SemiBold"�5W      $��Center=�5X      $��Center=�5Y      $	��130��5Z      $��35��5[      ���#008CDC$��5q�5]      
�������$��0,50,0,0q�5_      $ ��Views/Pages/AboutPage.xaml��5a      ��5`      �� *NavToPageCommand+��5b      �� 5c       �� 
TargetType" ��6   .�5d      ��  �� Property � �� �� Value �#008CDC63   �� 5e       � �� 5f      �� 5g       �� 
TargetType" ��6.   .R�5h   "   �� #��z��6)   #��z��5i   )   #��z��5j   )    �� CornerRadius$ 4��5k   )   ._�5l   &   �� $��Center=�67   $��Center=�5m   7   5n   #   5o      5q       �� Triggers 5r      P� 5s       P� Property �@J�IsMouseOver P� Value . . �5t      ��  � �� �#0094E46;   5u      5w      5y      5{      5}      
   M S B A M L   `   `   `  ����G  CMetaSyllabus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:MetaSyllabus.ViewModelsMetaSyllabus.ViewModels  5      7 clr-namespace:MetaSyllabus.ViewsMetaSyllabus.Views  5      V RGraphX.Controls, Version=2.0.2.0, Culture=neutral, PublicKeyToken=abeeef8557a9853fJ6clr-namespace:GraphX.Controls;assembly=GraphX.ControlsGraphX.Controls 5      =#clr-namespace:MetaSyllabus.GraphingMetaSyllabus.Graphing  5      A%clr-namespace:MetaSyllabus.ConvertersMetaSyllabus.Converters  5	      ,    &MetaSyllabus.Pages.ConceptListingsPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      4vm%clr-namespace:MetaSyllabus.ViewModels     5      .v clr-namespace:MetaSyllabus.Views     5      Egx6clr-namespace:GraphX.Controls;assembly=GraphX.Controls     5      1g#clr-namespace:MetaSyllabus.Graphing     5      6conv%clr-namespace:MetaSyllabus.Converters     5	        N� Title$  Concepts��5
       N� 	KeepAlive .  N� 
Background �#0094E4 � 	Resources 5      8   2MetaSyllabus.Converters.ViewModelToVertexConverter   ViewModelToVertexConverter��%   &	          5    
   6*   5!      .��5#      � $��5,5,5,5q�6    � RowDefinitions 5%   
   �� 5&      $��5* �6   $	��300��6(   �� 5'      $
��auto �6   �� 5(      $��3* �6   $	��100��6(   .+�5+   
   � $��0��6    � ColumnDefinitions 5-      �� 5.      $��7* �6#   $	��200��6.   �� 5/      $
��Auto �6#   �� 50      $	��13* �6#   $	��400��6/   .+�54      +   %MetaSyllabus.Views.AutoScrollListView  -   5          Name$ ConceptListing��55   #   $��0��54   #   $��Right=�56   #   $ �� IsSynchronizedWithCurrentItem$
 TrueY�58   #    �� SelectionMode$ Single=�59   #   $��Hidden=�5:   #   ��57   #   �� *ConceptViewModels+	 �� SelectedItem	 5;   #   �� *SelectedConcept+
 �� Mode$
 TwoWay=� �� View 5=      �� 5>      .P�5?      �� $��Concept Name��6)   )   #MetaSyllabus.Views.SortableGridView   PropertyName$
 Name��5A   )    �� DisplayMemberBinding 5@   )   �� *Name+ �� Width 5B   )   �� *ActualWidth+ �� ElementName$ 	AutoWidth��5D      ��5G      �� 5H       �� 
TargetType" ��6   .�5I      ��  �� Property �@��
IsSelected �� Value 5J   !   �� *
IsSelected+$
 TwoWay=�5L      5O      � 5R      $y�Hidden=�6    5S      �� 5T      $��* �6'   �� 5U      $��20 �6'   .+�5X      � -   5         �Name$ 	AutoWidth��5Y      $��0��5X      5Y      5Z      �� 5\      $��1��6   $��4��5]      $��Center=�5^      $��0,2,0,2q�5_      $��Stretch=�5`       �� ResizeBehavior$ PreviousAndNext=�5a       �� ResizeDirection$ Columns=�5b      ���#008CDC5c      �� 5f      $��2��6   
�������$��
20,20,20,0q�5h      .��5i      /   )MetaSyllabus.ViewModels.BindableTextBlock  $
��Wrap=�5k   '    � DataContext 5i   '   �� *SelectedConcept+   BindableInlines 5j   '   �� *
RichText+5k   '   5l      5n      �� 5p   
   $��4��6   $��1��5q      $��Stretch=�5r      $��2,0,2,0q�5s      $��Center=�5t      $ PreviousAndNext=�5u      $
 Rows=�5v      ���#008CDC5w      !  GraphX.Controls.ZoomControl  5z   
   -   5          Name$ gx_ZoomControl��5{      $��2��5z         MinZoom$ .8Q�5|         MaxZoom$	 1.4Q�5}      "   UseAbsoluteZoomOnMouseWheel . .��5�      ,   &MetaSyllabus.Graphing.ConceptGraphArea  �   �MetaSyllabus.Graphing.GraphArea`1[[MetaSyllabus.ViewModels.ConceptViewModel, MetaSyllabus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]   SelectedVertex 6!   �� *SelectedConcept+ �� 	Converter# [  $
 TwoWay=�5�      5�      5�      
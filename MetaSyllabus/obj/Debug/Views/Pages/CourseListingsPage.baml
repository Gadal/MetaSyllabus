   M S B A M L   `   `   `  ����G  CMetaSyllabus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:MetaSyllabus.ViewModelsMetaSyllabus.ViewModels  5      7 clr-namespace:MetaSyllabus.ViewsMetaSyllabus.Views  5      V RGraphX.Controls, Version=2.0.2.0, Culture=neutral, PublicKeyToken=abeeef8557a9853fJ6clr-namespace:GraphX.Controls;assembly=GraphX.ControlsGraphX.Controls 5      =#clr-namespace:MetaSyllabus.GraphingMetaSyllabus.Graphing  5	      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5
      ?$clr-namespace:MetaSyllabus.BehaviorsMetaSyllabus.Behaviors  5      A%clr-namespace:MetaSyllabus.ConvertersMetaSyllabus.Converters  5      +    %MetaSyllabus.Pages.CourseListingsPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4vm%clr-namespace:MetaSyllabus.ViewModels     5      .v clr-namespace:MetaSyllabus.Views     5      Egx6clr-namespace:GraphX.Controls;assembly=GraphX.Controls     5      1g#clr-namespace:MetaSyllabus.Graphing     5	      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5
      2b$clr-namespace:MetaSyllabus.Behaviors     5      6conv%clr-namespace:MetaSyllabus.Converters     5        N� Title$  Courses��5       N� 	KeepAlive .  N� 
Background �#0094E4 � 	Resources 5"      8   2MetaSyllabus.Converters.ViewModelToVertexConverter   ViewModelToVertexConverter  HierarchicalStyle �� 
TargetType �� Property �� Value �� Mode �� Triggers	 P� Property
 P� Value��%�  &	        &	        5$   
   6*   �� 5&   
   " Q�5'      .�5(      ��  �@Q�
IsExpanded 5)      �� *
IsExpanded+$ TwoWay=��� 5+       �@Q�
IsSelected 5,      �� *
IsSelected+$ TwoWay=��� 5.       � ��$ Normal"�5/       51      P� 52      	 �@Q�
IsSelected
 . . �54      ��  � ��$
 Bold"�55      56      58      59      .��5;      � $��5,5,5,5q�6    � RowDefinitions 5=   
   �� 5>      $��5* �6   �� 5?      $
��Auto �6   �� 5@      $��3* �6   .+�5C   
   � $��0��6    � ColumnDefinitions 5E      �� 5F      $��9* �6#   $	��200��6.   �� 5G      $
��Auto �6#   �� 5H      $	��11* �6#   $	��400��6/   .+�5L      R� -   5         R�Name$ CourseListingsTree��5P      $��0��5L      $��
20,20,20,0q�5M      #��[ 5O      ��5N      �� *
CourseTree+.  (System.Windows.Interactivity.Interaction   	Behaviors 5R      9   3MetaSyllabus.Behaviors.BindableSelectedItemBehavior  5S         SelectedItem 65   �� *SelectedCourse+$ TwoWay=� 5V       �� DataType)   #MetaSyllabus.ViewModels.Institution �� ItemsSource%   MetaSyllabus.ViewModels.Faculty(   "MetaSyllabus.ViewModels.Department-   'MetaSyllabus.ViewModels.CourseViewModel��%�  (��       5X   /   *, +)(�� i     5]   /   *, +)(�� �     5b   /   *, +)(�� D    5g   #   *, +)�� 5X      "  6/    5Y   /   �� *	Faculties+.R�5Z      �� ��6$   �� *Name+5[      �� 5]      "  6/    5^   /   �� *Departments+.R�5_      �� ��6$   �� *Name+5`      �� 5b      "  6/    5c   /   �� *CourseViewModels+.R�5d      �� ��6$   �� *Name+5e      �� 5g      "  6#   .R�5h      �� ��6$   �� *Title+5i      5k      5m      �� 5o      $��1��6   $��4��5p      $��Center=�5q      $��0,2,0,2q�5r      $��Stretch=�5s       �� ResizeBehavior$ PreviousAndNext=�5t       �� ResizeDirection$ Columns=�5u      ���#008CDC5v      �� 5y      $��2��6   
�������$��
20,20,20,0q�5{      .��5|      /   )MetaSyllabus.ViewModels.BindableTextBlock  $
��Wrap=�5~   '    � DataContext 5|   '   �� *SelectedCourse+   BindableInlines 5}   '   �� *
RichText+5~   '   5      5�      �� 5�   
   $��4��6   $��1��5�      $��Stretch=�5�      $��2,0,2,0q�5�      $��Center=�5�      $ PreviousAndNext=�5�      $
 Rows=�5�      ���#008CDC5�      !	  GraphX.Controls.ZoomControl	  5�   
   -   5         	 Name$ gx_ZoomControl��5�      $��2��5�       	  MinZoom$ .8Q�5�       	  MaxZoom$	 1.4Q�5�      " 	  UseAbsoluteZoomOnMouseWheel . .��5�      +
   %MetaSyllabus.Graphing.CourseGraphArea
  �   �MetaSyllabus.Graphing.GraphArea`1[[MetaSyllabus.ViewModels.CourseViewModel, MetaSyllabus, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]   SelectedVertex 6    �� *SelectedCourse+ �� 	Converter# [  $ TwoWay=�5�      5�      5�      
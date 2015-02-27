using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

//TODO: Improve search performance so that this dependency isn't necessary.
using MetaSyllabus.ViewModels;

namespace MetaSyllabus.Behaviors
{
    // TreeViews don't support binding to the SelectedItem property by default.
    // This attached behaviour exposes SelectedItem as a dependency property for binding 
    // and is compatible with TwoWay bindings, HierarchicalDataTemplates, and virtualization.

    /// <summary>
    /// Behavior that exposes the SelectedItem property of a TreeView to binding.
    /// Supports TwoWay bindinds.
    /// </summary>
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region Properties

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(BindableSelectedItemBehavior),
            new UIPropertyMetadata(null, OnSelectedItemChanged));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion // Properties

        #region Private/Protected Methods

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        /// <summary>
        /// Recursively search this subtree for a given item.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl.  This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// Returns null if item is unfound.
        /// </returns>
        private static TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container == null || container.DataContext == null) { return null; }

            if (container.DataContext.GetType() == typeof(ApplicationViewModel))
            {
                if (!(item is CourseViewModel)) { return null; }
                return GetCourseTreeViewItem(container, item as CourseViewModel);
            }

            if (container.DataContext == item)
            {
                return container as TreeViewItem;
            }
                
            if (container is TreeViewItem && ((TreeViewItem)container).IsExpanded)
            {
                container.SetValue(TreeViewItem.IsExpandedProperty, true);
            }

            // Try generating the ItemsPresenter and the ItemsPanel by calling ApplyTemplate.
            // Note that if the TreeViewItem is virtualized, even if the item is marked as
            // expanded, we need to do this step to regenerate the visuals because they
            // may be virtualized away.
            container.ApplyTemplate();
            ItemsPresenter itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);
            if (itemsPresenter != null) 
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                itemsPresenter = GetVisualDescendant<ItemsPresenter>(container);
                if (itemsPresenter == null)
                {
                    container.UpdateLayout();
                    itemsPresenter = GetVisualDescendant<ItemsPresenter>(container);
                }
            }

            Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

            // Creates the generator for this panel as a side-effect.
            UIElementCollection children = itemsHostPanel.Children;

            for (int i = 0; i < container.Items.Count; i++)
            {
                TreeViewItem subContainer = (TreeViewItem)container.ItemContainerGenerator
                                                                   .ContainerFromIndex(i);

                subContainer.BringIntoView();

                TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                if (resultContainer != null) { return resultContainer; }

                // The object isn't under this TreeViewItem, so collapse it.
                subContainer.IsExpanded = false;
            }

            return null;
        }

        /// <summary>
        /// Search this subtree for a given CourseViewModel
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl.  This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// Returns null if item is unfound.</returns>
        /// <remarks>
        /// Being able to discard a branch by knowing which institution/faculty/department the course is in
        /// makes the search much, much quicker.  Reimplementing TreeView to include a hashset between
        /// TreeViewItems and their associated view models might work (though that has some problems).
        /// </remarks>
        private static TreeViewItem GetCourseTreeViewItem(ItemsControl container, CourseViewModel item)
        {
            if (container == null) { return null; }

            if (container is TreeViewItem && ((TreeViewItem)container).IsExpanded)
            {
                container.SetValue(TreeViewItem.IsExpandedProperty, true);
            }

            // Try generating the ItemsPresenter and the ItemsPanel by calling ApplyTemplate.
            // Note that if the TreeViewItem is virtualized, even if the item is marked as
            // expanded, we need to do this step to regenerate the visuals because they
            // may be virtualized away.
            container.ApplyTemplate();
            ItemsPresenter itemsPresenter = (ItemsPresenter)container.Template.FindName("ItemsHost", container);
            if (itemsPresenter != null)
            {
                itemsPresenter.ApplyTemplate();
            }
            else
            {
                itemsPresenter = GetVisualDescendant<ItemsPresenter>(container);
                if (itemsPresenter == null)
                {
                    container.UpdateLayout();
                    itemsPresenter = GetVisualDescendant<ItemsPresenter>(container);
                }
            }

            Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);
            UIElementCollection children = itemsHostPanel.Children;

            for (int i = 0; i < container.Items.Count; i++)
            {
                TreeViewItem subContainer = (TreeViewItem)container.ItemContainerGenerator
                                                                   .ContainerFromIndex(i);

                subContainer.BringIntoView();

                if (subContainer.DataContext is Institution)
                {
                    Institution institution = (Institution)subContainer.DataContext;

                    if (!String.Equals(institution.Name, item.InstitutionName)) 
                    { 
                        subContainer.IsExpanded = false;
                        continue;
                    }

                    return GetCourseTreeViewItem(subContainer, item);
                }
                
                if (subContainer.DataContext is Faculty)
                {
                    Faculty faculty = (Faculty)subContainer.DataContext;
                    if (!String.Equals(faculty.Name, item.FacultyName))
                    {
                        subContainer.IsExpanded = false;
                        continue;
                    }

                    return GetCourseTreeViewItem(subContainer, item);
                }

                if (subContainer.DataContext is Department)
                {
                    Department department = (Department)subContainer.DataContext;
                    if (!String.Equals(department.Name, item.DepartmentName))
                    {
                        subContainer.IsExpanded = false;
                        continue;
                    }

                    return GetCourseTreeViewItem(subContainer, item);
                }

                if (subContainer.DataContext is CourseViewModel)
                {
                    CourseViewModel course = (CourseViewModel)subContainer.DataContext;
                    if (course != item)
                    {
                        continue;
                    }

                    return subContainer;
                }
            }

            return null;
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem newItem = e.NewValue as TreeViewItem;
            if (newItem != null)
            {
                newItem.SetValue(TreeViewItem.IsSelectedProperty, true);
                return;
            }

            BindableSelectedItemBehavior behavior = (BindableSelectedItemBehavior)sender;
            if (behavior == null) { return; }

            TreeView treeView = behavior.AssociatedObject;
            if (treeView == null) { return; }

            newItem = GetTreeViewItem(treeView, e.NewValue);
            if (newItem  == null) { return; }

            newItem.IsSelected = true;
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }

        /// <summary>
        /// Run a depth-first search on the visual tree of a Visual for an object of type T.
        /// </summary>
        /// <returns>
        /// Returns the first descendant of visual that has type T, or null if none is found.
        /// </returns>
        private static T GetVisualDescendant<T>(Visual visual)
            where T : Visual
        {
            int childCount = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < childCount; i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);

                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    T descendant = GetVisualDescendant<T>(child);
                    if (descendant != null) 
                    {
                        return descendant;
                    }
                }
            }

            return null;
        }

        #endregion // Private/Protected Methods
    }
}

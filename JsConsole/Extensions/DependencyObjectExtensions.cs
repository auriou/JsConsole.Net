using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace JsConsole.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static void AddPropertyChangedHandler(this DependencyObject obj, DependencyProperty property,
            EventHandler handler)
        {
            DependencyPropertyDescriptor.FromProperty(property, obj.GetType()).AddValueChanged(obj, handler);
        }

        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while ((parent != null) && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (parent as T);
        }

        public static DependencyObject FindAncestor(this DependencyObject obj, Type ancestorType)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while ((parent != null) && !ancestorType.IsAssignableFrom(parent.GetType()))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : DependencyObject
        {
            Queue<DependencyObject> iteratorVariable0 = new Queue<DependencyObject>(obj.GetVisualChildren());
            while (iteratorVariable0.Count > 0)
            {
                DependencyObject iteratorVariable1 = iteratorVariable0.Dequeue();
                if (iteratorVariable1 is T)
                {
                    yield return (T) iteratorVariable1;
                }
                foreach (DependencyObject obj2 in iteratorVariable1.GetVisualChildren())
                {
                    iteratorVariable0.Enqueue(obj2);
                }
            }
        }

        public static IEnumerable<DependencyObject> FindDescendants(this DependencyObject obj, Type descendantType)
        {
            Queue<DependencyObject> iteratorVariable0 = new Queue<DependencyObject>(obj.GetVisualChildren());
            while (iteratorVariable0.Count > 0)
            {
                DependencyObject iteratorVariable1 = iteratorVariable0.Dequeue();
                if (descendantType.IsAssignableFrom(iteratorVariable1.GetType()))
                {
                    yield return iteratorVariable1;
                }
                foreach (DependencyObject obj2 in iteratorVariable1.GetVisualChildren())
                {
                    iteratorVariable0.Enqueue(obj2);
                }
            }
        }

        public static IEnumerable<DependencyObject> GetLogicalChildren(this DependencyObject obj)
        {
            return LogicalTreeHelper.GetChildren(obj).Cast<DependencyObject>();
        }

        public static DependencyObject GetLogicalParent(this DependencyObject obj)
        {
            return LogicalTreeHelper.GetParent(obj);
        }

        public static T GetValue<T>(this DependencyObject obj, DependencyProperty dp)
        {
            return (T) obj.GetValue(dp);
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject obj)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            int childIndex = 0;
            while (true)
            {
                if (childIndex >= childrenCount)
                {
                    yield break;
                }
                yield return VisualTreeHelper.GetChild(obj, childIndex);
                childIndex++;
            }
        }

        public static DependencyObject GetVisualParent(this DependencyObject obj)
        {
            return VisualTreeHelper.GetParent(obj);
        }


        public static void RemovePropertyChangedHandler(this DependencyObject obj, DependencyProperty property,
            EventHandler handler)
        {
            DependencyPropertyDescriptor.FromProperty(property, obj.GetType()).RemoveValueChanged(obj, handler);
        }
    }
}
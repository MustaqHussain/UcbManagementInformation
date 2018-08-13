// <copyright file="Properties.cs" company="Microsoft Corporation">
// Copyright (c) 2008 All Right Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2009-03-21</date>
// <summary>Virtual Earth Map Layer items and template binding</summary>

namespace UcbManagementInformation.Maps
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using Microsoft.Maps.MapControl;
    using System.Windows.Controls;

    /// <summary>
    /// Supplementary Attached Properties for the Virtual Earth Map Control
    /// </summary>
    /// <seealso cref="Microsoft.VirtualEarth.MapControl.Map"/>
    /// <seealso cref="Microsoft.VirtualEarth.MapControl.MapLayer"/>
    /// <example>
    /// Example of the using the CenterPoint, ItemsSource and ItemTemplate dependency properties and a DataTemplate
    ///     <m:Map Grid.Column="1"
    ///         Mode="AerialWithLabels" 
    ///         x:Name="Map" 
    ///         ve:Properties.CenterPoint="{Binding SelectedStore, Source={StaticResource ViewModel}, Converter={StaticResource StoreToLocationConverter}}"
    ///         xmlns:m="clr-namespace:Microsoft.VirtualEarth.MapControl;assembly=Microsoft.VirtualEarth.MapControl"
    ///         xmlns:ve="clr-namespace:Synergist.VE"
    ///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    ///         <m:Map.Resources>
    ///             <DataTemplate x:Key="BestBuyShape">
    ///                 <Image Stretch="None" 
    ///                        ToolTipService.ToolTip="{Binding Name}"
    ///                        Source="{Binding Logo, Source={StaticResource StoresDS}}" 
    ///                        m:MapLayer.MapPosition="{Binding Converter={StaticResource StoreToLocationConverter}}"
    ///                        m:MapLayer.MapPositionMethod="BottomLeft"/>
    ///                </DataTemplate>
    ///         </m:Map.Resources>
    ///         <m:Map.Children>
    ///             <m:MapLayer x:Name="BestBuyLocations" 
    ///                        ve:Properties.ItemsSource="{Binding Items, Source={StaticResource StoresDS}}"
    ///                        ve:Properties.ItemTemplate="{StaticResource BestBuyShape}"/>
    ///         </m:Map.Children>
    ///     </m:Map>
    /// </example>
    public sealed class Properties
    {
        #region Fields
        /// <summary>
        /// The ItemsSource attached property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(Properties), 
            new PropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));

        /// <summary>
        /// The ItemTemplate attached property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.RegisterAttached(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(Properties),
            new PropertyMetadata(null));

        /// <summary>
        /// The CenterPoint Property (Map.Center is not a dependency property so I've created my own)
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.RegisterAttached(
            "CenterPoint",
            typeof(Location),
            typeof(Properties),
            new PropertyMetadata(new PropertyChangedCallback(OnCenterPointChanged)));

        #endregion

        /// <summary>
        /// Prevents a default instance of the Properties class from being created
        /// </summary>
        private Properties()
        {
        }

        #region Methods
        /// <summary>
        /// Sets the items source for a Virtual Earth MapLayer
        /// </summary>
        /// <param name="element">a MapLayer</param>
        /// <param name="value">the enumerable collection</param>
        public static void SetItemsSource(DependencyObject element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets the items source for a Virtual Earth MapLayer
        /// </summary>
        /// <param name="element">a MapLayer</param>
        /// <returns>the enumerable collection</returns>
        public static IEnumerable GetItemsSource(DependencyObject element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        /// <summary>
        /// Sets the item template for a Virtual Earth MapLayer
        /// </summary>
        /// <param name="element">a MapLayer</param>
        /// <param name="value">the Item DataTemplate</param>
        public static void SetItemTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(ItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets the item template for a Virtual Earth MapLayer
        /// </summary>
        /// <param name="element">a MapLayer</param>
        /// <returns>the DataTemplate for the items</returns>
        public static DataTemplate GetItemTemplate(DependencyObject element)
        {
            return (DataTemplate) element.GetValue(ItemTemplateProperty);
        }

        /// <summary>
        /// Sets the center point for a Virtual Earth Map
        /// </summary>
        /// <param name="element">the Virtual Earth Map</param>
        /// <param name="value">the new Center Point location</param>
        /// <remarks>When attached to a Virtual Earth Map, this is will set the Map.Center</remarks>
        public static void SetCenterPoint(UIElement element, Location value)
        {
            var map = element as Map;

            if (map == null)
            {
                element.SetValue(CenterPointProperty, value);
            }
            else
            {
                map.Center = value;
            }
        }

        /// <summary>
        /// Gets the center point for a Virtual Earth Map
        /// </summary>
        /// <param name="element">the Virtual Earth Map</param>
        /// <returns>the Center Point Location</returns>
        /// <remarks>When attached to a Virtual Earth Map, this is will return the Map.Center</remarks>
        public static Location GetCenterPoint(UIElement element)
        {
            var map = element as Map;

            if (map == null)
            {
                return (Location)element.GetValue(CenterPointProperty);
            }

            return map.Center;
        }

        #endregion

        #region Implementation
        /// <summary>
        /// When the Items Source changes add the items and if the collection supports INotifyCollectionChanged, add a collection changed event handler
        /// </summary>
        /// <param name="depObject">the MapLayer</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnItemsSourceChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            var layer = depObject as MapLayer;

            if (layer == null)
            {
                return;
            }

            var uiElement = depObject as UIElement;

            var itemsSource = args.NewValue as IEnumerable;

            var notifyCollectionChanged = itemsSource as INotifyCollectionChanged;

            AddItemToLayer(layer, uiElement, itemsSource);

            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(delegate(object sender, NotifyCollectionChangedEventArgs e)
                {
                    AddItemToLayer(layer, uiElement, e.NewItems);

                    RemoveItems(layer, e.OldItems);
                });
            }
        }

        private static void RemoveItems(MapLayer layer, IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                var found = (from child in layer.Children.Cast<FrameworkElement>()
                             where child.DataContext == item
                             select child).FirstOrDefault();
                
                if (found != null)
                {
                    layer.Children.Remove(found);
                }
            }
        }

        /// <summary>
        /// Add an item to the collection
        /// </summary>
        /// <param name="layer">the map layer</param>
        /// <param name="uiElement">the UI template</param>
        /// <param name="items">the items to add</param>
        private static void AddItemToLayer(MapLayer layer, UIElement uiElement, IEnumerable items)
        {
            if (items == null)
            {
                return;
            }

            var template = GetItemTemplate(uiElement);

            foreach (var item in items)
            {
                var newShape = template.LoadContent() as FrameworkElement;

                newShape.DataContext = item;

                layer.Children.Add(newShape);
            }
        }

        /// <summary>
        /// When the CenterPoint changes change the Map Center
        /// </summary>
        /// <param name="depObject">a VirtualEarth Map</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnCenterPointChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs args)
        {
            var map = depObject as Map;

            if (map == null)
            {
                return;
            }

            Location newCenterPoint = (Location)args.NewValue;

            map.AnimationLevel = AnimationLevel.Full;

            map.Center = newCenterPoint;
        }

        #endregion
    }
}

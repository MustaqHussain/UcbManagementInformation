/* 
 * Copyright (c) 2010, Andriy Syrov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided 
 * that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the 
 * following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
 * the following disclaimer in the documentation and/or other materials provided with the distribution.
 *
 * Neither the name of Andriy Syrov nor the names of his contributors may be used to endorse or promote 
 * products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY 
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN 
 * IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 *   
 */
using System;
using System.Linq;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimelineLibrary
{
    /// 
    /// <summary>
    /// This class privides mapping between screen coordinates/sizes and time and 
    /// timeline event placement</summary>
    /// 
    public class TimelineBuilder
    {
        private const double                            TOP_MARGIN = 5;

        // we need this margin so that events do not run over date/time column titles
        private const double                            BOTTOM_MARGIN = 5; 

        private const int                               MIN_EVENT_ZINDEX = 10;

        private const int                               ZINDEX_INC = 1000;

        #region Private Fields

        private int                                     m_columnCount;
        private DateTime                                m_currDate;
        private TimelineCalendar                        m_timeline;
        private Dictionary<object, long>                m_columnIndexes;
        private Dictionary<object, long>                m_columnTextIndexes;
        private long                                    m_minIndex;
        private long                                    m_maxIndex;
        private List<TimelineEvent>                     m_events;
        private Dictionary<TimelineEvent, object>       m_visibleEvents;
        private double                                  m_maxEventHeight;
        private Dictionary<TimelineEvent, TimelineDisplayEvent>
                                                        m_dispEvents;
        private bool                                    m_assignRows;
        private Canvas                                  m_canvas;            
        private DataTemplate                            m_template;
        private DataTemplate                            m_textTemplate;
        private DataTemplate                            m_eventTemplate;
        private TimelineBand                            m_parent;
        private Dictionary<TimelineEvent, TimelineDisplayEvent> 
                                                        m_savedDispEvents;

        #endregion

        public TimelineBuilder(
            TimelineBand                                band,
            Canvas                                      canvas,
            DataTemplate                                template,
            DataTemplate                                textTemplate,
            int                                         columnCount,
            TimelineCalendar                            timeline,
            DataTemplate                                eventTemplate,
            double                                      maxEventHeight, 
            bool                                        assignRows,
            DateTime                                    currDateTime
        )
        {
            Debug.Assert(template != null);
            Debug.Assert(canvas != null);
            Debug.Assert(eventTemplate != null);
            Debug.Assert(band != null);
            Debug.Assert(columnCount > 0);
            Debug.Assert(timeline != null);
            Debug.Assert(maxEventHeight > 0);

            m_parent = band;
            m_eventTemplate = eventTemplate;
            m_canvas = canvas;
            m_template = template;;
            m_columnCount = columnCount;
            m_timeline = timeline;
            m_assignRows = assignRows;
            m_textTemplate = textTemplate;
            
            m_visibleEvents = new Dictionary<TimelineEvent, object>();
            m_dispEvents = new Dictionary<TimelineEvent, TimelineDisplayEvent>();
            m_maxEventHeight = maxEventHeight;

            CurrentDateTime = currDateTime;

            Utilities.Trace(this);
        }

        public TimelineCalendar Calendar
        {
            get
            {
                return m_timeline;
            }
        }

        public TimeSpan VisibleWindowSize
        {
            get;
            set;
        }

        /// 
        /// <summary>
        /// This method used by Utilities.Trace</summary>
        /// 
        public override string ToString(
        )
        {
            return (m_timeline == null ? String.Empty : m_timeline.LineType.ToString());
        }


        public int ColumnCount
        {
            get
            {
                return m_columnCount;
            }
            set
            {
                m_columnCount = value;
            }
        }

        /// 
        /// <summary>
        /// This calculates row for each of event (makes sence only for main band).
        /// We try to place evens so that the do not overlap on the screen.</summary>
        /// 
        public void CalculateEventRows(
        )
        {
            int                                         count;
            int                                         rowCount;
            int                                         minIndex;
            DateTime[]                                  dates;
            DateTime                                    minDate;
            TimelineEvent                               e;
            int                                         overrideRowCount;

            Debug.Assert(m_assignRows);
            Debug.Assert(m_events != null);

            Utilities.Trace(this);

            rowCount = (int) ((PixelHeight - TOP_MARGIN - BOTTOM_MARGIN) / m_maxEventHeight);

            if (m_events.Count > 0)
            {
                overrideRowCount = m_events.Max((s) => { return s.RowOverride; }) + 1;
                rowCount = Math.Max(rowCount, overrideRowCount);
            }

            if (rowCount == 0)
            {
                for (int i = 0; i < m_events.Count; ++i)
                {
                    e = m_events[i];
                    e.Row = 0;
                }
            }
            else 
            {
                dates = new DateTime[rowCount];

                for (int i = 0; i < rowCount; ++i)
                {
                    dates[i] = new DateTime();
                }

                m_events.Sort((i1, i2) => i1.EndDate < i2.EndDate ? -1 : 
                                            (i1.EndDate == i2.EndDate ? 0 : 1));

                count = m_events.Count;

                for (int i = 0; i < count; ++i)
                {
                    minDate = dates[0];    
                    minIndex = 0;
                    e = m_events[i];

                    if (e.RowOverride == -1)
                    {
                        for (int k = 0; k < rowCount; ++k)
                        {
                            if (minDate > dates[k])
                            {
                                minIndex = k;
                                minDate = dates[k];
                            }
                        }

                        dates[minIndex] = e.EndDate;
                        e.Row = minIndex;
                    }
                    else
                    {
                        e.Row = e.RowOverride;
                        dates[e.Row] = e.EndDate;
                    }
                }
            }
        }


        /// 
        /// <summary>
        /// Calculate top position and width of each event</summary>
        /// 
        public void CalculateEventPositions(
        )
        {
            TimelineDisplayEvent                        pos;

            Debug.Assert(m_events != null);
            
            if (m_dispEvents == null)
            {
                return;
            }

            Utilities.Trace(this);
            
            foreach (TimelineEvent e in m_events)
            {
                if (!m_dispEvents.ContainsKey(e))
                {
                    pos = new TimelineDisplayEvent(e, m_parent.TimelineTray, this);
                    if (m_savedDispEvents != null && m_savedDispEvents.ContainsKey(e))
                    {
                        pos.Selected = m_savedDispEvents[e].Selected;
                        if (pos.Selected && !string.IsNullOrEmpty(TimelineDisplayEvent.SelectedEventImageUrl))
                        {
                            m_savedDispEvents[e].Event.TeaserEventImage = e.TeaserEventImage;
                            e.TeaserEventImage = TimelineDisplayEvent.SelectedEventImageUrl;
                        }
                        else if (!pos.Selected)
                        {
                            e.TeaserEventImage = m_savedDispEvents[e].Event.TeaserEventImage;
                        }
                    }
                    m_dispEvents.Add(e, pos);
                }
                else
                {
                    pos = m_dispEvents[e];
                }

                Debug.Assert(e.Row >= 0, "Need first call CalculateEventRows for main band");

                if (pos.Top == -1 || pos.TimelineTray.RecalculateEventTopPosition)
                {
                    pos.SetCalculatedTop(e.Row * m_maxEventHeight + TOP_MARGIN);
                }

                pos.ActualEventPixelWidth = TimeSpanToPixels(e.EndDate - e.StartDate); 
                pos.ActualEventPixelWidth = Math.Max(3.0, pos.ActualEventPixelWidth);

                pos.EventPixelWidth = Math.Min(PixelWidth * 2, pos.ActualEventPixelWidth);
            }
        }

        /// 
        /// <summary>
        /// Display all visible events and remove all invisible for current visible time window</summary>
        /// 
        public void DisplayEvents(
            bool                                        dateChanged = false
        )
        {
            DateTime                                    begin;
            DateTime                                    end;
            double                                      distance;
            double                                      x;
            TimelineDisplayEvent                        pos;
            double                                      viewSize = 0;

            if (m_timeline.IsValidIndex(m_minIndex))
            {
                begin = m_timeline.GetFloorTime(m_timeline[m_minIndex]);
            }
            else
            {
                begin = DateTime.MinValue;
            }

            if (m_timeline.IsValidIndex(m_maxIndex))
            {
                end = m_timeline.GetCeilingTime(m_timeline[m_maxIndex]);
            }
            else
            {
                end = DateTime.MaxValue;
            }

            if (m_events != null)
            {
                foreach (TimelineEvent e in m_events)
                {
                    pos = m_dispEvents[e];
                        
                    distance = TimeSpanToPixels(CurrentDateTime - e.StartDate);
                    x = PixelWidth / 2 - distance;

                    if (!((e.StartDate < begin && e.EndDate < begin) || 
                          (e.StartDate > end && e.EndDate > end)) || 
                          (x + pos.DescriptionWidth >= 0 && x <= m_canvas.ActualWidth))
                    {
                        if (pos.EventPixelWidth != pos.ActualEventPixelWidth)
                        {
                            if (x + pos.ActualEventPixelWidth < PixelWidth)
                            {
                                x = x + pos.ActualEventPixelWidth - PixelWidth * 2;
                            }
                            else if (x < -PixelWidth && x + pos.ActualEventPixelWidth > PixelWidth)
                            {
                                x = Math.Max(-PixelWidth, x);
                            }
                        }

                        if (!m_visibleEvents.ContainsKey(e))
                        {
                            m_visibleEvents.Add(e, CreateEvent(e, m_dispEvents[e]));
                        }

                        viewSize = Math.Max(viewSize, 
                                MoveEvent(m_visibleEvents[e], 
                                  m_dispEvents[e].TopWithoutOffset, 
                                  x, 
                                  pos.EventPixelWidth));
                    }
                    else
                    {
                        if (m_visibleEvents.ContainsKey(e))
                        {
                            RemoveEvent(m_visibleEvents[e]);
                            m_visibleEvents.Remove(e);
                        }
                    }
                }
            }                    
            if (m_parent.IsMainBand)
            {
                m_parent.TimelineTray.FireScrollViewChanged(viewSize);
            }
        }

        /// 
        /// <summary>
        /// Remove all events from timeline screen</summary>
        /// 
        public void ClearEvents(
        )
        {
            if (m_visibleEvents != null)
            {
                foreach (object e in m_visibleEvents.Values)
                {
                    RemoveEvent(e);
                }
            }

            m_visibleEvents = new Dictionary<TimelineEvent, object>();
            m_savedDispEvents = m_dispEvents;
            m_dispEvents = new Dictionary<TimelineEvent,TimelineDisplayEvent>();
            m_events = new List<TimelineEvent>();
        }

        /// 
        /// <summary>
        /// Removes all columns from timelineband screen</summary>
        /// 
        public void ClearColumns(
        )
        {
            if (Columns != null)
            {
                foreach (object obj in Columns.Keys)
                {
                    RemoveColumn(obj);
                }
            }

            if (ColumnTexts != null)
            {
                foreach (object obj in ColumnTexts.Keys)
                {
                    RemoveColumnText(obj);
                }
            }
        }

        #region Properties

        public double PixelWidth
        {
            get
            {
                return m_canvas.ActualWidth;
            }
        }

        public double PixelHeight
        {
            get
            {
                return m_canvas.ActualHeight;
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                return m_currDate;
            }
            set
            {
                if (value < m_timeline.MinDateTime)
                {
                    m_currDate = m_timeline.MinDateTime;
                }
                else if (value > m_timeline.MaxDateTime)
                {
                    m_currDate = m_timeline.MaxDateTime;
                }
                else
                {
                    m_currDate = value;
                }
            }
        }

        public double ColumnPixelWidth
        {
            get
            {
                return PixelWidth / m_columnCount;
            }
        }

        public TimeSpan ColumnTimeWidth
        {
            get
            {
                DateTime                                begin;
                DateTime                                end;
                DateTime                                date;
                TimeSpan                                tick;

                date = CurrentDateTime;
                tick = new TimeSpan(1L);

                if (date == m_timeline.MaxDateTime)
                {
                    date = m_timeline.GetFloorTime(date) - tick;
                }
                else if (date == m_timeline.MinDateTime)
                {
                    date = m_timeline.GetCeilingTime(date) + tick;
                }

                end = m_timeline.GetFloorTime(date);
                begin = m_timeline.GetCeilingTime(date);

                return begin - end;
            }
        }

        public Dictionary<object, long> Columns
        {
            get
            {
                return m_columnIndexes;
            }
        }

        public Dictionary<object, long> ColumnTexts
        {
            get
            {
                return m_columnTextIndexes;
            }
        }

        public List<TimelineEvent> TimelineEvents
        {
            get
            {
                return m_events;
            }
            set
            {
                Debug.Assert(value != null);
                m_events = value;
            }
        }

        #endregion

        public object CreateColumn(
            double                                      left, 
            double                                      top, 
            double                                      width, 
            double                                      height, 
            string                                      content
        )
        {
            FrameworkElement                            el;

            Debug.Assert(top > -1);

            el = m_template.LoadContent() as FrameworkElement;

            el.DataContext = content;
            m_canvas.Children.Add(el);

            el.SetValue(Canvas.LeftProperty, left);
            el.SetValue(Canvas.TopProperty, 0.0);
            el.Width = ColumnPixelWidth + 2;
            el.Height = height;

            return el;
        }

        public object CreateColumnText(
            double                                      left, 
            double                                      top, 
            double                                      width, 
            double                                      height, 
            string                                      content
        )
        {
            FrameworkElement                            el = null;

            Debug.Assert(top > -1);

            if (m_textTemplate != null)
            {
                el = m_textTemplate.LoadContent() as FrameworkElement;

                el.DataContext = content;
                m_canvas.Children.Add(el);

                el.SetValue(Canvas.LeftProperty, left);

                el.SetValue(Canvas.TopProperty, height - 
                        Math.Max(el.ActualHeight, el.Height));

                el.Width = ColumnPixelWidth + 2;
            }

            return el;
        }


        public void MoveColumn(
            object                                      element,
            double                                      left,
            string                                      content
        )
        {
            FrameworkElement                            el;

            Debug.Assert(element != null);
            Debug.Assert(content != null);

            el = (FrameworkElement) element;

            if (((string) el.DataContext) != content)
            {
                el.DataContext = content;
            }
            el.SetValue(Canvas.LeftProperty, left);
        }

        public void MoveColumnText(
            object                                      element,
            double                                      left,
            string                                      content
        )
        {
            FrameworkElement                            el;

            Debug.Assert(element != null);
            Debug.Assert(content != null);

            el = (FrameworkElement) element;

            if (((string) el.DataContext) != content)
            {
                el.DataContext = content;
            }
            el.SetValue(Canvas.LeftProperty, left);
        }

        public object CreateEvent(
            TimelineEvent                               e,
            TimelineDisplayEvent                        de
        )
        {
            FrameworkElement                            element;
            DependencyObject                            obj;
            Button                                      link;

            Debug.Assert(e != null);

            obj = m_eventTemplate.LoadContent();
            element = (FrameworkElement) obj;
            element.MouseEnter += OnMouseEnter;
            element.MouseLeave += OnMouseLeave;
            element.DataContext = de;

            if (m_parent.IsMainBand)
            {
                element.MouseLeftButtonDown += OnLeftButtonDown;
            }

            element.SetValue(Canvas.ZIndexProperty, (e.Row + 1) * MIN_EVENT_ZINDEX);
            
            link = (Button) element.FindName("EventLinkTextBlock");

            if (link != null)
            {
                link.Click += m_parent.OnMoreInfoClick;
            }
            m_canvas.Children.Add(element);

            if (m_parent.IsMainBand)
            {
                m_parent.TimelineTray.FireEventCreated(element, de);
            }

            return element;
        }

        void OnLeftButtonDown(
            object                                      sender, 
            System.Windows.Input.MouseButtonEventArgs   e
        )
        {
            FrameworkElement                            fe;
            TimelineDisplayEvent                        de;

            fe = (sender as FrameworkElement);

            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                foreach (TimelineDisplayEvent ev in m_parent.Selection)
                {
                    ev.Selected = false;
                }
                m_parent.Selection.Clear();
            }

            de = fe.DataContext as TimelineDisplayEvent;
            de.Selected = !de.Selected;

            if (de.Selected)
            {
                m_parent.Selection.Add(de);
            }
            else
            {
                m_parent.Selection.Remove(de);
            }

            if (m_parent.OnSelectionChanged != null)
            {
                m_parent.OnSelectionChanged(this, EventArgs.Empty);
            }
        }

        void OnMouseLeave(
            object                                      sender, 
            System.Windows.Input.MouseEventArgs         e
        )
        {
            FrameworkElement                            el;
            int                                         index;

            el = (FrameworkElement) sender;

            if (!InertialTimelineScroll.Moving)
            {
                if ((el.DataContext as TimelineDisplayEvent).IsEventMouseOver)
                {
                    (el.DataContext as TimelineDisplayEvent).IsEventMouseOver = false;

                    index = (int) el.GetValue(Canvas.ZIndexProperty) - ZINDEX_INC;
                    if (index > MIN_EVENT_ZINDEX - 1)
                    {
                        el.SetValue(Canvas.ZIndexProperty, index);
                    }
                }
            }
        }

        private void OnMouseEnter(
            object                                      sender, 
            System.Windows.Input.MouseEventArgs         e
        )
        {
            FrameworkElement                            el;
            int                                         index;

            el = (FrameworkElement) sender;

            if (!InertialTimelineScroll.Moving)
            {
                if (!(el.DataContext as TimelineDisplayEvent).IsEventMouseOver)
                {
                    index = (int) el.GetValue(Canvas.ZIndexProperty);
                    if (index < MIN_EVENT_ZINDEX + ZINDEX_INC)
                    {
                        index += ZINDEX_INC;
                        (el.DataContext as TimelineDisplayEvent).IsEventMouseOver = true;
                        el.SetValue(Canvas.ZIndexProperty, index);
                    }
                }
            }
        }

        public double MoveEvent(
            object                                      eo,
            double                                      top,
            double                                      left,
            double                                      width
        )
        {
            FrameworkElement                            el;
            TimelineDisplayEvent                        de;

            el = eo as FrameworkElement;
            Debug.Assert(el != null);
            
            de = el.DataContext as TimelineDisplayEvent;

            if (left > -el.ActualWidth + 1 && left < m_canvas.ActualWidth + el.ActualWidth - 1)
            {
                if (m_parent.IsMainBand)
                {
                    m_parent.TimelineTray.FireOnEventVisible(el, (el.DataContext as TimelineDisplayEvent));
                }
            }
            SetTopWithScrollOffset(el, top, true);
            el.SetValue(Canvas.LeftProperty, left);

            return de.TopWithoutOffset + el.ActualHeight;
        }

        public void SetTopWithScrollOffset(
            FrameworkElement                            el, 
            double                                      top,
            bool                                        lockOverride
        )
        {
            if (lockOverride)
            {
                LockTopOverrideUpdate = true;
            }

            try
            {
                if (m_parent.IsMainBand)
                {
                    el.SetValue(Canvas.TopProperty, top - m_parent.CanvasScrollOffset);
                }
                else
                {
                    el.SetValue(Canvas.TopProperty, top);
                }
            }
            finally
            {
                if (lockOverride)
                {
                    LockTopOverrideUpdate = false;
                }
            }
        }

        public bool LockTopOverrideUpdate
        {
            get;
            set;
        }

        public void RemoveEvent(
            object                                      e
        )
        {
            UIElement                                   element;
            Button                                 link;

            element = e as UIElement;
            Debug.Assert(element != null);

            m_canvas.Children.Remove(e as UIElement);

            element.MouseEnter -= OnMouseEnter;
            element.MouseLeave -= OnMouseLeave;

            link = (Button) (element as FrameworkElement).FindName("EventLinkTextBlock");

            if (link != null)
            {
                link.Click -= m_parent.OnMoreInfoClick;
            }

            if (m_parent.IsMainBand)
            {
                m_parent.TimelineTray.FireEventDeleted(element as FrameworkElement, 
                    (element as FrameworkElement).DataContext as TimelineDisplayEvent);
            }
        }

        public void RemoveColumn(
            object                                      e
        )
        {
            m_canvas.Children.Remove(e as UIElement);
        }

        public void RemoveColumnText(
            object                                      e
        )
        {
            m_canvas.Children.Remove(e as UIElement);
        }

        private string GetDataContext(
            long                                        index
        )
        {
            if (m_timeline.IsValidIndex(index))
            {
                return TimelineCalendar.ItemToString(m_timeline, m_timeline[index]);
            }
            else
            {
                return string.Empty;
            }
        }

        /// 
        /// <summary>
        /// Build columns (one for each years, dates, etc.)</summary>
        /// 
        public void BuildColumns(
        )
        {
            double                                      step;
            double                                      width;
            double                                      height;
            object                                      el;
            double                                      x;
            FrameworkElement                            fe;

            Utilities.Trace(this);

            width = PixelWidth;
            height = PixelHeight;
            step = ColumnPixelWidth;


            if (m_columnIndexes != null)
            {
                foreach (object o in m_columnIndexes.Keys)
                {
                    fe = (FrameworkElement) o;
                    m_canvas.Children.Remove(fe);
                }
            }

            if (m_columnTextIndexes != null)
            {
                foreach (object o in m_columnTextIndexes.Keys)
                {
                    fe = (FrameworkElement) o;
                    m_canvas.Children.Remove(fe);
                }
            }

            m_columnIndexes = new Dictionary<object, long>();
            m_columnTextIndexes = new Dictionary<object,long>();
            
            for (int i = 0; i < m_columnCount + 2; ++i)
            {
                x = ColumnPixelWidth * (i - 1);

                el = CreateColumn(x, 0.0, step + 1, height, String.Empty);
                m_columnIndexes.Add(el, 0);

                el = CreateColumnText(x, 0.0, step + 1, height, String.Empty);

                if (m_columnTextIndexes != null && el != null)
                {
                    m_columnTextIndexes.Add(el, 0);
                }
            }

            FixPositions();
        }

        /// 
        /// <summary>
        /// Convert pixels to TimeSpan according to current timeline band type</summary>
        /// 
        public TimeSpan PixelsToTimeSpan(
            double                                      pixels
        )
        {
            long                                        val;
            TimeSpan                                    span;
            double                                      dval;

            dval = (pixels / ColumnPixelWidth) * ColumnTimeWidth.TotalMilliseconds;
            dval = dval * TimeSpan.TicksPerMillisecond;

            val = (long) Math.Round(dval, 0);
            span = new TimeSpan(val);

            return span;
        }

        /// 
        /// <summary>
        /// Convert TimeSpan to pixels according to current timeline band type</summary>
        /// 
        public double TimeSpanToPixels(
            TimeSpan                                    span
            )
        {
            TimeSpan                                    totalVisible;

            switch (m_timeline.LineType)
            {
                case TimelineCalendarType.Decades:
                    totalVisible = new TimeSpan(365 * 10 * m_columnCount, 0, 0, 0);
                    break;

                case TimelineCalendarType.Years:
                    totalVisible = new TimeSpan(365 * m_columnCount, 0, 0, 0);
                    break;

                case TimelineCalendarType.Months:
                    totalVisible = new TimeSpan(31 * m_columnCount, 0, 0, 0);
                    break;

                case TimelineCalendarType.Days:
                    totalVisible = new TimeSpan(m_columnCount, 0, 0, 0);
                    break;

                case TimelineCalendarType.Hours:
                    totalVisible = new TimeSpan(0, m_columnCount, 0, 0, 0);
                    break;

                case TimelineCalendarType.Minutes10:
                    totalVisible = new TimeSpan(0, 0, m_columnCount * 10, 0, 0);
                    break;
                
                case TimelineCalendarType.Minutes:
                    totalVisible = new TimeSpan(0, 0, m_columnCount, 0, 0);
                    break;

                case TimelineCalendarType.Seconds:
                    totalVisible = new TimeSpan(0, 0, 0, m_columnCount, 0);
                    break;

                case TimelineCalendarType.Milliseconds100:
                    totalVisible = new TimeSpan(0, 0, 0, 0, m_columnCount * 100);
                    break;                

                case TimelineCalendarType.Milliseconds10:
                    totalVisible = new TimeSpan(0, 0, 0, 0, m_columnCount * 10);
                    break;

                default: // TimelineCalendarType.Milliseconds:
                    totalVisible = new TimeSpan(0, 0, 0, 0, m_columnCount);
                    break;
            }

            return (PixelWidth * span.TotalMilliseconds) / totalVisible.TotalMilliseconds;
        }

        /// 
        /// <summary>
        /// Moves current timeline band for the passed timespan</summary>
        /// 
        public void TimeMove(
            TimeSpan                                    span
            )
        {
            CurrentDateTime -= span;
            FixPositions();
        }

        /// 
        /// <summary>
        /// After current data is changed this function fixes positions of all columns 
        /// and all events</summary>
        /// 
        private void FixPositions(
        )
        {
            double                                      width;
            double                                      height;
            object                                      el;
            double                                      x;
            double                                      ox;
            long                                        index;
            double                                      posOffset;
            object[]                                    elements;
            object[]                                    textElements = null;
            int                                         middle;
            DateTime                                    floor;
            DateTime                                    currDate;
            long                                        maxIndex;

            width = PixelWidth;
            height = PixelHeight;

            m_minIndex = 0;
            m_maxIndex = 0;

            currDate = CurrentDateTime;

            floor = m_timeline.GetFloorTime(currDate);
            posOffset = TimeSpanToPixels(currDate - floor);

            elements = new object[m_columnCount + 2];
            m_columnIndexes.Keys.CopyTo(elements, 0);

            textElements = new object[m_columnCount + 2];
            m_columnTextIndexes.Keys.CopyTo(textElements, 0); 

            middle = m_columnCount / 2 + 1;
            ox = width / 2 - posOffset;
            index = m_timeline.IndexOf(currDate);
            x = ox;
            maxIndex = m_timeline.IndexOf(m_timeline.MaxDateTime);

            for (int i = middle; i < m_columnCount + 2; ++i)
            {
                el = elements[i];
                MoveColumn(el, x, GetDataContext(index));
                m_columnIndexes[el] = Math.Min(maxIndex, index);

                el = textElements[i];

                if (el != null)
                {
                    MoveColumnText(el, x, GetDataContext(index));
                    m_columnTextIndexes[el] = Math.Min(maxIndex, index);
                }

                x += ColumnPixelWidth;

                if (index != int.MaxValue)
                {
                    ++index;
                }
            }

            m_maxIndex = index;

            index = m_timeline.IndexOf(currDate) - 1;
            x = ox - ColumnPixelWidth;

            for (int i = middle - 1; i >= 0; --i, --index)
            {
                el = elements[i];
                MoveColumn(el, x, GetDataContext(index));
                m_columnIndexes[el] = Math.Max(-1, index);

                el = textElements[i];

                if (el != null)
                {
                    MoveColumnText(el, x, GetDataContext(index));
                    m_columnTextIndexes[el] = Math.Min(-1, index);
                }

                x -= ColumnPixelWidth;
            }
            m_minIndex = Math.Max(index, 0);
            DisplayEvents(true);
        }

        public List<TimelineEvent> VisibleTimelineEvents
 		{
 			get
 			{
 				if (m_visibleEvents != null)
 				{
 					return (from visibles in m_visibleEvents 
                                select visibles.Key as TimelineEvent).ToList();
 				}
 				return null;
 			}
 		}

        public long MinVisibleIndex
        {
            get
            {
                return m_minIndex + 2;
            }
        }

        public DateTime MinVisibleDateTime
        {
            get
            {
                return (DateTime) m_timeline[MinVisibleIndex];
            }
        }

        public long MaxVisibleIndex
        {
            get
            {
                return m_maxIndex - 2;
            }
        }

        public DateTime MaxVisibleDateTime
        {
            get
            {
                return (DateTime) m_timeline[MaxVisibleIndex];
            }
        }
    }
}
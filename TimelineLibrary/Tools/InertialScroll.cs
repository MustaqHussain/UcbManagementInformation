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
using System.Windows;
using System.Windows.Threading;

namespace TimelineLibrary
{
    public class InertialTimelineScroll
    {
        private const int                               TRACKING_INTERVAL = 100;
        private const double                            DECELERATION = 0.98;
        private const int                               DECELERATION_INTERVAL = 1000 / 48;
        private const int                               MIN_POINTS = 1;
        private const int                               DOUBLE_CLICK_TIME = 500;


        DispatcherTimer                                 m_timer;
        FrameworkElement                                m_element;

        bool                                            m_dragging;
        bool                                            m_inertialMode;
        bool                                            m_firstInertialStep;

        private Point                                   m_p1;
        private Point                                   m_p2;
        private Point                                   m_newPoint;
        private Point                                   m_prevPoint;
        private bool                                    m_haveNewPoint;
        private bool                                    m_haveBothPoints;

        private double                                  m_incrementX;
        private double                                  m_incrementY;
        private DateTime                                m_lastClick = new DateTime(1, 1, 1);
        
        public event Action<Point>                      OnDragStart;                             
        public event Action<Point>                      OnDragStop;                             
        public event Action<Point, Point>               OnDragMove;   
        public event Action<Point>                      OnDoubleClick;
     
        public static bool                              Moving;
        public static InertialTimelineScroll            MovingScroll;

        public InertialTimelineScroll(
            FrameworkElement                            fe
        )
        {
            m_element = fe;

            m_element.MouseLeftButtonUp += OnDragMouseUp;
            m_element.MouseLeftButtonDown += OnDragMouseDown;
            m_element.MouseMove += OnMouseMove;
        }

        void OnTick(
            object                                      sender, 
            EventArgs                                   e
        )
        {
            if (m_dragging)
            {
                if (m_haveNewPoint)
                {
                    m_p1 = m_p2;
                    m_p2 = m_newPoint;
                    m_haveNewPoint = false;
                    m_haveBothPoints = true;
                }
            }
            else if (m_inertialMode)
            {
#if SILVERLIGHT
                InertialScrollStep();
#else
                m_element.Dispatcher.BeginInvoke(new Action(this.InertialScrollStep), DispatcherPriority.ApplicationIdle);
#endif
            }
        }

        private void InertialScrollStep()
        {
            if (m_firstInertialStep)
            {
                m_timer.Stop();
                m_element.ReleaseMouseCapture();

                m_timer.Interval = TimeSpan.FromMilliseconds(DECELERATION_INTERVAL);
                m_timer.Start();

                Moving = true;
                MovingScroll = this;

                m_incrementX += ((m_p2.X - m_p1.X) / TRACKING_INTERVAL) * DECELERATION_INTERVAL;
                m_incrementY += ((m_p2.Y - m_p1.Y) / TRACKING_INTERVAL) * DECELERATION_INTERVAL;

                m_firstInertialStep = false;
            }

            m_p2.X += m_incrementX;
            m_p2.Y += m_incrementY;

            m_incrementX *= DECELERATION;
            m_incrementY *= DECELERATION;

            if (Math.Abs(m_incrementX) < MIN_POINTS && Math.Abs(m_incrementY) < MIN_POINTS)
            {
                Release();
            }
            else
            {
                if (OnDragMove != null)
                {
                    OnDragMove(m_p1, m_p2);
                }
                m_p1 = m_p2;
            }
        }

        public void Release(
        )
        {
            Moving = false;
            MovingScroll = null;
            m_element.ReleaseMouseCapture();

            m_timer.Stop();
            m_timer.Tick -= OnTick;

            m_inertialMode = false;
            m_firstInertialStep = false;
            m_p1 = m_p2 = new Point(0, 0);

            if (OnDragStop != null)
            {
                OnDragStop(m_p2);
            }
        }

        private void OnMouseMove(
            object                                      sender, 
            System.Windows.Input.MouseEventArgs         e
        )
        {
            if (m_dragging)
            {
                m_newPoint = e.GetPosition(m_element);
                
                if (m_haveNewPoint)
                {
                    OnDragMove(m_prevPoint, m_newPoint);
                }
                
                m_prevPoint = m_newPoint;
                m_haveNewPoint = true;
            }
        }

        private void OnDragMouseUp(
            object                                      sender, 
            System.Windows.Input.MouseButtonEventArgs   e
        )
        {
            Point                                       p;

            if (m_dragging)
            {
                m_dragging = false;

                p = e.GetPosition(m_element);
                
                if (m_haveBothPoints && p != m_p2)
                {
                    m_firstInertialStep = true;
                    m_inertialMode = true;
                }
                else
                {
                    Release();
                }
                e.Handled = true;
            }
        }

        private void OnDragMouseDown(
            object                                      sender, 
            System.Windows.Input.MouseButtonEventArgs   e
        )
        {
            bool                                        capture;

            if (m_dragging && MovingScroll != null)
            {
                MovingScroll.Release();
            }

            if (DateTime.Now - m_lastClick < TimeSpan.FromMilliseconds(DOUBLE_CLICK_TIME) && OnDoubleClick != null)
            {
                OnDoubleClick(e.GetPosition(m_element));
            }
            else 
            {
                capture = m_element.CaptureMouse();

                m_newPoint = e.GetPosition(m_element);
                m_p2 = m_newPoint;

                m_haveNewPoint = false;
                m_haveBothPoints = false;

                m_dragging = true;
#if SILVERLIGHT
                m_timer = new DispatcherTimer();
#else
                m_timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
#endif
                m_timer.Interval = TimeSpan.FromMilliseconds(TRACKING_INTERVAL);
                m_timer.Tick += OnTick;
                m_timer.Start();

                OnDragStart(m_newPoint);
                e.Handled = true;
            }
            m_lastClick = DateTime.Now;
        }

    } 
}

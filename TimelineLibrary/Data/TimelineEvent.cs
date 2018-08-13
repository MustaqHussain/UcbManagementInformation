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
using System.Net;
using System.ComponentModel;

namespace TimelineLibrary
{
    /// 
    /// <summary>
    /// Class that represents event element read from xml file</summary>
    /// 
    public class TimelineEvent: INotifyPropertyChanged
    {
        #region Private fields

        private int                                     m_row = -1;
        private int                                     m_rowOverride = -1;
        private double                                  m_widthOverride = -1;
        private double                                  m_heightOverride = -1;
        private double                                  m_topOverride = -1;
        private string                                  m_image = String.Empty;
        private string                                  m_id;
        private string                                  m_title;
        private string                                  m_description;
        private DateTime                                m_start;
        private DateTime                                m_end;
        private string                                  m_link;
        private bool                                    m_isDuration;
        private string                                  m_teaserImage;
        private string                                  m_eventColor;
        private object                                  m_tag;
		private bool									m_selected;


        private bool? isVersionLatest;

        #endregion

        public bool? IsVersionLatest
        {
            get { return isVersionLatest; }
            set { isVersionLatest = value; }
        }

        public event PropertyChangedEventHandler        PropertyChanged;

        protected void FirePropertyChanged(
            string                                      name
        )
        {
            if (PropertyChanged != null)
            {
                //PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
                FirePropertyChanged("Id");
            }
        }
        
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
                FirePropertyChanged("Title");
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                FirePropertyChanged("Description");
            }
        }
        
        public DateTime StartDate
        {
            get
            {
                return m_start;
            }
            set
            {
                m_start = value;
                FirePropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get
            {
                return m_end;
            }
            set
            {
                m_end = value;
                FirePropertyChanged("EndDate");
            }
        }

        public string Link
        {
            get
            {
                return m_link;
            }
            set
            {
                m_link = value;
                FirePropertyChanged("Link");
            }
        }

        public bool IsDuration
        {
            get
            {
                return m_isDuration;
            }
            set
            {
                m_isDuration = value;
                FirePropertyChanged("IsDuration");
            }
        }

        public string EventImage
        {
            get
            {
                return m_image;
            }
            set
            {
                m_image = value;
                FirePropertyChanged("EventImage");
            }
        }

        public string TeaserEventImage
        {
            get
            {
                return m_teaserImage;
            }
            set
            {
                m_teaserImage = value;
                FirePropertyChanged("TeaserEventImage");
            }
        }

        public string EventColor
        {
            get
            {
                return m_eventColor;
            }
            set
            {
                m_eventColor = value;
                FirePropertyChanged("EventColor");
            }
        }

        public int RowOverride
        {
            get
            {
                return m_rowOverride;
            }

            set
            {
                m_rowOverride = value;
				FirePropertyChanged("RowOverride");
            }
        }

        public double WidthOverride
        {
            get
            {
                return m_widthOverride;
            }

            set
            {
                m_widthOverride = value;
				FirePropertyChanged("WidthOverride");
            }
        }

        public double HeightOverride
        {
            get
            {
                return m_heightOverride;
            }

            set
            {
                m_heightOverride = value;
				FirePropertyChanged("HeightOverride");
            }
        }

        public double TopOverride
        {
            get
            {
                return m_topOverride;
            }

            set
            {
                m_topOverride = value;
				FirePropertyChanged("TopOverride");
            }
        }

        public object Tag
        {
            get
            {
                return m_tag;
            }

            set
            {
                m_tag = value;
                FirePropertyChanged("Tag");
            }
        }

        public int Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
                FirePropertyChanged("Row");
            }
        }

		public bool Selected
		{
			get
			{
				return m_selected;
			}
			set 
			{
				m_selected = value;
				FirePropertyChanged("Selected");
			}
		}
    }
}

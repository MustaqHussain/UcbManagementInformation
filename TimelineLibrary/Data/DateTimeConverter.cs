﻿/* 
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
using System.ComponentModel;
using System.Globalization;

namespace TimelineLibrary
{
    public class DateTimeConverter: TypeConverter
    {
        public const string                            DEFAULT_CULTUREID = "en-US";

        public static CultureInfo                      CultureInfo = new CultureInfo(DEFAULT_CULTUREID);
        private static string                          m_cultId = DEFAULT_CULTUREID;

        public static string CultureId
        {
            get
            {
                return m_cultId;
            }
            set
            {
                CultureInfo = new CultureInfo(value);
                m_cultId = value;
            }
        }

        public override bool CanConvertFrom(
            ITypeDescriptorContext                      context, 
            Type                                        sourceType
        )
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(
            ITypeDescriptorContext                      context, 
            Type                                        destinationType
        )
        {   
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
            ITypeDescriptorContext                      context, 
            CultureInfo                                 culture, 
            object                                      value, 
            Type                                        destinationType
        )
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext                      context, 
            CultureInfo                                 culture, 
            object                                      value
        )
        {
            if (value.GetType() == typeof(string))
            {
                return ParseDateTime((string) value);
            }
            return base.ConvertFrom(context, culture, value);
        }


        /// 
        /// <summary>
        /// Parse initial date passed from xaml, could be datetime string or 
        /// 'today', 'now', 'yesterday', etc. constants.</summary>
        /// 
        /// 
        public static DateTime ParseDateTime(
            string                                      initialTime
        )
        {
            DateTime                                    initTime;

            switch(initialTime.ToLower())
            {
                case "tomorrow":
                    initTime = DateTime.Today.AddDays(1);
                    break;

                case "today":
                    initTime = DateTime.Today;
                    break;

                case "yesterday":
                    initTime = DateTime.Today.AddDays(-1);
                    break;

                case "now":
                    initTime = DateTime.Now;
                    break;

                default:
                    initTime = DateTime.Parse(initialTime, DateTimeConverter.CultureInfo);
                    break;
            }

            return initTime;
        }

    }
}

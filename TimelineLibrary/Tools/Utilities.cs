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
using System.Diagnostics;

namespace TimelineLibrary
{
    /// 
    /// <summary>
    /// Misc utilities</summary>
    /// 
    public static class Utilities
    {
        private const string                            FMT_TRACE = "{0:000000}:{1}.{2} {3}";
        private static DateTime                         g_start = DateTime.Now;
        private const int                               METHOD_ALIGN = 30;

        /// 
        /// <summary>
        /// Outputs debug trace message</summary>
        /// 
        public static void Trace<T>(
            object                                      sender, 
            T                                           msgObject
        ) 
        {
#if DEBUG
            StackTrace                                  trace;
            string                                      method;
            string                                      message;

            if (Debugger.IsLogging())
            {
                trace = new System.Diagnostics.StackTrace();
                method = trace.GetFrame(2).GetMethod().Name;

                message = msgObject != null ? msgObject.ToString() : String.Empty;

                if (method.Length < METHOD_ALIGN)
                {
                    method += new String(' ', METHOD_ALIGN - method.Length); 
                }

                Debug.WriteLine(string.Format(
                    FMT_TRACE, 
                    (DateTime.Now - g_start).TotalMilliseconds,
                    sender.GetType().Name, 
                    method, 
                    message.Length == 0 ? sender.ToString() : message));
            }
#endif
        }

        public static void Trace(
            object                                      sender
        )
        {
            Trace(sender, String.Empty);
        }
    }
}

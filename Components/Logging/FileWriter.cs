#region BSD license

/*
 * Copyright (c) 2008, Thomas "zeroflag" Kraemer. All rights reserved.
 * Copyright (c) 2008, Anders "anonimasu" Helin. All rights reserved.
 * Copyright (c) 2008, The zeroflag.Components.NET Team. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * Neither the name of the zeroflag.Components.NET Team nor the names of its contributors may 
 * be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

#endregion BSD license

#region SVN Version Information

///	<file>
///		<!-- Last modification of this file: -->
///		<revision>$Rev: 29 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: FileWriter.cs 29 2008-09-24 06:56:58Z zeroflag $</id>
///	</file>

#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace zeroflag.Components.Logging
{
    public class FileWriter : LogWriter
    {
        private readonly Dictionary<string, StreamWriter> _Writers = new Dictionary<string, StreamWriter>();
        private StreamWriter _Master;
        private string _MasterName;
        private string _NameTemplate;
        private bool _SplitByOwner;

        /// <summary>
        /// Whether to create one log-file for each owner.
        /// </summary>
        public bool SplitByOwner
        {
            get { return this._SplitByOwner; }
            set
            {
                if (this._SplitByOwner != value)
                {
                    this._SplitByOwner = value;
                }
            }
        }


        /// <summary>
        /// The name for the master-log.
        /// </summary>
        public string MasterName
        {
            get { return this._MasterName; }
            set
            {
                if (this._MasterName != value)
                {
                    this._MasterName = value;
                }
            }
        }


        /// <summary>
        /// The template for the log-file-name. %n is replaced by the owner.
        /// </summary>
        public string NameTemplate
        {
            get { return this._NameTemplate; }
            set
            {
                if (this._NameTemplate != value)
                {
                    this._NameTemplate = value;
                }
            }
        }


        protected Dictionary<string, StreamWriter> Writers
        {
            get { return this._Writers; }
        }

        public StreamWriter this[string name]
        {
            get
            {
                if (!this.Writers.ContainsKey(name) || this.Writers[name] == null)
                {
                    this.Writers[name] = this.CreateWriter(name);
                }
                return this.Writers[name];
            }
        }

        public StreamWriter Master
        {
            get { return this._Master ?? (this._Master = this.CreateWriter(this.MasterName ?? "master")); }
            set { this._Master = value; }
        }

        protected StreamWriter CreateWriter(string name)
        {
            StreamWriter writer = null;
            string sufix = "";
            int count = -1;
            while (writer == null)
            {
                try
                {
                    string temp = name + sufix;
                    writer = new StreamWriter((this.NameTemplate ?? "log_%n.txt").Replace("%n", temp));
                    break;
                }
                catch (IOException)
                {
                    count++;
                    sufix = "_" + count;

                    if (count > 10)
                    {
                        return writer;
                        //throw;
                    }
                }
            }
            return writer;
        }


        public override void Write(DateTime time, string owner, string text)
        {
            try
            {
                this.Master.WriteLine(
                    new StringBuilder(time.ToString("HH:mm:ss.fff")).Append(" [").Append(owner.PadLeft(15)).Append("]: ")
                        .Append(text));
                if (this.SplitByOwner)
                    this[owner].WriteLine(
                        new StringBuilder(time.ToString("HH:mm:ss.fff")).Append(" [").Append(owner.PadLeft(15)).Append(
                            "]: ")
                            .Append(text));
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public override void Flush()
        {
            foreach (StreamWriter writer in this.Writers.Values)
                try
                {
                    writer.Flush();
                }
                catch (ObjectDisposedException)
                {
                }
            try
            {
                this.Master.Flush();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public override void Dispose()
        {
            foreach (StreamWriter writer in this.Writers.Values)
            {
                try
                {
                    writer.WriteLine(new StringBuilder(DateTime.Now.ToString("HH:mm:ss.fff")).Append(" closed."));
                }
                catch (ObjectDisposedException)
                {
                }

                writer.Dispose();
            }
            try
            {
                this.Master.WriteLine(new StringBuilder(DateTime.Now.ToString("HH:mm:ss.fff")).Append(" closed."));
            }
            catch (ObjectDisposedException)
            {
            }

            this.Master.Dispose();
        }
    }
}

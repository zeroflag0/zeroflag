#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

#region BSD License
/*
 * Copyright (c) 2006-2008, Thomas "zeroflag" Kraemer
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list 
 * of conditions and the following disclaimer. Redistributions in binary form must 
 * reproduce the above copyright notice, this list of conditions and the following 
 * disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the SAGEngine nor the names of its contributors may be used 
 * to endorse or promote products derived from this software without specific prior 
 * written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion BSD License

using System;
using System.Collections.Generic;

namespace zeroflag.Threading
{
	/// <summary>
	/// A single-reader single-writer message-queue without locking.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>Update must be called by the writer when lots (= more than the buffer size, 2048 by default) of messages were sent before but nothing is added.</remarks>
	public class LocklessQueueOld<T>// : Sage.Threading.IMessageQueue<T>
		where T : class // <-- must be a reference type or an atomic type to be threadsafe
	{
		Queue<T> m_WriteQueue = new Queue<T>();
		/// <summary>
		/// Queue for new values before they're added to the buffer (through Update).
		/// </summary>
		protected Queue<T> WriteQueue
		{
			get { return m_WriteQueue; }
		}

		#region MessageContainer
		/// <summary>
		/// A box arround the stored objects.
		/// With this, one can set a value in the array without causing it to synchronize to the other threads or override their versions.
		/// </summary>
		protected class MessageContainer
		{
			T m_Value;
			/// <summary>
			/// Gets or sets the value.
			/// Getting the value will result in its deletion (null).
			/// </summary>
			public T Value
			{
				get
				{
					T value = m_Value;
					if ( value != null )
					{
						this.m_Value = null;
					}
					return value;
				}
				set
				{
					m_Value = value;
				}
			}
			/// <summary>
			/// Whether the box has a value or not.
			/// </summary>
			public bool HasValue
			{
				get
				{
					if ( m_Value != null )
						return true;
					else
						return false;
				}
			}
		}
		#endregion MessageContainer

		public LocklessQueueOld( int bufferSize )
		{
			this.m_Buffer = new MessageContainer[ bufferSize ];
			this.Clear();
		}

		/// <summary>
		/// Creates a new message queue and initializes it's buffer.
		/// </summary>
		public LocklessQueueOld()
		{
			this.Clear();
		}

		~LocklessQueueOld()
		{
			for ( int i = 0; i < this.Buffer.Length; i++ )
			{
				this.Buffer[ i ] = null;
			}
			this.m_Buffer = null;
		}
		/// <summary>
		/// Clear all content from the write queue and the ringbuffer.
		/// Must only be called when threads are synchronized.
		/// </summary>
		protected void Clear()
		{
			this.ReadIndex = 0;
			this.WriteIndex = 0;
			this.WriteQueue.Clear();

			for ( int i = 0; i < this.Buffer.Length; i++ )
			{
				if ( this.Buffer[ i ] == null )
					this.Buffer[ i ] = new MessageContainer();
				else
					this.Buffer[ i ].Value = null;
			}
		}

		private int ReadIndex = 0;
		private int WriteIndex = 0;

		MessageContainer[] m_Buffer = new MessageContainer[ 2048 ];
		/// <summary>
		/// The queue's ring buffer.
		/// Default size is 2048 entries.
		/// </summary>
		protected MessageContainer[] Buffer
		{
			get { return m_Buffer; }
		}

		#region IMessageQueue<T> Members
		/// <summary>
		/// Whether there are values ready to be read.
		/// *** READER THREAD ***
		/// </summary>
		public bool IsEmpty
		{
			get { return !this.Buffer[ this.ReadIndex ].HasValue; }
		}
		/// <summary>
		/// Pops a value from the queue.
		/// *** READER THREAD ***
		/// </summary>
		/// <returns></returns>
		public T Pop()
		{
			if ( this.Buffer[ this.ReadIndex ].HasValue )
			// value waiting?
			{
				// grab the current value...
				T value = this.Buffer[ this.ReadIndex ].Value;

				// continue to the next...
				this.ReadIndex = ( this.ReadIndex + 1 ) % this.Buffer.Length;

				return value;
			}
			else
			{
				// no value waiting...
				return null;
			}
		}
		/// <summary>
		/// Pushes a new value to the queue and updates the buffer.
		/// *** WRITER THREAD ***
		/// </summary>
		/// <param name="msg"></param>
		public void Push( T msg )
		{
			// push the message to the write queue...
			this.WriteQueue.Enqueue( msg );
			// try to push some more to the buffer...
			this.Update();
		}


		/// <summary>
		/// Fills the buffer from the queue.
		/// *** WRITER THREAD ***
		/// </summary>
		public void Update()
		{
			// feed the buffer...
			while ( this.PushToBuffer() ) ;
		}

		#endregion
		/// <summary>
		/// Pushes a single value from the WriteQueue to the Buffer.
		/// </summary>
		/// <returns>True if a value was pushed, false if it wasn't (like no items in queue or no free cell in buffer).</returns>
		protected bool PushToBuffer()
		{
			if ( !this.Buffer[ this.WriteIndex ].HasValue && this.WriteQueue.Count > 0 )
			// buffer cell free?
			{
				// get the message...
				T value = this.WriteQueue.Dequeue();

				// store the message...
				this.Buffer[ this.WriteIndex ].Value = value;

				// move the index to the next cell...
				this.WriteIndex = ( this.WriteIndex + 1 ) % this.Buffer.Length;

				// yes, we pushed a new message from the write-queue to the buffer...
				return true;

			}
			else
			{
				// no, the exchange table is full...
				return false;
			}
		}
	}
}

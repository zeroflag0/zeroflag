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
using System.Threading;

namespace zeroflag.Threading
{
	/// <summary>
	/// A single-reader single-writer message-queue without locking.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MultiWriterQueue<T>
	{
		#region First

		private Node _First;

		/// <summary>
		/// The first node in the queue.
		/// </summary>
		public Node First
		{
			get
			{
				// if there's nothing in First, doesn't mean there's nothing at all...
				if ( _First == null )
					if ( _ReadLast != null )
					{
						if ( _ReadLast.Next != null )
						{
							_First = _ReadLast.Next;
						}
					}
				return _First;
			}
			set
			{
				if ( _First != value )
				{
					_First = value;
				}
			}
		}

		#endregion First

		#region Last

		private Node _Last;

		/// <summary>
		/// The last node in the queue.
		/// </summary>
		/// <remarks>This value is not guaranteed to always represent THE last node, but it should always contain one of the later nodes, except when the Queue is empty...</remarks>
		public Node Last
		{
			get { return _Last; }
			set
			{
				if ( _Last != value )
				{
					_Last = value;
				}
			}
		}

		#endregion Last

		#region IsEmpty

		/// <summary>
		/// Whether the queue is empty or not.
		/// </summary>
		public bool IsEmpty
		{
			get { return object.ReferenceEquals( this.First, null ); }
		}

		#endregion IsEmpty


		/// <summary>
		/// Write one value to the queue. This method may be used simultaneously by multiple threads.
		/// </summary>
		/// <param name="value"></param>
		public virtual void Write( T value )
		{
			Node node = new Node() { Value = value };
			Node last;
			// remember the new node as last node...
			if ( ( last = Interlocked.Exchange<Node>( ref _Last, node ) ) != null )
			// if there was a last node before...
			{
				// link the new last node behind the previous last node...
				Interlocked.Exchange<Node>( ref last.Next, node );
			}
			else
				// if there wasn't a last node before, the new node is also the first node...
				Interlocked.Exchange<Node>( ref _First, node );
		}

		Node _ReadLast;
		/// <summary>
		/// Read one value from the queue. This method removes the first item.
		/// NOTE: This method may only be used from a single thread!
		/// </summary>
		/// <returns></returns>
		public virtual T Read()
		{
			if ( First != null )	// <-- this check is crucial as First{get;} also corrects race-conditions in _First
				return _ReadLast = Interlocked.Exchange<Node>( ref _First, _First.Next );
			else
				return default( T );
		}

		#region Node
		public class Node
		{
			#region Value

			/// <summary>
			/// The node's value.
			/// </summary>
			public T Value;

			#endregion Value

			#region Next

			/// <summary>
			/// The following node, if any.
			/// </summary>
			public Node Next;

			#endregion Next

			//public static implicit operator Node( T value )
			//{
			//    return new Node() { Value = value };
			//}

			public override string ToString()
			{
				return this.Value.ToString() + " > " + ( (object)this.Next ?? "<null>" );
			}
		}
		#endregion Node
	}
}

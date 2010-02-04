#region LGPL License

//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
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

using System;

namespace zeroflag.Collections
{
	public interface ITreeNode<TNode> : System.Collections.Generic.IEnumerable<TNode>
		where TNode : class, ITreeNode<TNode>
	{
		void Add(TNode child);
		List<TNode> Children { get; }
		void Clear();
		bool Contains(TNode child);
		int Count { get; }
		TNode Parent { get; set; }
		event Action<object, TNode, TNode> ParentChanged;
		bool Remove(TNode child);
		System.Collections.Generic.IEnumerable<TNode> Enumerate();
	}
}
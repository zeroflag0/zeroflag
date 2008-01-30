using System;
namespace zeroflag.Collections
{
	public interface ITreeNode<T> : System.Collections.Generic.IEnumerable<T>
		where T : TreeNode<T>
	{
		void Add(T child);
		List<T> Children { get; }
		void Clear();
		bool Contains(T child);
		int Count { get; }
		T Parent { get; set; }
		event TreeNode<T>.ParentChangedHandler ParentChanged;
		bool Remove(T child);
	}
}

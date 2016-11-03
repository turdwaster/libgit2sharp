using System;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    /// <summary>
    /// A filter source - describes the direction of filtering and the file being filtered.
    /// </summary>
    public class MergeDriverSource
    {
        public Repository Repository;
        public IndexEntry Ancestor;
        public IndexEntry Ours;
        public IndexEntry Theirs;

        /// <summary>
        /// Needed for mocking purposes
        /// </summary>
        protected MergeDriverSource() { }

        protected MergeDriverSource(Repository repos, IndexEntry ancestor, IndexEntry ours, IndexEntry theirs)
        {
            Repository = repos;
            Ancestor = ancestor;
            Ours = ours;
            Theirs = theirs;
        }

        /// <summary>
        /// Take an unmanaged pointer and convert it to filter source callback paramater
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal static unsafe MergeDriverSource FromNativePtr(IntPtr ptr)
        {
            return FromNativePtr((git_merge_driver_source*)ptr.ToPointer());
        }

        /// <summary>
        /// Take an unmanaged pointer and convert it to filter source callback paramater
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal static unsafe MergeDriverSource FromNativePtr(git_merge_driver_source* ptr)
        {
            if (ptr == null)
                throw new ArgumentException();

            return new MergeDriverSource(
                new Repository(new RepositoryHandle(ptr->repository, false)),
                IndexEntry.BuildFromPtr(ptr->ancestor),
                IndexEntry.BuildFromPtr(ptr->ours),
                IndexEntry.BuildFromPtr(ptr->theirs));
        }
    }
}

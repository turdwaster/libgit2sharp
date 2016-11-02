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
        public ObjectId AncestorId;
        public ObjectId OurId;
        public ObjectId TheirsId;

        /// <summary>
        /// Needed for mocking purposes
        /// </summary>
        protected MergeDriverSource(Repository repos, ObjectId ancestorId, ObjectId oursId, ObjectId theirsId)
        {
            Repository = repos;
            AncestorId = ancestorId;
            OurId = oursId;
            TheirsId = theirsId;
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

            var ancestor = ptr->ancestor;
            var ours = ptr->ours;
            var theirs = ptr->theirs;
            var repoPtr = ptr->repository;

            var repo = new Repository(new RepositoryHandle(repoPtr, false));

            var ancestorId = ancestor == null ? null : ObjectId.BuildFromPtr(&ancestor->id);
            var oursId = ours == null ? null : ObjectId.BuildFromPtr(&ours->id);
            var theirsId = theirs == null ? null : ObjectId.BuildFromPtr(&theirs->id);

            return new MergeDriverSource(repo, ancestorId, oursId, theirsId);
        }
    }
}

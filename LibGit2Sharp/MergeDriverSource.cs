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
        public ObjectId AncestorId;
        public ObjectId OurId;
        public ObjectId TheirsId;

        /// <summary>
        /// Needed for mocking purposes
        /// </summary>
        protected MergeDriverSource(ObjectId ancestorId, ObjectId oursId, ObjectId theirsId)
        {
            AncestorId = ancestorId;
            OurId = oursId;
            TheirsId = theirsId;
        }

        internal unsafe MergeDriverSource(git_merge_driver_source* source)
        {
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
            var ancestorId = ptr->ancestor->id;
            var oursId = ptr->ours->id;
            var theirsId = ptr->theirs->id;

            var ancestorContent = ObjectId.BuildFromPtr(&ancestorId);
            var oursContent = ObjectId.BuildFromPtr(&oursId);
            var theirsContent = ObjectId.BuildFromPtr(&theirsId);
           
            return new MergeDriverSource(ancestorContent, oursContent, theirsContent);
        }
    }
}

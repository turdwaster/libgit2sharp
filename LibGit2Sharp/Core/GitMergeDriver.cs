﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibGit2Sharp.Core
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GitMergeDriver
    {
        /** The `version` should be set to `GIT_MERGE_DRIVER_VERSION`. */
        public uint version;

        /** Called when the merge driver is first used for any file. */
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public git_merge_driver_init_fn initialize;

        /** Called when the merge driver is unregistered from the system. */
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public git_merge_driver_shutdown_fn shutdown;

        /**
         * Called to merge the contents of a conflict.  If this function
         * returns `GIT_PASSTHROUGH` then the default (`text`) merge driver
         * will instead be invoked.  If this function returns
         * `GIT_EMERGECONFLICT` then the file will remain conflicted.
         */
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public git_merge_driver_apply_fn apply;

        internal delegate int git_merge_driver_init_fn(IntPtr merge_driver);
        internal delegate void git_merge_driver_shutdown_fn(IntPtr merge_driver);
        internal delegate int git_merge_driver_apply_fn(
            IntPtr merge_driver,
            IntPtr path_out,
            UIntPtr mode_out,
            IntPtr merged_out,// GitBuf merged_out,
            IntPtr filter_name,
            IntPtr merge_driver_source // const git_merge_driver_source *src
        );
    }

    /// <summary>
    /// The file source being filtered
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct git_merge_driver_source
    {
        public git_repository* repository;
	    char *default_driver;
	    IntPtr file_opts;

	    public git_index_entry* ancestor;
        public git_index_entry* ours;
        public git_index_entry* theirs;
    }
}

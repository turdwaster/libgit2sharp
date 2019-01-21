﻿using System;
using LibGit2Sharp.Core;
using System.Runtime.InteropServices;

namespace LibGit2Sharp
{
    public sealed class MergeDriverRegistration
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="driver"></param>
        internal MergeDriverRegistration(MergeDriver driver)
        {
            System.Diagnostics.Debug.Assert(driver != null);

            MergeDriver = driver;

            // marshal the git_merge_driver structure into native memory
            MergeDriverPointer = Marshal.AllocHGlobal(Marshal.SizeOf(driver.GitMergeDriver));
            Marshal.StructureToPtr(driver.GitMergeDriver, MergeDriverPointer, false);

            // register the merge driver with the native libary
            Proxy.git_merge_driver_register(driver.Name, MergeDriverPointer);
        }
        /// <summary>
        /// Finalizer called by the <see cref="GC"/>, deregisters and frees native memory associated with the registered filter in libgit2.
        /// </summary>
        ~MergeDriverRegistration()
        {
            // deregister the filter
            GlobalSettings.DeregisterMergeDriver(this);
            // clean up native allocations
            Free();
        }

        /// <summary>
        /// Gets if the registration and underlying filter are valid.
        /// </summary>
        public bool IsValid { get { return !freed; } }
        /// <summary>
        /// The registerd filters
        /// </summary>
        public readonly MergeDriver MergeDriver;
        /// <summary>
        /// The name of the driver in the libgit2 registry
        /// </summary>
        public string Name { get { return MergeDriver.Name; } }
        /// <summary>
        /// The priority of the registered filter
        /// </summary>
        public readonly int Priority;

        private readonly IntPtr MergeDriverPointer;

        private bool freed;

        internal void Free()
        {
            if (!freed)
            {
                // unregister the filter with the native libary
                Proxy.git_merge_driver_unregister(MergeDriver.Name);
                // release native memory
                Marshal.FreeHGlobal(MergeDriverPointer);
                // remember to not do this twice
                freed = true;
            }
        }
    }
}

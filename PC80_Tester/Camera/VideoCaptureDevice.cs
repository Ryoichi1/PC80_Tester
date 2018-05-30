// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using AForge.Video.DirectShow.Internals;


    public class VideoCaptureDevice2 //: IVideoSource
    {
        // moniker string of video capture device
        private string deviceMoniker;

        // received frames count
        //private int framesReceived;

        // recieved byte count
        //private long bytesReceived;

        // video and snapshot resolutions to set
        //private VideoCapabilities videoResolution = null;

        //private VideoCapabilities snapshotResolution = null;

        // provide snapshots or not
        //private bool provideSnapshots = false;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        //private VideoCapabilities[] videoCapabilities;
        //private VideoCapabilities[] snapshotCapabilities;

        //private bool needToSetVideoInput = false;
        //private bool needToSimulateTrigger = false;
        //private bool needToDisplayPropertyPage = false;
        //private bool needToDisplayCrossBarPropertyPage = false;
        //private IntPtr parentWindowForPropertyPage = IntPtr.Zero;

        // video capture source object
        //private object sourceObject = null;

        // time of starting the DirectX graph
        //private DateTime startTime = new DateTime();

        // dummy object to lock for synchronization
        private object sync = new object();

        // flag specifying if IAMCrossbar interface is supported by the running graph/source object
        //private bool? isCrossbarAvailable = null;

        //private VideoInput[] crossbarVideoInputs = null;
        //private VideoInput crossbarVideoInput = VideoInput.Default;

        // cache for video/snapshot capabilities and video inputs
        //private static Dictionary<string, VideoCapabilities[]> cacheVideoCapabilities = new Dictionary<string, VideoCapabilities[]>();

        //private static Dictionary<string, VideoCapabilities[]> cacheSnapshotCapabilities = new Dictionary<string, VideoCapabilities[]>();
        //private static Dictionary<string, VideoInput[]> cacheCrossbarVideoInputs = new Dictionary<string, VideoInput[]>();


        /// <summary>
        /// State of the video source.
        /// </summary>
        ///
        /// <remarks>Current state of video source object - running or not.</remarks>
        ///
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        ///
        //public VideoCaptureDevice2()
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        ///
        /// <param name="deviceMoniker">Moniker string of video capture device.</param>
        ///
        public VideoCaptureDevice2(string deviceMoniker)
        {
            this.deviceMoniker = deviceMoniker;
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        ///
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        ///
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        ///
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        ///
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        ///
        /// <remarks><para>Stops video source aborting its thread.</para>
        ///
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        ///
        //public void Stop()
        //{
        //    if (this.IsRunning)
        //    {
        //        thread.Abort();
        //        WaitForStop();
        //    }
        //}

        /// <summary>
        /// Free resource.
        /// </summary>
        ///
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }

        /// <summary>
        /// Sets a specified property on the camera.
        /// </summary>
        ///
        /// <param name="property">Specifies the property to set.</param>
        /// <param name="value">Specifies the new value of the property.</param>
        /// <param name="controlFlags">Specifies the desired control setting.</param>
        ///
        /// <returns>Returns true on success or false otherwise.</returns>
        ///
        /// <exception cref="ArgumentException">Video source is not specified - device moniker is not set.</exception>
        /// <exception cref="ApplicationException">Failed creating device object for moniker.</exception>
        /// <exception cref="NotSupportedException">The video source does not support camera control.</exception>
        ///
        public bool SetVideoProperty(VideoProcAmpProperty property, int value, VideoProcAmpFlags controlFlags)
        {
            bool ret = true;

            // check if source was set
            if ((deviceMoniker == null) || (string.IsNullOrEmpty(deviceMoniker)))
            {
                throw new ArgumentException("Video source is not specified.");
            }

            lock (sync)
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter(deviceMoniker);
                }
                catch
                {
                    throw new ApplicationException("Failed creating device object for moniker.");
                }

                if (!(tempSourceObject is IAMVideoProcAmp))
                {
                    throw new NotSupportedException("The video source does not support camera control.");
                }

                IAMVideoProcAmp pCamControl = (IAMVideoProcAmp)tempSourceObject;
                int hr = pCamControl.Set(property, value, controlFlags);

                ret = (hr >= 0);

                Marshal.ReleaseComObject(tempSourceObject);
            }

            return ret;
        }


    }
}
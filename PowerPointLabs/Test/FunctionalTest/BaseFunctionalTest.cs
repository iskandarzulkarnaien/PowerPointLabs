﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;

using Microsoft.Office.Interop.PowerPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Test.Base;
using Test.Util;

using TestInterface;
using TestInterface.Windows;

namespace Test.FunctionalTest
{
    [TestClass]
    public abstract class BaseFunctionalTest: TestAssemblyFixture
    {
        public TestContext TestContext { get; set; }

        // prefix legend:
        // pp - PowerPoint
        // ppl - PowerPointLabs
        protected static IPowerPointLabsFeatures PplFeatures;
        protected static IPowerPointOperations PpOperations;
        protected static IWindowStackManager WindowStackManager;

        // To be implemented by downstream testing classes,
        // specify the name for the testing slide.
        // It is assumed that the testing slides reside
        // in "doc/test" folder.
        protected abstract string GetTestingSlideName();

        // To be override by some test case to use new
        // PowerPoint application instance for FT
        protected virtual bool IsUseNewPpInstance()
        {
            return false;
        }

        [TestInitialize]
        public void Setup()
        {
            if (IsUseNewPpInstance())
            {
                CloseActivePpInstance();
            }

            Process pptProcess = GetChildProcess(GetTestingSlideName());
            Process mainProcess = GetMainProcessAndCloseOthers(pptProcess);
            SetupProcessAndWindowWatching(mainProcess, pptProcess);
            ConnectPpl();
        }

        [TestCleanup]
        public void TearDown()
        {
            TeardownWindowWatching();
            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                if (!Directory.Exists(PathUtil.GetTestFailurePath()))
                {
                    Directory.CreateDirectory(PathUtil.GetTestFailurePath());
                }
                PpOperations.SavePresentationAs(
                    PathUtil.GetTestFailurePresentationPath(
                        TestContext.TestName + "_" +
                        GetTestingSlideName()));
            }
            while (PpOperations.GetNumWindows() > 0)
            {
                PpOperations.ClosePresentation();
            }
        }

        protected static void CheckIfClipboardIsRestored(Action action, int actualSlideNum, string shapeNameToBeCopied, int expSlideNum, string expShapeNameToDelete, string expCopiedShapeName)
        {
            Slide actualSlide = PpOperations.SelectSlide(actualSlideNum);
            ShapeRange shapeToBeCopied = PpOperations.SelectShape(shapeNameToBeCopied);
            Assert.AreEqual(1, shapeToBeCopied.Count);

            // Add this shape to clipboard
            shapeToBeCopied.Copy();
            action();

            // Paste whatever in clipboard
            ShapeRange newShape = actualSlide.Shapes.Paste();

            // Check if pasted shape is the same as the shape added to clipboard originally
            Assert.AreEqual(shapeNameToBeCopied, newShape.Name);
            Assert.AreEqual(shapeToBeCopied.Count, newShape.Count);

            Slide expSlide = PpOperations.SelectSlide(expSlideNum);
            if (expShapeNameToDelete != "")
            {
                PpOperations.SelectShape(expShapeNameToDelete)[1].Delete();
            }

            //Set the pasted shape location because the location of the pasted shape is flaky
            Shape expCopied = PpOperations.SelectShape(expCopiedShapeName)[1];
            newShape.Top = expCopied.Top;
            newShape.Left = expCopied.Left;

            SlideUtil.IsSameLooking(expSlide, actualSlide);
        }

        /// <summary>
        /// Postpones the process startup to have better control on waiting for the process to
        /// finish starting up.
        /// </summary>
        /// <param name="process">Process that windows will eventually reside on</param>
        /// <param name="childProcess">Process to be started</param>
        private void SetupProcessAndWindowWatching(Process process, Process childProcess)
        {
            string startWindowName = GetTestingSlideName().After("\\") + " - PowerPoint";
            WindowWatcher.Setup(process, childProcess, startWindowName);
            WindowWatcher.AddToWhitelist("PowerPointLabs FT");
            WindowWatcher.AddToWhitelist("Loading...");
        }

        private void ConnectPpl()
        {
            const int waitTime = 3000;
            int retryCount = 5;
            while (retryCount > 0)
            {
                // if already connected, break
                if (PplFeatures != null && PpOperations != null && WindowStackManager != null)
                {
                    break;
                }
                // otherwise keep trying to connect for some times
                try
                {
                    IPowerPointLabsFT ftInstance = (IPowerPointLabsFT)Activator.GetObject(typeof(IPowerPointLabsFT),
                        "ipc://PowerPointLabsFT/PowerPointLabsFT");
                    PplFeatures = ftInstance.GetFeatures();
                    PpOperations = ftInstance.GetOperations();
                    WindowStackManager = ftInstance.GetWindowStackManager();
                    WindowStackManager.Setup();
                    break;
                }
                catch (RemotingException)
                {
                    retryCount--;
                    ThreadUtil.WaitFor(waitTime);
                }
            }
            if (PplFeatures == null || PpOperations == null)
            {
                Assert.Fail("Failed to connect to PowerPointLabs add-in. You can try to increase retryCount.");
            }

            PpOperations.EnterFunctionalTest();

            // activate the thread of presentation window
            // Sometimes it takes very long for messag box to pop up
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "PowerPointLabs FT", "{*}",
                PpOperations.ActivatePresentation, null, 5, 10000);
        }

        private void TeardownWindowWatching()
        {
            WindowWatcher.Teardown();
            WindowStackManager.Teardown();
        }

        /// <summary>
        /// Closes all other processes except the main process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private Process GetMainProcessAndCloseOthers(Process process)
        {
            Process[] p = Process.GetProcessesByName(Constants.pptProcess);
            if (p.Length == 0) { return process; }
            for (int i = 1; i < p.Length; i++)
            {
                p[i].CloseMainWindow();
                p[i].WaitForExit();
            }
            return p[0]; // assume this is the main process
        }

        private Process GetChildProcess(string slideName)
        {
            Process pptProcess = new Process
            {
                StartInfo =
                {
                    FileName = slideName, 
                    WorkingDirectory = PathUtil.GetDocTestPath()
                }
            };
            return pptProcess;
        }

        private void CloseActivePpInstance()
        {
            Process[] processes = Process.GetProcessesByName(Constants.pptProcess);
            if (processes.Length > 0)
            {
                foreach (Process p in processes)
                {
                    p.CloseMainWindow();
                }
            }
            WaitForPpInstanceToClose();
            PpOperations = null;
            PplFeatures = null;
        }

        private void WaitForPpInstanceToClose()
        {
            int retry = 5;
            while (Process.GetProcessesByName(Constants.pptProcess).Length > 0
                && retry > 0)
            {
                retry--;
                ThreadUtil.WaitFor(1500);
            }

            if (Process.GetProcessesByName(Constants.pptProcess).Length > 0)
            {
                foreach (Process process in Process.GetProcessesByName(Constants.pptProcess))
                {
                    process.Kill();
                }
            }
        }
    }
}

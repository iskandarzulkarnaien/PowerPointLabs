using Microsoft.VisualStudio.TestTools.UnitTesting;

using Test.Util;

namespace Test.FunctionalTest
{
    [TestClass]
    public class AutoZoomTest : BaseFunctionalTest
    {
        private const int DrillDownActualStartSlideNo = 4;
        private const int DrillDownActualTransitionSlideNo = 5;
        private const int DrillDownActualEndSlideNo = 6;
        private const int DrillDownExpectedSlideNo = 7;
        private const int DrillDownExpectedTransitionSlideNo = 8;
        private const int DrillDownExpectedEndSlideNo = 9;

        private const int DrillDownBackgroundActualStartSlideNo = 10;
        private const int DrillDownBackgroundActualTransitionSlideNo = 11;
        private const int DrillDownBackgroundActualEndSlideNo = 12;
        private const int DrillDownBackgroundExpectedSlideNo = 13;
        private const int DrillDownBackgroundExpectedTransitionSlideNo = 14;
        private const int DrillDownBackgroundExpectedEndSlideNo = 15;

        private const int StepBackActualStartSlideNo = 16;
        private const int StepBackActualEndSlideNo = 17;
        private const int StepBackExpectedSlideNo = 20;
        private const int StepBackExpectedTransitionSlideNo = 21;
        private const int StepBackExpectedEndSlideNo = 22;

        private const int StepBackBackgroundActualStartSlideNo = 23;
        private const int StepBackBackgroundActualEndSlideNo = 24;
        private const int StepBackBackgroundExpectedSlideNo = 27;
        private const int StepBackBackgroundExpectedTransitionSlideNo = 28;
        private const int StepBackBackgroundExpectedEndSlideNo = 29;

        private const int ErrorTestingSlideNo = 34;
        private const int FirstSlideSlideNo = 1;
        private const int LastSlideSlideNo = 51;

        private const int CheckClipboardRestoredAfterDrillDownOriginalSlideNo = 31;
        private const int CheckClipboardRestoredAfterDrillDownActionSlideNo = 32;
        private const int CheckClipboardRestoredAfterDrillDownExpectedSlideNo = 34;

        private const int CheckClipboardRestoredAfterDrillDownBackgroundOriginalSlideNo = 34;
        private const int CheckClipboardRestoredAfterDrillDownBackgroundActionSlideNo = 35;
        private const int CheckClipboardRestoredAfterDrillDownBackgroundExpectedSlideNo = 37;

        private const int CheckClipboardRestoredAfterStepBackOriginalSlideNo = 37;
        private const int CheckClipboardRestoredAfterStepBackActionSlideNo = 38;
        private const int CheckClipboardRestoredAfterStepBackExpectedSlideNo = 40;

        private const int CheckClipboardRestoredAfterStepBackBackgroundOriginalSlideNo = 40;
        private const int CheckClipboardRestoredAfterStepBackBackgroundActionSlideNo = 41;
        private const int CheckClipboardRestoredAfterStepBackBackgroundExpectedSlideNo = 43;

        private const string ShapeToCopy = "pictocopy";
        private const string ShapeToDelete = "text 3";
        private const string ExpCopiedShape = "copied";

        protected override string GetTestingSlideName()
        {
            return "ZoomLab\\AutoZoom.pptx";
        }

        [TestMethod]
        [TestCategory("FT")]
        public void FT_AutoZoomTest()
        {
            // Do tests in reverse order because added slides change slide numbers lower down.
            // New tests should be added before older tests to prevent having to change slide no constants.
            // New slides should also be added after older slides.
            TestClipboardRestoredAfterStepBackBackground();
            TestClipboardRestoredAfterStepBack();
            TestClipboardRestoredAfterDrillDownBackground();
            TestClipboardRestoredAfterDrillDown();
            TestStepBackBackground();
            TestStepBack();
            TestDrillDownBackground();
            TestDrillDown();
            TestDrillDownUnsuccessful();
            TestStepBackUnsuccessful();
        }

        private void TestDrillDown()
        {
            PplFeatures.SetZoomProperties(true, true);

            PpOperations.SelectSlide(DrillDownActualStartSlideNo);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownActualStartSlideNo, DrillDownExpectedSlideNo);
            AssertIsSame(DrillDownActualTransitionSlideNo, DrillDownExpectedTransitionSlideNo);
            AssertIsSame(DrillDownActualEndSlideNo, DrillDownExpectedEndSlideNo);
        }

        private void TestDrillDownBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            PpOperations.SelectSlide(DrillDownBackgroundActualStartSlideNo);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownBackgroundActualStartSlideNo, DrillDownBackgroundExpectedSlideNo);
            AssertIsSame(DrillDownBackgroundActualTransitionSlideNo, DrillDownBackgroundExpectedTransitionSlideNo);
            AssertIsSame(DrillDownBackgroundActualEndSlideNo, DrillDownBackgroundExpectedEndSlideNo);
        }

        private void TestStepBack()
        {
            PplFeatures.SetZoomProperties(true, true);

            // StepBack 'starts' from the end slide
            PpOperations.SelectSlide(StepBackActualEndSlideNo);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            // StepBack creates a slide behind the end slide, causing the end slide's index to be incremented by 1
            const int StepBackActualTransitionSlideNo = StepBackActualEndSlideNo;

            AssertIsSame(StepBackActualStartSlideNo, StepBackExpectedSlideNo);
            AssertIsSame(StepBackActualTransitionSlideNo, StepBackExpectedTransitionSlideNo);
            AssertIsSame(StepBackActualEndSlideNo + 1, StepBackExpectedEndSlideNo);
        }

        private void TestStepBackBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            // StepBack 'starts' from the end slide
            PpOperations.SelectSlide(StepBackBackgroundActualEndSlideNo);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            // StepBack creates a slide behind the end slide, causing the end slide's index to be incremented by 1
            const int StepBackBackgroundActualTransitionSlideNo = StepBackBackgroundActualEndSlideNo;

            AssertIsSame(StepBackBackgroundActualStartSlideNo, StepBackBackgroundExpectedSlideNo);
            AssertIsSame(StepBackBackgroundActualTransitionSlideNo, StepBackBackgroundExpectedTransitionSlideNo);
            AssertIsSame(StepBackBackgroundActualEndSlideNo + 1, StepBackBackgroundExpectedEndSlideNo);
        }

        private void TestDrillDownUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(ErrorTestingSlideNo);
            slide.MoveTo(LastSlideSlideNo);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No next slide is found. Please select the correct slide.",
                PplFeatures.DrillDown);
        }

        private void TestStepBackUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(LastSlideSlideNo);
            slide.MoveTo(FirstSlideSlideNo);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No previous slide is found. Please select the correct slide.",
                PplFeatures.StepBack);
        }

        private void TestClipboardRestoredAfterDrillDown()
        {
            try
            {
                CheckIfClipboardIsRestored(() =>
                {
                    PplFeatures.SetZoomProperties(true, true);

                    PpOperations.SelectSlide(CheckClipboardRestoredAfterDrillDownActionSlideNo);
                    PpOperations.SelectShape("Drill Down This Shape");
                    PplFeatures.DrillDown();
                }, CheckClipboardRestoredAfterDrillDownOriginalSlideNo, ShapeToCopy, CheckClipboardRestoredAfterDrillDownExpectedSlideNo, ShapeToDelete, ExpCopiedShape);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                // Failed clipboard restore after drill down will usually result in copying multiple shapes
                if (e.Message.Equals("ShapeRange (unknown member) : Invalid request.  Command cannot be applied to a shape range with multiple shapes."))
                {
                    Assert.Fail("Failed to restore clipboard after DrillDown");
                }
                else
                {
                    throw;
                }
            }
        }

        private void TestClipboardRestoredAfterDrillDownBackground()
        {
            try {
                CheckIfClipboardIsRestored(() =>
                {
                    PplFeatures.SetZoomProperties(false, true);

                    PpOperations.SelectSlide(CheckClipboardRestoredAfterDrillDownBackgroundActionSlideNo);
                    PpOperations.SelectShape("Drill Down This Shape");
                    PplFeatures.DrillDown();
                }, CheckClipboardRestoredAfterDrillDownBackgroundOriginalSlideNo, ShapeToCopy, CheckClipboardRestoredAfterDrillDownBackgroundExpectedSlideNo, ShapeToDelete, ExpCopiedShape);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                // Failed clipboard restore after drill down will usually result in copying multiple shapes
                if (e.Message.Equals("ShapeRange (unknown member) : Invalid request.  Command cannot be applied to a shape range with multiple shapes."))
                {
                    Assert.Fail("Failed to restore clipboard after DrillDownBackground");
                }
                else
                {
                    throw;
                }
            }
        }

        private void TestClipboardRestoredAfterStepBack()
        {
            CheckIfClipboardIsRestored(() =>
            {
                PplFeatures.SetZoomProperties(true, true);

                PpOperations.SelectSlide(CheckClipboardRestoredAfterStepBackActionSlideNo);
                PpOperations.SelectShape("Step Back This Shape");
                PplFeatures.StepBack();
            }, CheckClipboardRestoredAfterStepBackOriginalSlideNo, ShapeToCopy, CheckClipboardRestoredAfterStepBackExpectedSlideNo, ShapeToDelete, ExpCopiedShape);
        }

        private void TestClipboardRestoredAfterStepBackBackground()
        {
            CheckIfClipboardIsRestored(() =>
            {
                PplFeatures.SetZoomProperties(false, true);

                PpOperations.SelectSlide(CheckClipboardRestoredAfterStepBackBackgroundActionSlideNo);
                PpOperations.SelectShape("Step Back This Shape");
                PplFeatures.StepBack();
            }, CheckClipboardRestoredAfterStepBackBackgroundOriginalSlideNo, ShapeToCopy, CheckClipboardRestoredAfterStepBackBackgroundExpectedSlideNo, ShapeToDelete, ExpCopiedShape);
        }


        private void AssertIsSame(int actualSlideIndex, int expectedSlideIndex)
        {
            Microsoft.Office.Interop.PowerPoint.Slide actualSlide = PpOperations.SelectSlide(actualSlideIndex);
            Microsoft.Office.Interop.PowerPoint.Slide expectedSlide = PpOperations.SelectSlide(expectedSlideIndex);
            SlideUtil.IsSameLooking(expectedSlide, actualSlide);
            SlideUtil.IsSameAnimations(expectedSlide, actualSlide);
        }
    }
}

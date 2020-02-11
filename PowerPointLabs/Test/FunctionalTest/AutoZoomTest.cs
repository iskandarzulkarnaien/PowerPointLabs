using Microsoft.VisualStudio.TestTools.UnitTesting;

using Test.Util;

namespace Test.FunctionalTest
{
    [TestClass]
    public class AutoZoomTest : BaseFunctionalTest
    {
        private const int DrillDownActualSourceSlideNo = 4;
        private const int DrillDownActualTransitionSlideNo = 5;
        private const int DrillDownActualDestinationSlideNo = 6;
        private const int DrillDownExpectedSlideNo = 7;
        private const int DrillDownExpectedTransitionSlideNo = 8;
        private const int DrillDownExpectedDestinationSlideNo = 9;

        private const int DrillDownBackgroundActualSourceSlideNo = 10;
        private const int DrillDownBackgroundActualTransitionSlideNo = 11;
        private const int DrillDownBackgroundActualDestinationSlideNo = 12;
        private const int DrillDownBackgroundExpectedSlideNo = 13;
        private const int DrillDownBackgroundExpectedTransitionSlideNo = 14;
        private const int DrillDownBackgroundExpectedDestinationSlideNo = 15;

        private const int StepBackActualSourceSlideNo = 16;
        private const int StepBackActualTransitionSlideNo = 17;
        private const int StepBackActualDestinationSlideNo = 18;
        private const int StepBackExpectedSlideNo = 20;
        private const int StepBackExpectedTransitionSlideNo = 21;
        private const int StepBackExpectedDestinationSlideNo = 22;

        private const int StepBackBackgroundActualSourceSlideNo = 23;
        private const int StepBackBackgroundActualTransitionSlideNo = 24;
        private const int StepBackBackgroundActualDestinationSlideNo = 25;
        private const int StepBackBackgroundExpectedSlideNo = 27;
        private const int StepBackBackgroundExpectedTransitionSlideNo = 28;
        private const int StepBackBackgroundExpectedDestinationSlideNo = 29;

        protected override string GetTestingSlideName()
        {
            return "ZoomLab\\AutoZoom.pptx";
        }

        [TestMethod]
        [TestCategory("FT")]
        public void FT_AutoZoomTest()
        {
            // Do tests in reverse order because added slides change slide numbers lower down.
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

            PpOperations.SelectSlide(4);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownActualSourceSlideNo, DrillDownExpectedSlideNo);
            AssertIsSame(DrillDownActualTransitionSlideNo, DrillDownExpectedTransitionSlideNo);
            AssertIsSame(DrillDownActualDestinationSlideNo, DrillDownExpectedDestinationSlideNo);
        }

        private void TestDrillDownBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            PpOperations.SelectSlide(10);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownBackgroundActualSourceSlideNo, DrillDownBackgroundExpectedSlideNo);
            AssertIsSame(DrillDownBackgroundActualTransitionSlideNo, DrillDownBackgroundExpectedTransitionSlideNo);
            AssertIsSame(DrillDownBackgroundActualDestinationSlideNo, DrillDownBackgroundExpectedDestinationSlideNo);
        }

        private void TestStepBack()
        {
            PplFeatures.SetZoomProperties(true, true);

            PpOperations.SelectSlide(17);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            AssertIsSame(StepBackActualSourceSlideNo, StepBackExpectedSlideNo);
            AssertIsSame(StepBackActualTransitionSlideNo, StepBackExpectedTransitionSlideNo);
            AssertIsSame(StepBackActualDestinationSlideNo, StepBackExpectedDestinationSlideNo);
        }

        private void TestStepBackBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            PpOperations.SelectSlide(24);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            AssertIsSame(StepBackBackgroundActualSourceSlideNo, StepBackBackgroundExpectedSlideNo);
            AssertIsSame(StepBackBackgroundActualTransitionSlideNo, StepBackBackgroundExpectedTransitionSlideNo);
            AssertIsSame(StepBackBackgroundActualDestinationSlideNo, StepBackBackgroundExpectedDestinationSlideNo);
        }

        private void TestDrillDownUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(34);
            slide.MoveTo(35);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No next slide is found. Please select the correct slide.",
                PplFeatures.DrillDown);
        }

        private void TestStepBackUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(35);
            slide.MoveTo(1);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No previous slide is found. Please select the correct slide.",
                PplFeatures.StepBack);
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

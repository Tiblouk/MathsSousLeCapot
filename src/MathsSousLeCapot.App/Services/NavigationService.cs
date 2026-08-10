using MathsSousLeCapot.App.Features.BinaryNumbers;
using MathsSousLeCapot.App.Features.Assessments;
using MathsSousLeCapot.App.Features.BasicOperations;
using MathsSousLeCapot.App.Features.Counting;
using MathsSousLeCapot.App.Features.FoundationNumbers;
using MathsSousLeCapot.App.Features.HighSchool;
using MathsSousLeCapot.App.Features.MiddleSchool;
using MathsSousLeCapot.App.Features.PositionalCounter;
using MathsSousLeCapot.App.Features.PrimaryCourses;
using MathsSousLeCapot.App.Features.Settings;
using MathsSousLeCapot.Core.Courses;

namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Centralise les routes de l'application et les fiches d'entrée des cours.
/// </summary>
public static class NavigationService
{
    /// <summary>
    /// Enregistre toutes les pages accessibles en dehors de la racine du Shell.
    /// </summary>
    public static void RegisterRoutes()
    {
        Register<CountingCourseEntryPage>();
        Register<CountingCoursePage>();
        Register<CountingTrainingPage>();
        Register<CourseEntryPage>();
        Register<CounterCoursePage>();
        Register<CounterTrainingPage>();
        Register<BinaryCourseEntryPage>();
        Register<BinaryCoursePage>();
        Register<BinaryTrainingPage>();
        Register<BasicOperationEntryPage>();
        Register<BasicOperationCoursePage>();
        Register<BasicOperationTrainingPage>();
        Register<PrimaryCourseEntryPage>();
        Register<PrimaryCoursePage>();
        Register<PrimaryTrainingPage>();
        Register<MiddleSchoolCourseEntryPage>();
        Register<MiddleSchoolCoursePage>();
        Register<MiddleSchoolTrainingPage>();
        Register<HighSchoolCourseEntryPage>();
        Register<HighSchoolCoursePage>();
        Register<HighSchoolTrainingPage>();
        Register<FoundationNumberCourseEntryPage>();
        Register<FoundationNumberCoursePage>();
        Register<FoundationNumberTrainingPage>();
        Register<ClassAssessmentPage>();
        Register<SettingsPage>();
    }

    /// <summary>
    /// Retourne la route de la fiche d'un cours lorsqu'elle est implémentée.
    /// </summary>
    public static bool TryGetCourseEntryRoute(string courseId, out string route)
    {
        route = courseId switch
        {
            CourseCatalog.CountingCourseId => nameof(CountingCourseEntryPage),
            CourseCatalog.PositionalCounterCourseId => nameof(CourseEntryPage),
            CourseCatalog.BinaryCourseId => nameof(BinaryCourseEntryPage),
            CourseCatalog.AdditionCourseId or CourseCatalog.SubtractionCourseId =>
                $"{nameof(BasicOperationEntryPage)}?courseId={courseId}",
            _ => string.Empty
        };
        if (route.Length > 0)
        {
            return true;
        }

        if (FoundationNumberCourseCatalog.TryGet(courseId, out _))
        {
            route =
                $"{nameof(FoundationNumberCourseEntryPage)}?courseId={courseId}";
            return true;
        }

        if (PrimaryCourseCatalog.TryGet(courseId, out _))
        {
            route = $"{nameof(PrimaryCourseEntryPage)}?courseId={courseId}";
            return true;
        }

        if (MiddleSchoolCourseCatalog.TryGet(courseId, out _))
        {
            route = $"{nameof(MiddleSchoolCourseEntryPage)}?courseId={courseId}";
            return true;
        }

        if (HighSchoolCourseCatalog.TryGet(courseId, out _))
        {
            route = $"{nameof(HighSchoolCourseEntryPage)}?courseId={courseId}";
            return true;
        }

        return false;
    }

    /// <summary>
    /// Enregistre une page avec son nom de type comme route stable.
    /// </summary>
    private static void Register<TPage>()
        where TPage : Element
    {
        Routing.RegisterRoute(typeof(TPage).Name, typeof(TPage));
    }
}

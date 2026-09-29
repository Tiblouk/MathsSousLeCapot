namespace MathsSousLeCapot.App.Services;

/// <summary>
/// Décrit un thème sélectionnable et l'ensemble de ses couleurs sémantiques.
/// </summary>
public sealed record ThemeDefinition(
    string Code,
    string NameKey,
    ThemePalette Palette,
    AppTheme AppTheme);

/// <summary>
/// Regroupe les couleurs globales utilisées par tous les composants visuels.
/// </summary>
public sealed record ThemePalette(
    string Primary,
    string PrimaryDark,
    string PrimaryLight,
    string Background,
    string CardBackground,
    string ControlBackground,
    string TextPrimary,
    string TextSecondary,
    string Border,
    string ExplanationBackground,
    string CourseRead,
    string CourseReadBackground,
    string CourseMasteredHard,
    string CourseMasteredHardBackground,
    string Success,
    string SuccessBackground,
    string Error,
    string ErrorBackground);

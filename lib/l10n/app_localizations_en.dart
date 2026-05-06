// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appName => 'VitalAct';

  @override
  String get tagline => 'A fun way to learn First Aid!';

  @override
  String get loading => 'Loading...';

  @override
  String get errorGeneral => 'Something went wrong. Please try again.';

  @override
  String get getStarted => 'Get Started';

  @override
  String get alreadyHaveAccount => 'I already have an account';

  @override
  String get loginTitle => 'Log In';

  @override
  String get signupTitle => 'Sign Up';

  @override
  String get email => 'Email';

  @override
  String get password => 'Password';

  @override
  String get continueAsGuest => 'Continue as guest';

  @override
  String get logout => 'Log Out';

  @override
  String get pleaseFixErrors => 'Please fix the errors';

  @override
  String get anonymousSignInFailed => 'Anonymous sign-in failed';

  @override
  String get errorInvalidEmail => 'Invalid email address';

  @override
  String get errorWrongPassword => 'Incorrect password';

  @override
  String get errorUserNotFound => 'User not found';

  @override
  String get nameSurname => 'Name Surname';

  @override
  String get emailLabel => 'Email:';

  @override
  String get passwordLabel => 'Password:';

  @override
  String get darkModeLabel => 'Dark Mode:';

  @override
  String get darkMode => 'Dark Mode';

  @override
  String get continueLesson => 'CONTINUE';

  @override
  String get leaveLessonTitle => 'Leave lesson?';

  @override
  String get leaveLessonMessage =>
      'Your progress in this lesson will not be saved.';

  @override
  String get stay => 'Stay';

  @override
  String get leave => 'Leave';

  @override
  String get continueButton => 'Continue';

  @override
  String get nextButton => 'NEXT';

  @override
  String get answerButton => 'ANSWER';

  @override
  String get typeYourAnswer => 'Type your answer...';

  @override
  String get checkingAnswer => 'Checking your answer...';

  @override
  String get analyzing => 'Analyzing...';

  @override
  String get correct => 'Correct!';

  @override
  String get notQuite => 'Not quite';

  @override
  String hintLabel(String hint) {
    return 'Hint: $hint';
  }

  @override
  String score(int score) {
    return 'Score: $score/10';
  }

  @override
  String get whatCouldBeImproved => 'What could be improved:';

  @override
  String get practice => 'Practice';

  @override
  String get practiceSubtitle => 'Sharpen your emergency skills';

  @override
  String get mentalDrill => 'Mental Drill';

  @override
  String get physicalDrill => 'Physical Drill';

  @override
  String get rapidResponse => 'Rapid Response';

  @override
  String get handPlacement => 'Hand Placement';

  @override
  String get cprPlacementTitle => 'CPR Placement';

  @override
  String get cprPlacementInstruction => 'Tap where CPR should be performed';

  @override
  String get cprPlacementSubtitle => 'Focus on the center of the chest';

  @override
  String get cprPlacementCorrectDetail =>
      'The heel of your hand on the center of the sternum.';

  @override
  String get cprPlacementIncorrectDetail =>
      'The correct spot is highlighted in green. Try again!';

  @override
  String get cprPlacementTapHint => 'Tap anywhere on the image above';

  @override
  String get cprPlacementCardSubtitle => 'Identify where on the chest';

  @override
  String get cprPlacementReadingTitle => 'Where to perform CPR';

  @override
  String get cprPlacementReadingDetail =>
      'Place the heel of your hand on the center of the chest — on the lower half of the breastbone (sternum). Place your other hand on top, interlace your fingers, and keep your elbows straight.';

  @override
  String get cprPlacementReadingCompression =>
      'Push hard and fast — compress at least 5 cm (2 inches) deep at 100–120 compressions per minute.';

  @override
  String get startTest => 'Start Test';

  @override
  String get tryAgain => 'Try Again';

  @override
  String get done => 'Done';
}

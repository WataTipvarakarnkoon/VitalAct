// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Thai (`th`).
class AppLocalizationsTh extends AppLocalizations {
  AppLocalizationsTh([String locale = 'th']) : super(locale);

  @override
  String get appName => 'VitalAct';

  @override
  String get proceed => 'ดำเนินการต่อ';

  @override
  String get cancel => 'ยกเลิก';

  @override
  String get back => 'ย้อนกลับ';

  @override
  String get next => 'ถัดไป';

  @override
  String get finish => 'เสร็จสิ้น';

  @override
  String get tagline => 'A fun way to learn First Aid!';

  @override
  String get getStarted => 'เริ่มต้นใช้งาน';

  @override
  String get alreadyHaveAccount => 'ฉันมีบัญชีอยู่แล้ว';

  @override
  String get loginTitle => 'เข้าสู้ระบบ';

  @override
  String get signupTitle => 'ลงทะเบียน';

  @override
  String get email => 'อีเมล';

  @override
  String get password => 'รหัสผ่าน';

  @override
  String get continueAsGuest => 'ดำเนินการต่อในฐานะผู้เยี่ยมชม';

  @override
  String get logout => 'ออกจากระบบ';

  @override
  String get errorInvalidEmail => 'Invalid email address';

  @override
  String get errorWrongPassword => 'Incorrect password';

  @override
  String get errorUserNotFound => 'User not found';

  @override
  String get lessonPathTitle => 'Lesson Path';

  @override
  String get locked => 'Locked';

  @override
  String get completed => 'Completed';

  @override
  String get startLesson => 'Start Lesson';

  @override
  String get exitLessonTitle => 'Exit lesson?';

  @override
  String get exitLessonMessage => 'Your progress will not be saved.';

  @override
  String get exit => 'Exit';

  @override
  String progress(Object current, Object total) {
    return '$current / $total';
  }

  @override
  String get chooseBestAnswer => 'Choose the best answer';

  @override
  String get submitAnswer => 'Submit';

  @override
  String get correctAnswer => 'Correct!';

  @override
  String get wrongAnswer => 'Try again';

  @override
  String get scenarioTitle => 'Scenario';

  @override
  String get whatWouldYouDo => 'What would you do?';

  @override
  String get excellentResponse => 'Excellent response!';

  @override
  String get goodResponse => 'Good response';

  @override
  String get improveResponse => 'What could be improved';

  @override
  String get normal => 'Normal';

  @override
  String get emergency => 'Emergency';

  @override
  String get normalVsAbnormal => 'Normal vs abnormal signs';

  @override
  String get lifeThreatening => 'Life-threatening red flags';

  @override
  String score(Object score) {
    return 'Score: $score/10';
  }

  @override
  String get whatYouDidWell => 'What you did well';

  @override
  String get whatToImprove => 'What could improve';

  @override
  String get mentalDrill => 'Mental Drill';

  @override
  String get physicalDrill => 'Physical Drill';

  @override
  String get practiceSubtitle => 'Sharpen your emergency skills';

  @override
  String get emergencySimulation => 'Emergency Simulation';

  @override
  String get cprTrainer => 'CPR Trainer';

  @override
  String get darkMode => 'Dark Mode';

  @override
  String get nameSurname => 'Name Surname';
}

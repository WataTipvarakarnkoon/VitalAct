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
  String get tagline => 'A fun way to learn First Aid!';

  @override
  String get loading => 'กำลังโหลด...';

  @override
  String get errorGeneral => 'เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง';

  @override
  String get getStarted => 'เริ่มต้นใช้งาน';

  @override
  String get alreadyHaveAccount => 'ฉันมีบัญชีอยู่แล้ว';

  @override
  String get loginTitle => 'เข้าสู่ระบบ';

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
  String get pleaseFixErrors => 'กรุณาแก้ไขข้อผิดพลาด';

  @override
  String get anonymousSignInFailed => 'การเข้าสู่ระบบแบบไม่ระบุตัวตนล้มเหลว';

  @override
  String get errorInvalidEmail => 'ที่อยู่อีเมลไม่ถูกต้อง';

  @override
  String get errorWrongPassword => 'รหัสผ่านไม่ถูกต้อง';

  @override
  String get errorUserNotFound => 'ไม่พบผู้ใช้';

  @override
  String get nameSurname => 'ชื่อ นามสกุล';

  @override
  String get emailLabel => 'อีเมล:';

  @override
  String get passwordLabel => 'รหัสผ่าน:';

  @override
  String get darkModeLabel => 'โหมดมืด:';

  @override
  String get darkMode => 'โหมดมืด';

  @override
  String get continueLesson => 'ดำเนินการต่อ';

  @override
  String get leaveLessonTitle => 'ออกจากบทเรียน?';

  @override
  String get leaveLessonMessage => 'ความคืบหน้าในบทเรียนนี้จะไม่ถูกบันทึก';

  @override
  String get stay => 'อยู่ต่อ';

  @override
  String get leave => 'ออก';

  @override
  String get continueButton => 'ดำเนินการต่อ';

  @override
  String get nextButton => 'ถัดไป';

  @override
  String get answerButton => 'ตอบ';

  @override
  String get typeYourAnswer => 'พิมพ์คำตอบของคุณ...';

  @override
  String get checkingAnswer => 'กำลังตรวจสอบคำตอบ...';

  @override
  String get analyzing => 'กำลังวิเคราะห์...';

  @override
  String get correct => 'ถูกต้อง!';

  @override
  String get notQuite => 'ไม่ถูกทั้งหมด';

  @override
  String hintLabel(String hint) {
    return 'คำใบ้: $hint';
  }

  @override
  String score(int score) {
    return 'คะแนน: $score/10';
  }

  @override
  String get whatCouldBeImproved => 'สิ่งที่ควรปรับปรุง:';

  @override
  String get practice => 'ฝึกฝน';

  @override
  String get practiceSubtitle => 'ฝึกฝนทักษะฉุกเฉินของคุณ';

  @override
  String get mentalDrill => 'ฝึกความคิด';

  @override
  String get physicalDrill => 'ฝึกร่างกาย';

  @override
  String get rapidResponse => 'เกมตอบคำถาม';

  @override
  String get handPlacement => 'การวางมือ';

  @override
  String get cprPlacementTitle => 'การวางมือ CPR';

  @override
  String get cprPlacementInstruction => 'แตะที่จุดที่ควรทำ CPR';

  @override
  String get cprPlacementSubtitle => 'มุ่งไปที่กลางหน้าอก';

  @override
  String get cprPlacementCorrectDetail => 'วางส้นมือไว้ที่กลางกระดูกหน้าอก';

  @override
  String get cprPlacementIncorrectDetail =>
      'จุดที่ถูกต้องแสดงด้วยสีเขียว ลองอีกครั้ง!';

  @override
  String get cprPlacementTapHint => 'แตะที่ใดก็ได้บนภาพด้านบน';

  @override
  String get cprPlacementCardSubtitle => 'ระบุจุดบนหน้าอก';

  @override
  String get cprPlacementReadingTitle => 'ตำแหน่งที่ควรทำ CPR';

  @override
  String get cprPlacementReadingDetail =>
      'วางส้นมือไว้ตรงกลางหน้าอก — บริเวณครึ่งล่างของกระดูกอก (กระดูกหน้าอก) วางมืออีกข้างซ้อนด้านบน สอดนิ้วและตั้งข้อศอกให้ตรง.';

  @override
  String get cprPlacementReadingCompression =>
      'กดให้แรงและเร็ว — กดลึกอย่างน้อย 5 ซม. (2 นิ้ว) ด้วยความถี่ 100–120 ครั้งต่อนาที.';

  @override
  String get startTest => 'เริ่มทดสอบ';

  @override
  String get tryAgain => 'ลองอีกครั้ง';

  @override
  String get done => 'เสร็จสิ้น';
}

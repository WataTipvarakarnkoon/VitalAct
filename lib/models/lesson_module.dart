import 'package:vitalact/models/lesson_item.dart';

class LessonModule {
  final String moduleTitle;
  final List<LessonItem> lessons;

  LessonModule({
    required this.moduleTitle,
    required this.lessons,
  });

  factory LessonModule.fromJson(Map<String, dynamic> json) {
    return LessonModule(
      moduleTitle: json['moduleTitle'],
      lessons:
          (json['lessons'] as List).map((e) => LessonItem.fromJson(e)).toList(),
    );
  }
}

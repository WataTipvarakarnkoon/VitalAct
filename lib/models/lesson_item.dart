import 'lesson_step.dart';
import '../services/step_factory.dart';

class LessonItem {
  final String id;
  final String title;
  final String spriteAsset;
  final List<LessonStep> steps;

  LessonItem({
    required this.id,
    required this.title,
    required this.spriteAsset,
    required this.steps,
  });

  factory LessonItem.fromJson(Map<String, dynamic> json) {
    return LessonItem(
      id: json['id'],
      title: json['title'],
      spriteAsset: json['spriteAsset'],
      steps: (json['steps'] as List)
          .map((step) => StepFactory.build(step))
          .toList(),
    );
  }
}

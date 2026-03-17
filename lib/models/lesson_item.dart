import '../models/lesson_step.dart';

class LessonItem {
  final String title;
  final List<LessonStep> steps;
  final String? spriteAsset;

  const LessonItem({
    required this.title,
    required this.steps,
    this.spriteAsset,
  });
}

import '../models/lesson_step.dart';

class LessonItem {
  final String title;
  final List<LessonStep> steps;

  const LessonItem({
    required this.title,
    required this.steps,
  });
}

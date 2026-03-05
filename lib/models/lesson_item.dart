import '../models/lesson_step.dart';

class LessonItem {
  final String title;
  final String description;
  final List<LessonStep> steps;

  const LessonItem({
    required this.title,
    required this.description,
    required this.steps,
  });
}

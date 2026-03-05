import '../lesson_step.dart';

class ReadingStep extends LessonStep {
  final String title;
  final String content;

  const ReadingStep({
    required super.id,
    required this.title,
    required this.content,
  });
}

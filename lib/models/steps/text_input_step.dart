import 'package:vitalact/models/lesson_step.dart';

class TextInputStep extends LessonStep {
  final String instructions;
  final String title;
  final String imageAsset;

  final List<String> correctAnswers;

  final String aiPrompt;
  final String hint;

  const TextInputStep({
    required super.id,
    required this.instructions,
    required this.title,
    required this.imageAsset,
    required this.correctAnswers,
    required this.aiPrompt,
    required this.hint,
  });
}

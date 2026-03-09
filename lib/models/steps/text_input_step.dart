import 'package:vitalact/models/lesson_step.dart';

class TextInputStep extends LessonStep {
  final String instructions;
  final String title;
  final String imageAsset;

  final List<String> correctAnswers;

  final String correctExplanation;
  final String incorrectExplanation;
  final String hint;

  const TextInputStep({
    required super.id,
    required this.instructions,
    required this.title,
    required this.imageAsset,
    required this.correctAnswers,
    required this.correctExplanation,
    required this.incorrectExplanation,
    required this.hint,
  });
}

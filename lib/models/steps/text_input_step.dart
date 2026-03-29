import 'package:vitalact/models/lesson_step.dart';

class TextInputStep extends LessonStep {
  final String instructions;
  final String title;
  final String spriteAsset;

  final String aiPrompt;
  final String hint;

  const TextInputStep({
    required super.id,
    required this.instructions,
    required this.title,
    required this.spriteAsset,
    required this.aiPrompt,
    required this.hint,
  });
}

import 'package:vitalact/models/lesson_step.dart';

class TextInputStep extends LessonStep {
  final String title;
  final String instructions;

  final String aiPrompt;
  final String hint;
  final String spriteAsset;
  final String disclaimer;

  TextInputStep({
    required super.id,
    required this.title,
    required this.instructions,
    required this.aiPrompt,
    required this.hint,
    required this.spriteAsset,
    this.disclaimer = '',
    required super.type,
  });
}

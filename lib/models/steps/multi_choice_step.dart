import '../lesson_step.dart';

class MultiChoiceStep extends LessonStep {
  final String title;
  final String instructions;
  final List<String> choices;
  final int correctIndex;
  final String correctExplanation;
  final String incorrectExplanation;
  final String hint;
  final String disclaimer;
  final String spriteAsset;

  MultiChoiceStep({
    required super.id,
    required this.title,
    required this.instructions,
    required this.choices,
    required this.correctIndex,
    required this.hint,
    required this.spriteAsset,
    required this.correctExplanation,
    required this.incorrectExplanation,
    required this.disclaimer,
  });
}
